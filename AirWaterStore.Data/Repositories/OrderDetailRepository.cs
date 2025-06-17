using AirWaterStore.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirWaterStore.Data.Repositories
{
    public class OrderDetailRepository : IOrderDetailRepository
    {
        private readonly AirWaterStoreContext _context;
        public OrderDetailRepository(AirWaterStoreContext context)
        {
            _context = context;
        }

        public async Task<List<OrderDetail>> GetAllByOrderIdAsync(int orderId)
        {
            return await _context.OrderDetails
                .Where(x => x.OrderId == orderId)
                .Include(x => x.Game)
                .OrderBy(m => m.OrderDetailId)
                .ToListAsync();
        }

        public async Task AddAsync(OrderDetail orderDetail)
        {
            _context.OrderDetails.Add(orderDetail);
            await _context.SaveChangesAsync();
        }
    }
}
