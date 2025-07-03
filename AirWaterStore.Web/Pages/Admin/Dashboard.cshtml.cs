using AirWaterStore.Business.Interfaces;
using AirWaterStore.Data.Models;
using AirWaterStore.Web.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AirWaterStore.Web.Pages.Admin
{
    public class DashboardModel : PageModel
    {
        private readonly IGameService _gameService;
        private readonly IOrderService _orderService;
        private readonly IUserService _userService;
        private readonly IChatRoomService _chatRoomService;

        public DashboardModel(IGameService gameService, IOrderService orderService,
            IUserService userService, IChatRoomService chatRoomService)
        {
            _gameService = gameService;
            _orderService = orderService;
            _userService = userService;
            _chatRoomService = chatRoomService;
        }

        public int TotalGames { get; set; }
        public int TotalOrders { get; set; }
        public int TotalUsers { get; set; }
        public int PendingChats { get; set; }
        public List<Order> RecentOrders { get; set; } = new List<Order>();
        public Dictionary<int, string> UserNames { get; set; } = new Dictionary<int, string>();

        public async Task<IActionResult> OnGetAsync()
        {
            // Check if user is staff
            if (!this.IsStaff())
            {
                return RedirectToPage("/Login");
            }

            // Get statistics
            TotalGames = await _gameService.GetTotalCountAsync();
            TotalOrders = await _orderService.GetTotalCountAsync();
            TotalUsers = await _userService.GetTotalCountAsync();

            // Get pending chats (chats without assigned staff)
            // var userId = HttpContext.Session.GetInt32(SessionParams.UserId);
            var userId = this.GetCurrentUserId();
            var allChats = await _chatRoomService.GetChatRoomsByUserIdAsync(userId);
            PendingChats = allChats.Count(c => c.StaffId == null);

            // Get recent orders
            RecentOrders = await _orderService.GetAllAsync(1, 5);

            // Load usernames for orders
            foreach (var order in RecentOrders)
            {
                if (!UserNames.ContainsKey(order.UserId))
                {
                    var user = await _userService.GetByIdAsync(order.UserId);
                    UserNames[order.UserId] = user?.Username ?? "Unknown User";
                }
            }

            return Page();
        }

        public string GetUsername(int userId)
        {
            return UserNames.TryGetValue(userId, out var username) ? username : "Unknown User";
        }
    }
}