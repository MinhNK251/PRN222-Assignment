using AirWaterStore.Business.Interfaces;
using AirWaterStore.Data.Models;
using AirWaterStore.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirWaterStore.Business.Services
{
    public class WishlistService : IWishlistService
    {
        private readonly IWishlistRepository _repository;
        
        public WishlistService(IWishlistRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<Game>> GetWishlistGamesForUserAsync(int userId, int pageNumber = 1, int pageSize = 10)
        {
            return await _repository.GetWishlistGamesForUserAsync(userId, pageNumber, pageSize);
        }

        public async Task<bool> HasUserWishlistedAsync(int wishlistId, int userId)
        {
            return await _repository.HasUserWishlistedAsync(wishlistId, userId);
        }

        public async Task<int> GetTotalCountByUserIdAsync(int userId)
        {
            return await _repository.GetTotalCountByUserIdAsync(userId);
        }

        public async Task<int> GetTotalCountByGameIdAsync(int gameId)
        {
            return await _repository.GetTotalCountByGameIdAsync(gameId);
        }

        public async Task<Wishlist?> GetWishlistItemAsync(int userId, int gameId)
        {
            return await _repository.GetWishlistItemAsync(userId, gameId);
        }

        public async Task AddAsync(Wishlist wishlist)
        {
            await _repository.AddAsync(wishlist);
        }

        public async Task DeleteAsync(int wishlistId)
        {
            await _repository.DeleteAsync(wishlistId);
        }

        public async Task DeleteByUserAndGameAsync(int userId, int gameId)
        {
            await _repository.DeleteByUserAndGameAsync(userId, gameId);
        }

        public async Task ClearUserWishlistAsync(int userId)
        {
            await _repository.ClearUserWishlistAsync(userId);
        }
    }
}
