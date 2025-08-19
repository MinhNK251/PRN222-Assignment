using AirWaterStore.Data.Models;
using Microsoft.EntityFrameworkCore;

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
                .Include(w => w.Game)
                .OrderByDescending(w => w.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(w => w.Game)
                .ToListAsync();
        }

        // ✅ FIXED METHOD:
        public async Task<bool> HasUserWishlistedAsync(int gameId, int userId)
        {
            return await _context.Wishlists
                .AnyAsync(w => w.GameId == gameId && w.UserId == userId);
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

        // ✅ FIXED METHOD:
        public async Task<Wishlist?> GetWishlistItemAsync(int userId, int gameId)
        {
            return await _context.Wishlists
                .FirstOrDefaultAsync(w => w.UserId == userId && w.GameId == gameId);
        }

        public async Task AddAsync(Wishlist wishlist)
        {
            // Check if already exists
            var existing = await GetWishlistItemAsync(wishlist.UserId, wishlist.GameId);
            if (existing != null)
            {
                throw new InvalidOperationException("Game is already in wishlist");
            }

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

        // ✅ FIXED METHOD:
        public async Task DeleteByUserAndGameAsync(int userId, int gameId)
        {
            var wishlistItem = await GetWishlistItemAsync(userId, gameId);
            if (wishlistItem != null)
            {
                _context.Wishlists.Remove(wishlistItem);
                await _context.SaveChangesAsync();
            }
        }

        public async Task ClearUserWishlistAsync(int userId)
        {
            var userWishlists = await _context.Wishlists
                .Where(w => w.UserId == userId)
                .ToListAsync();
            
            _context.Wishlists.RemoveRange(userWishlists);
            await _context.SaveChangesAsync();
        }
    }
}