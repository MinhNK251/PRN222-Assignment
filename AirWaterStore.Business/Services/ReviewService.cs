using AirWaterStore.Business.Interfaces;
using AirWaterStore.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirWaterStore.Data.Repositories
{
    public class ReviewService : IReviewService
    {
        private readonly IReviewRepository _repository;
        public ReviewService(IReviewRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<Review>> GetAllByGameIdAsync(int gameId)
        {
            return await _repository.GetAllByGameIdAsync(gameId);
        }

        public async Task<Review> GetByIdAsync(int reviewId)
        {
            return await _repository.GetByIdAsync(reviewId);
        }

        public async Task AddAsync(Review review)
        {
            await _repository.AddAsync(review);
        }

        public async Task UpdateAsync(Review review)
        {
            await _repository.UpdateAsync(review);
        }

        public async Task DeleteAsync(int reviewId)
        {
            await _repository.DeleteAsync(reviewId);
        }
    }
}
