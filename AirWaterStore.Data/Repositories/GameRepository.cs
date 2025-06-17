using AirWaterStore.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirWaterStore.Data.Repositories
{
    public class GameRepository : IGameRepository
    {
        private readonly AirWaterStoreContext _context;
        public GameRepository(AirWaterStoreContext context)
        {
            _context = context;
        }

        public async Task<List<Game>> GetAllAsync(int pageNumber = 1, int pageSize = 10)
        {
            return await _context.Games
                .OrderBy(m => m.ReleaseDate)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<Game> GetByIdAsync(int gameId)
        {
            return await _context.Games
                .Include(m => m.Reviews)
                .FirstOrDefaultAsync(m => m.GameId == gameId);
        }

        public async Task<int> GetTotalCountAsync()
        {
            return await _context.Games.CountAsync();
        }

        public async Task AddAsync(Game game)
        {
            _context.Games.Add(game);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Game game)
        {
            _context.Games.Update(game);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int gameId)
        {
            var game = await _context.Games.FindAsync(gameId);
            if (game != null)
            {
                _context.Games.Remove(game);
                await _context.SaveChangesAsync();
            }
        }                     
    }
}
