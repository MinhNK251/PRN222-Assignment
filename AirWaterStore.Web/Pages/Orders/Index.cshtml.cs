using AirWaterStore.Business.Interfaces;
using AirWaterStore.Data.Models;
using AirWaterStore.Web.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AirWaterStore.Web.Pages.Orders
{
    public class IndexModel : PageModel
    {
        private readonly IOrderService _orderService;
        private const int PageSize = 10;

        public IndexModel(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public List<Order> Orders { get; set; } = new List<Order>();
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }

        public async Task<IActionResult> OnGetAsync(int currentPage = 1)
        {
            var userId = HttpContext.Session.GetInt32(SessionParams.UserId);
            var userRole = HttpContext.Session.GetInt32(SessionParams.UserRole);

            if (!userId.HasValue)
            {
                return RedirectToPage("/Login");
            }

            CurrentPage = currentPage;

            if (userRole == 2) // Staff sees all orders
            {
                Orders = await _orderService.GetAllAsync(currentPage, PageSize);
                var totalCount = await _orderService.GetTotalCountAsync();
                TotalPages = (int)Math.Ceiling(totalCount / (double)PageSize);
            }
            else // Customer sees only their orders
            {
                Orders = await _orderService.GetAllByUserIdAsync(userId.Value, currentPage, PageSize);
                var totalCount = await _orderService.GetTotalCountByUserIdAsync(userId.Value);
                TotalPages = (int)Math.Ceiling(totalCount / (double)PageSize);
            }

            return Page();
        }
    }
}