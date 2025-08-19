using AirWaterStore.Data.Models;

namespace AirWaterStore.Data.Repositories
{
    public interface IDashboardRepository
    {
        Task<decimal> GetTotalRevenueAsync();
        Task<decimal> GetMonthlyIncomeAsync(int month, int year);
        Task<decimal> GetYearlyIncomeAsync(int year);
        Task<List<UserPaymentStats>> GetUserPaymentStatsAsync(int month, int year);
        Task<List<GameOrderStats>> GetGameOrderStatsAsync(int month, int year);
        Task<List<GameOrderStats>> GetTrendingGamesAsync(int month, int year);
        Task<List<Game>> GetLowStockAlertsAsync();
        Task<List<GameWishlistStats>> GetTopWishlistGameAsync();
        Task<List<CommissionRequest>> GetTopCommissionAsync();
    }
}