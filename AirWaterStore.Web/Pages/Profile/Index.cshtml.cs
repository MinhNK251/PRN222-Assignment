using AirWaterStore.Business.Interfaces;
using AirWaterStore.Data.Models;
using AirWaterStore.Web.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AirWaterStore.Web.Pages.Profile
{
    public class IndexModel : PageModel
    {
        private readonly IUserService _userService;
        private readonly IOrderService _orderService;
        private readonly IWishlistService _wishlistService;
        private readonly IReviewService _reviewService;

        public IndexModel(IUserService userService, IOrderService orderService, 
            IWishlistService wishlistService, IReviewService reviewService)
        {
            _userService = userService;
            _orderService = orderService;
            _wishlistService = wishlistService;
            _reviewService = reviewService;
        }

        public User User { get; set; } = default!;
        public int TotalOrders { get; set; }
        public int WishlistCount { get; set; }
        public int ReviewCount { get; set; }
        public decimal TotalSpent { get; set; }
        public List<Order> RecentOrders { get; set; } = new List<Order>();
        public List<Game> RecentWishlistGames { get; set; } = new List<Game>();

        public async Task<IActionResult> OnGetAsync()
        {
            if (!this.IsAuthenticated())
            {
                return RedirectToPage("/Login");
            }

            var userId = this.GetCurrentUserId();
            var user = await _userService.GetByIdAsync(userId);
            
            if (user == null)
            {
                return NotFound();
            }

            User = user;

            if (this.IsCustomer())
            {
                TotalOrders = await _orderService.GetTotalCountByUserIdAsync(userId);
                WishlistCount = await _wishlistService.GetTotalCountByUserIdAsync(userId);
                ReviewCount = await GetUserReviewCountAsync(userId);
                TotalSpent = await GetTotalSpentAsync(userId);
                RecentOrders = await _orderService.GetAllByUserIdAsync(userId, 1, 3);
                RecentWishlistGames = await _wishlistService.GetWishlistGamesForUserAsync(userId, 1, 3);
            }

            return Page();
        }

        private async Task<int> GetUserReviewCountAsync(int userId)
        {
            try
            {
                // Get all user's orders to find games they've purchased
                var allOrders = await _orderService.GetAllByUserIdAsync(userId, 1, 1000);
                var purchasedGameIds = allOrders
                    .SelectMany(o => o.OrderDetails)
                    .Select(od => od.GameId)
                    .Distinct()
                    .ToList();

                int reviewCount = 0;
                
                // Check each game for user's review
                foreach (var gameId in purchasedGameIds)
                {
                    var gameReviews = await _reviewService.GetAllByGameIdAsync(gameId);
                    var userReview = gameReviews.FirstOrDefault(r => r.UserId == userId);
                    if (userReview != null)
                    {
                        reviewCount++;
                    }
                }

                return reviewCount;
            }
            catch
            {
                return 0;
            }
        }

        private async Task<decimal> GetTotalSpentAsync(int userId)
        {
            try
            {
                // Get all completed orders for the user
                var allOrders = await _orderService.GetAllByUserIdAsync(userId, 1, 1000); // Get all orders
                var completedOrders = allOrders.Where(o => o.Status == OrderStatus.Completed);
                
                return completedOrders.Sum(o => o.TotalPrice);
            }
            catch
            {
                return 0;
            }
        }

        public string GetOrderStatusBadgeClass(string status)
        {
            return status switch
            {
                OrderStatus.Completed => "badge-success",
                OrderStatus.Pending => "badge-warning",
                OrderStatus.Failed => "badge-danger",
                _ => "badge-secondary"
            };
        }

        public string GetOrderStatusIcon(string status)
        {
            return status switch
            {
                OrderStatus.Completed => "fas fa-check-circle",
                OrderStatus.Pending => "fas fa-clock",
                OrderStatus.Failed => "fas fa-times-circle",
                _ => "fas fa-question-circle"
            };
        }
    }
}