using AirWaterStore.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirWaterStore.Data.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AirWaterStoreContext _context;
        public OrderRepository(AirWaterStoreContext context)
        {
            _context = context;
        }

        public async Task<List<Order>> GetAllAsync(int pageNumber = 1, int pageSize = 10)
        {
            return await _context.Orders
                .Include(m => m.OrderDetails)
                .OrderByDescending(m => m.OrderDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<List<Order>> GetAllByUserIdAsync(int userId, int pageNumber = 1, int pageSize = 10)
        {
            return await _context.Orders
                .Where(o => o.UserId == userId)
                .Include(m => m.OrderDetails)
                .OrderByDescending(m => m.OrderDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Order> GetByIdAsync(int orderId)
        {
            return await _context.Orders
                .Include(m => m.OrderDetails)
                .FirstOrDefaultAsync(m => m.OrderId == orderId);
        }

        public async Task<int> GetTotalCountAsync()
        {
            return await _context.Orders.CountAsync();
        }

        public async Task<int> GetTotalCountByUserIdAsync(int userId)
        {
            return await _context.Orders.Where(o => o.UserId == userId).CountAsync();
        }

        public async Task AddAsync(Order order)
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
        }      

        public async Task UpdateAsync(Order order)
        {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
        }
    }
}
