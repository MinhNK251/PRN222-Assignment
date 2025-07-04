using AirWaterStore.Data.Models;

namespace AirWaterStore.Data.Repositories
{
    public interface IOrderDetailRepository
    {
        Task<List<OrderDetail>> GetAllByOrderIdAsync(int orderId);
        Task AddAsync(OrderDetail orderDetail);
    }
}
