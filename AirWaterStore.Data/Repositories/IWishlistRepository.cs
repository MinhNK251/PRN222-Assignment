using AirWaterStore.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirWaterStore.Data.Repositories
{
    public interface IWishlistRepository
    {
        Task<List<Game>> GetWishlistGamesForUserAsync(int userId, int pageNumber = 1, int pageSize = 10);
        Task<bool> HasUserWishlistedAsync(int wishlistId, int userId);
        Task<int> GetTotalCountByUserIdAsync(int userId);
        Task<int> GetTotalCountByGameIdAsync(int gameId);
        Task AddAsync(Wishlist wishlist);
        Task DeleteAsync(int wishlistId);
    }
}
