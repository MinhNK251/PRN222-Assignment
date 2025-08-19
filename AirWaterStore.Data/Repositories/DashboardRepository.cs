using AirWaterStore.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace AirWaterStore.Data.Repositories
{
    public class DashboardRepository : IDashboardRepository
    {
        private readonly AirWaterStoreContext _context;

        public DashboardRepository(AirWaterStoreContext context)
        {
            _context = context;
        }

        // Get Total Revenue (All Time)
        public async Task<decimal> GetTotalRevenueAsync()
        {
            var totalRevenue = await _context.Payments
                .Where(p => p.Status == "Completed")
                .SumAsync(p => p.TotalPrice);

            return totalRevenue;
        }

        // Get the total income for a given month and year
        public async Task<decimal> GetMonthlyIncomeAsync(int month, int year)
        {
            var totalIncome = await _context.Payments
                .Where(p => p.PaymentDate.Month == month && p.PaymentDate.Year == year && p.Status == "Completed")
                .SumAsync(p => p.TotalPrice);

            return totalIncome;
        }

        // Get the total income for a given year
        public async Task<decimal> GetYearlyIncomeAsync(int year)
        {
            var totalIncome = await _context.Payments
                .Where(p => p.PaymentDate.Year == year && p.Status == "Completed")
                .SumAsync(p => p.TotalPrice);

            return totalIncome;
        }

        // Get the 5 most paying users for a given period (Month/Year)
        public async Task<List<UserPaymentStats>> GetUserPaymentStatsAsync(int month, int year)
        {
            var userPayments = await _context.Payments
                .Where(p => p.PaymentDate.Month == month && p.PaymentDate.Year == year && p.Status == "Completed")
                .GroupBy(p => p.Order.UserId)
                .Select(g => new UserPaymentStats
                {
                    UserId = g.Key,
                    TotalAmountSpent = g.Sum(p => p.TotalPrice)
                })
                .OrderByDescending(u => u.TotalAmountSpent)
                .ToListAsync();

            return userPayments;
        }

        // Get the most and least ordered games for a given period (Month/Year)
        public async Task<List<GameOrderStats>> GetGameOrderStatsAsync(int month, int year)
        {
            var gameOrders = await _context.OrderDetails
                .Where(od => od.Order.OrderDate.Month == month && od.Order.OrderDate.Year == year && od.Order.Status == "Completed")
                .GroupBy(od => od.GameId)
                .Select(g => new GameOrderStats
                {
                    GameId = g.Key,
                    TotalOrders = g.Sum(od => od.Quantity)
                })
                .OrderByDescending(g => g.TotalOrders)
                .ToListAsync();

            return gameOrders;
        }

        // Get the trending games for a given period (Month/Year) based on order quantity
        public async Task<List<GameOrderStats>> GetTrendingGamesAsync(int month, int year)
        {
            var trendingGames = await GetGameOrderStatsAsync(month, year);

            return trendingGames
                .Where(g => g.TotalOrders > 0)
                .Take(5) // Top 5 trending games
                .ToList();
        }

        // Get Low Stock Alerts (5 Games with Quantity <= 10)
        public async Task<List<Game>> GetLowStockAlertsAsync()
        {
            var lowStockGames = await _context.Games
                .Where(g => g.Quantity <= 10)
                .OrderBy(g => g.Quantity)
                .Take(5)
                .ToListAsync();

            return lowStockGames;
        }

        // Get Top Wishlist Games
        public async Task<List<GameWishlistStats>> GetTopWishlistGameAsync()
        {
            var topWishlistGames = await _context.Wishlists
                .GroupBy(w => w.GameId)
                .Select(g => new GameWishlistStats
                {
                    GameId = g.Key,
                    Title = g.FirstOrDefault().Game.Title,
                    WishlistCount = g.Count()
                })
                .OrderByDescending(g => g.WishlistCount)
                .Take(5)
                .ToListAsync();

            return topWishlistGames;
        }

        public async Task<List<CommissionRequest>> GetTopCommissionAsync()
        {
            var topCommissionRequests = await _context.CommissionRequests
            .OrderByDescending(c => c.Upvotes)
            .Take(5)
            .ToListAsync();
            return topCommissionRequests;
        }
    }

    public class UserPaymentStats
    {
        public int UserId { get; set; }
        public decimal TotalAmountSpent { get; set; }
    }

    public class GameOrderStats
    {
        public int GameId { get; set; }
        public int TotalOrders { get; set; }
    }

    public class GameWishlistStats
    {
        public int GameId { get; set; }
        public string Title { get; set; }
        public int WishlistCount { get; set; }
    }
}
