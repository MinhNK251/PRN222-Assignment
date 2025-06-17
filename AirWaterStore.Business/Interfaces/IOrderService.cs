using AirWaterStore.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirWaterStore.Business.Interfaces
{
    public interface IOrderService
    {
        Task<List<Order>> GetAllAsync(int pageNumber = 1, int pageSize = 10);
        Task<List<Order>> GetAllByUserIdAsync(int userId, int pageNumber = 1, int pageSize = 10);
        Task<Order> GetByIdAsync(int orderId);
        Task<int> GetTotalCountAsync();
        Task<int> GetTotalCountByUserIdAsync(int userId);
        Task AddAsync(Order order);
        Task UpdateAsync(Order order);
    }
}
