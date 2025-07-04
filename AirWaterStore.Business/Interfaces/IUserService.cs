using AirWaterStore.Data.Models;

namespace AirWaterStore.Business.Interfaces
{
    public interface IUserService
    {
        Task<List<User>> GetAllAsync(int userId, int pageNumber = 1, int pageSize = 10);
        Task<User?> GetByIdAsync(int userId);
        Task<int> GetTotalCountAsync();
        Task<User?> LoginAsync(string email, string password);
        Task AddAsync(User user);
        Task UpdateAsync(User user);
    }
}
