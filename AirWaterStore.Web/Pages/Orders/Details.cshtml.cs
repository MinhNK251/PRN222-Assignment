using AirWaterStore.Business.Interfaces;
using AirWaterStore.Data.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AirWaterStore.Web.Pages.Orders
{
    public class DetailsModel : PageModel
    {
        private readonly IOrderService _orderService;
        private readonly IOrderDetailService _orderDetailService;
        private readonly IUserService _userService;

        public DetailsModel(IOrderService orderService, IOrderDetailService orderDetailService, IUserService userService)
        {
            _orderService = orderService;
            _orderDetailService = orderDetailService;
            _userService = userService;
        }

        public Order Order { get; set; } = default!;
        public List<OrderDetail> OrderDetails { get; set; } = new();
        public string CustomerName { get; set; } = string.Empty;

        public bool IsStaff => HttpContext.Session.GetInt32("UserRole") == 2;
        public int? CurrentUserId => HttpContext.Session.GetInt32("UserId");

        public async Task<IActionResult> OnGetAsync(int id)
        {
            if (!CurrentUserId.HasValue)
            {
                return RedirectToPage("/Login");
            }

            var order = await _orderService.GetByIdAsync(id);

            if (order == null)
            {
                return NotFound();
            }

            Order = order;

            // Check authorization - customers can only see their own orders
            if (!IsStaff && Order.UserId != CurrentUserId.Value)
            {
                return Forbid();
            }

            OrderDetails = await _orderDetailService.GetAllByOrderIdAsync(id);

            var customer = await _userService.GetByIdAsync(Order.UserId);
            CustomerName = customer?.Username ?? "Unknown Customer";

            return Page();
        }

        public async Task<IActionResult> OnPostUpdateStatusAsync(int orderId, string status)
        {
            if (!IsStaff)
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
    }
}