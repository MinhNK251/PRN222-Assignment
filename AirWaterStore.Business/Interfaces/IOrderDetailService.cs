using AirWaterStore.Data.Models;

namespace AirWaterStore.Business.Interfaces
{
    public interface IOrderDetailService
    {
        Task<List<OrderDetail>> GetAllByOrderIdAsync(int orderId);
        Task AddAsync(OrderDetail orderDetail);
    }
}