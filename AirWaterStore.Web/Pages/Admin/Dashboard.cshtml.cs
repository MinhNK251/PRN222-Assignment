using AirWaterStore.Business.Interfaces;
using AirWaterStore.Data.Models;
using AirWaterStore.Data.Repositories;
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
        private readonly IDashboardService _dashboardService;

        public DashboardModel(IGameService gameService, IOrderService orderService,
            IUserService userService, IChatRoomService chatRoomService, IDashboardService dashboardService)
        {
            _gameService = gameService;
            _orderService = orderService;
            _userService = userService;
            _chatRoomService = chatRoomService;
            _dashboardService = dashboardService;
        }

        public decimal TotalRevenue { get; set; }
        public decimal TodayRevenue { get; set; }
        public decimal MonthlyRevenue { get; set; }
        public decimal YearlyRevenue { get; set; }
        public int TotalGames { get; set; }
        public int TotalOrders { get; set; }
        public int TotalUsers { get; set; }
        public int PendingChats { get; set; }
        public List<Order> RecentOrders { get; set; } = new List<Order>();
        public Dictionary<int, string> UserNames { get; set; } = new Dictionary<int, string>();
        public List<UserPaymentStats> UserPaymentStats { get; set; } = new List<UserPaymentStats>();
        public List<GameOrderStats> GameOrderStats { get; set; } = new List<GameOrderStats>();
        public List<GameOrderStats> TrendingGames { get; set; } = new List<GameOrderStats>();
        public List<Game> LowStockGames { get; set; } = new List<Game>();
        public List<GameWishlistStats> TopWishlistGames { get; set; } = new List<GameWishlistStats>();
        public List<CommissionRequest> TopCommissionRequests { get; set; } = new List<CommissionRequest>();
        
        [BindProperty(SupportsGet = true)]
        public int SelectedMonth { get; set; } = DateTime.Now.Month;
        [BindProperty(SupportsGet = true)]
        public int SelectedYear { get; set; } = DateTime.Now.Year;

        public async Task<IActionResult> OnGetAsync()
        {
            // Check if user is staff
            if (!this.IsStaff())
            {
                return RedirectToPage("/Login");
            }

            // Get statistics
            TotalRevenue = await _dashboardService.GetTotalRevenueAsync();
            TodayRevenue = await _dashboardService.GetTodayRevenueAsync();
            MonthlyRevenue = await _dashboardService.GetMonthlyIncomeAsync(SelectedMonth, SelectedYear);
            YearlyRevenue = await _dashboardService.GetYearlyIncomeAsync(SelectedYear);
            TotalGames = await _gameService.GetTotalCountAsync();
            TotalOrders = await _orderService.GetTotalCountAsync();
            TotalUsers = await _userService.GetTotalCountAsync();
            UserPaymentStats = await _dashboardService.GetUserPaymentStatsAsync(SelectedMonth, SelectedYear);
            GameOrderStats = await _dashboardService.GetGameOrderStatsAsync(SelectedMonth, SelectedYear);
            TrendingGames = await _dashboardService.GetTrendingGamesAsync(SelectedMonth, SelectedYear);
            TopWishlistGames = await _dashboardService.GetTopWishlistGameAsync();
            TopCommissionRequests = await _dashboardService.GetTopCommissionAsync();
            LowStockGames = await _dashboardService.GetLowStockAlertsAsync();

            // Get pending chats (chats without assigned staff)
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