using AirWaterStore.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirWaterStore.Data.Repositories
{
    public class WishlistRepository : IWishlistRepository
    {
        private readonly AirWaterStoreContext _context;
        public WishlistRepository(AirWaterStoreContext context)
        {
            _context = context;
        }
        public async Task<List<Game>> GetWishlistGamesForUserAsync(int userId, int pageNumber = 1, int pageSize = 10)
        {
            return await _context.Wishlists
                .Where(w => w.UserId == userId)
                .OrderByDescending(w => w.WishlistId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(w => w.Game)
                .ToListAsync();
        }

        public async Task<bool> HasUserWishlistedAsync(int wishlistId, int userId)
        {
            var wishlist = await _context.Wishlists
                .FirstOrDefaultAsync(w => w.WishlistId == wishlistId && w.UserId == userId);
            return wishlist != null;
        }

        public async Task<int> GetTotalCountByGameIdAsync(int gameId)
        {
            return await _context.Wishlists
                .Where(w => w.GameId == gameId)
                .CountAsync();
        }

        public async Task<int> GetTotalCountByUserIdAsync(int userId)
        {
            return await _context.Wishlists
                .Where(w => w.UserId == userId)
                .CountAsync();
        }
        public async Task AddAsync(Wishlist wishlist)
        {
            await _context.Wishlists.AddAsync(wishlist);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int wishlistId)
        {
            var wishlist = await _context.Wishlists.FindAsync(wishlistId);
            if (wishlist != null)
            {
                _context.Wishlists.Remove(wishlist);
                await _context.SaveChangesAsync();
            }
        }

    }
}
