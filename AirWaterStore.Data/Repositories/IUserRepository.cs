using AirWaterStore.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirWaterStore.Data.Repositories
{
    public interface IUserRepository
    {
        Task<List<User>> GetAllAsync(int userId, int pageNumber = 1, int pageSize = 10);
        Task<User> GetByIdAsync(int userId);
        Task<int> GetTotalCountAsync();
        Task<User> LoginAsync(string email, string password);
        Task AddAsync(User user);
        Task UpdateAsync(User user);
    }
}
