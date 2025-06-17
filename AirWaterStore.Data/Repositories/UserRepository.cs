using AirWaterStore.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirWaterStore.Data.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AirWaterStoreContext _context;
        public UserRepository(AirWaterStoreContext context)
        {
            _context = context;
        }

        public async Task<List<User>> GetAllAsync(int userId, int pageNumber = 1, int pageSize = 10)
        {
            return await _context.Users
                .Where(u => u.UserId != userId)
                .OrderBy(m => m.UserId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<User> GetByIdAsync(int userId)
        {
            return await _context.Users
                .FirstOrDefaultAsync(m => m.UserId == userId);
        }

        public async Task<int> GetTotalCountAsync()
        {
            return await _context.Users.CountAsync();
        }

        public async Task<User> LoginAsync(string email, string password)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email && u.Password == password);
        }

        public async Task AddAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
    }
}
