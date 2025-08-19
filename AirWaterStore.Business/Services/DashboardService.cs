using AirWaterStore.Business.Interfaces;
using AirWaterStore.Data.Models;
using AirWaterStore.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirWaterStore.Business.Services
{
    public class DashboardService : IDashboardService
    {
        private readonly IDashboardRepository _repository;
        public DashboardService(IDashboardRepository repository)
        {
            _repository = repository;
        }

        public async Task<decimal> GetTotalRevenueAsync()
        {
            return await _repository.GetTotalRevenueAsync();
        }

        public async Task<decimal> GetMonthlyIncomeAsync(int month, int year)
        {
            return await _repository.GetMonthlyIncomeAsync(month, year);
        }

        public async Task<decimal> GetYearlyIncomeAsync(int year)
        {
            return await _repository.GetYearlyIncomeAsync(year);
        }

        public async Task<List<UserPaymentStats>> GetUserPaymentStatsAsync(int month, int year)
        {
            return await _repository.GetUserPaymentStatsAsync(month, year);
        }

        public async Task<List<GameOrderStats>> GetGameOrderStatsAsync(int month, int year)
        {
            return await _repository.GetGameOrderStatsAsync(month, year);
        }

        public async Task<List<GameOrderStats>> GetTrendingGamesAsync(int month, int year)
        {
            return await _repository.GetTrendingGamesAsync(month, year);
        }

        public async Task<List<Game>> GetLowStockAlertsAsync()
        {
            return await _repository.GetLowStockAlertsAsync();
        }

        public async Task<List<GameWishlistStats>> GetTopWishlistGameAsync()
        {
            return await _repository.GetTopWishlistGameAsync();
        }

        public async Task<List<CommissionRequest>> GetTopCommissionAsync()
        {
            return await _repository.GetTopCommissionAsync();
        }
    }
}
