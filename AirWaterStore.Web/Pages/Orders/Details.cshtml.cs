using AirWaterStore.Business.Interfaces;
using AirWaterStore.Business.Library;
using AirWaterStore.Business.LibraryS;
using AirWaterStore.Data.Models;
using AirWaterStore.Web.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Options;

namespace AirWaterStore.Web.Pages.Orders;

public class DetailsModel : PageModel
{
    private readonly IOrderService _orderService;
    private readonly IOrderDetailService _orderDetailService;
    private readonly IUserService _userService;
    private readonly IVnPayService _paymentService;
    private readonly VnPayConfig _vpnPayConfig;

    public DetailsModel(IOrderService orderService, IOrderDetailService orderDetailService, IUserService userService, IVnPayService vpnPayService, IOptions<VnPayConfig> vnpayConfig)
    {
        _orderService = orderService;
        _orderDetailService = orderDetailService;
        _userService = userService;
        _paymentService = vpnPayService;
        _vpnPayConfig = vnpayConfig.Value;
    }

    [BindProperty(SupportsGet = true)]
    public string Vnp_TxnRef { get; set; }
    [BindProperty(SupportsGet = true)]
    public string Vnp_TransactionStatus { get; set; }
    [BindProperty(SupportsGet = true)]
    public string Vnp_ResponseCode { get; set; }
    [BindProperty(SupportsGet = true)]
    public string Vnp_SecureHash { get; set; }

    public Order Order { get; set; } = default!;
    public List<OrderDetail> OrderDetails { get; set; } = new();
    public string CustomerName { get; set; } = string.Empty;

    // public bool IsStaff => HttpContext.Session.GetInt32(SessionParams.UserRole) == UserRole.Staff;
    // public int? CurrentUserId => HttpContext.Session.GetInt32(SessionParams.UserId);

    public async Task<IActionResult> OnGetAsync(int? id)
    {
        if (!this.IsAuthenticated())
        {
            return RedirectToPage("/Login");
        }

        Order? order = null;

        if (!string.IsNullOrEmpty(Vnp_TxnRef))
        {
            order = await _orderService.GetByIdAsync(VnPayHelper.ExtractOrderId(Vnp_TxnRef));

            var vnpay = new VnPayLibrary();

            foreach (var (key, value) in Request.Query)
            {
#pragma warning disable CS8604 // Possible null reference argument.
                if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                {
                    vnpay.AddResponseData(key, value);
                }
#pragma warning restore CS8604 // Possible null reference argument.
            }

            var checkSigniture = vnpay.ValidateSignature(Vnp_SecureHash, _vpnPayConfig.HashSecret);

            if (checkSigniture)
            {
                if (Vnp_ResponseCode.Equals("00") && Vnp_TransactionStatus.Equals("00"))
                {
                    Console.WriteLine("Giao dich thanh cong");
                    if (order != null && !order.Status.Equals(OrderStatus.Completed))
                    {
                        order.Status = OrderStatus.Completed;
                        await _orderService.UpdateAsync(order);
                    }
                }
                else
                {
                    Console.WriteLine("Giao dich that bai");
                }
            }
        }
        else
        {
            if (id != null)
            {
                order = await _orderService.GetByIdAsync((int)id);
            }
        }

        if (order == null)
        {
            return NotFound();
        }

        Order = order;

        // Check authorization - customers can only see their own orders
        if (!this.IsStaff() && Order.UserId != this.GetCurrentUserId())
        {
            return Forbid();
        }

        OrderDetails = await _orderDetailService.GetAllByOrderIdAsync(Order.OrderId);

        var customer = await _userService.GetByIdAsync(Order.UserId);
        CustomerName = customer?.Username ?? "Unknown Customer";

        return Page();
    }

    public async Task<IActionResult> OnPostUpdateStatusAsync(int orderId, string status)
    {
        if (!this.IsStaff())
        {
            return Forbid();
        }

        var order = await _orderService.GetByIdAsync(orderId);
        if (order != null)
        {
            order.Status = status;
            await _orderService.UpdateAsync(order);
        }

        return RedirectToPage(new { id = orderId });
    }

    public async Task<IActionResult> OnPostCheckOutAsync(int orderId)
    {
        if (this.IsStaff())
        {
            return Unauthorized();
        }

        var order = await _orderService.GetByIdAsync(orderId);
        if (order == null)
        {
            return NotFound();
        }

        string paymentLink = _paymentService.CreatePayment(order);

        return Redirect(paymentLink);
    }

}