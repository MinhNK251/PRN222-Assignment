using AirWaterStore.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirWaterStore.Business.Interfaces
{
    public interface IOrderDetailService
    {
        Task<List<OrderDetail>> GetAllByOrderIdAsync(int orderId);
        Task AddAsync(OrderDetail orderDetail);
    }
}