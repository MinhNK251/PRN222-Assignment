using AirWaterStore.Business.Interfaces;
using AirWaterStore.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirWaterStore.Data.Repositories
{
    public class OrderService : IOrderService
    {
        private readonly IOrderRepository _repository;
        public OrderService(IOrderRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<Order>> GetAllAsync(int pageNumber = 1, int pageSize = 10)
        {
            return await _repository.GetAllAsync(pageNumber, pageSize);
        }

        public async Task<List<Order>> GetAllByUserIdAsync(int userId, int pageNumber = 1, int pageSize = 10)
        {
            return await _repository.GetAllByUserIdAsync(userId, pageNumber, pageSize);
        }

        public async Task<Order> GetByIdAsync(int orderId)
        {
            return await _repository.GetByIdAsync(orderId);
        }

        public async Task<int> GetTotalCountAsync()
        {
            return await _repository.GetTotalCountAsync();
        }

        public async Task<int> GetTotalCountByUserIdAsync(int userId)
        {
            return await _repository.GetTotalCountByUserIdAsync(userId);
        }
        public async Task AddAsync(Order order)
        {
            await _repository.AddAsync(order);
        }

        public async Task UpdateAsync(Order order)
        {
            await _repository.UpdateAsync(order);
        }
    }
}
