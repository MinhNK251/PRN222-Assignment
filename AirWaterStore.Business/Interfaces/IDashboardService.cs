using AirWaterStore.Data.Models;
using AirWaterStore.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirWaterStore.Business.Interfaces
{
    public interface IDashboardService
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
