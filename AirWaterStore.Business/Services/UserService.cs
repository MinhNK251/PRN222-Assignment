using AirWaterStore.Business.Interfaces;
using AirWaterStore.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirWaterStore.Data.Repositories
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _repository;
        public UserService(IUserRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<User>> GetAllAsync(int userId, int pageNumber = 1, int pageSize = 10)
        {
            return await _repository.GetAllAsync(userId, pageNumber, pageSize);
        }

        public async Task<User> GetByIdAsync(int userId)
        {
            return await _repository.GetByIdAsync(userId);
        }

        public async Task<int> GetTotalCountAsync()
        {
            return await _repository.GetTotalCountAsync();
        }

        public async Task<User> LoginAsync(string email, string password)
        {
            return await _repository.LoginAsync(email, password);
        }
        public async Task AddAsync(User user)
        {
            await _repository.AddAsync(user);
        }        

        public async Task UpdateAsync(User user)
        {
            await _repository.UpdateAsync(user);
        }
    }
}
