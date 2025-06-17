using AirWaterStore.Business.Interfaces;
using AirWaterStore.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirWaterStore.Data.Repositories
{
    public class OrderDetailService : IOrderDetailService
    {
        private readonly IOrderDetailRepository _repository;
        public OrderDetailService(IOrderDetailRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<OrderDetail>> GetAllByOrderIdAsync(int orderId)
        {
            return await _repository.GetAllByOrderIdAsync(orderId);
        }

        public async Task AddAsync(OrderDetail orderDetail)
        {
            await _repository.AddAsync(orderDetail);
        }
    }
}
