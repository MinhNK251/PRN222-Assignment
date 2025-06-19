using AirWaterStore.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirWaterStore.Data.Repositories
{
    public class ReviewRepository : IReviewRepository
    {
        private readonly AirWaterStoreContext _context;
        public ReviewRepository(AirWaterStoreContext context)
        {
            _context = context;
        }

        public async Task<List<Review>> GetAllByGameIdAsync(int gameId)
        {
            return await _context.Reviews
                .Where(r => r.GameId == gameId)
                .OrderBy(m => m.ReviewDate)
                .ToListAsync();
        }

        public async Task<Review> GetByIdAsync(int reviewId)
        {
            return await _context.Reviews
                .FirstOrDefaultAsync(m => m.ReviewId == reviewId);
        }
        public async Task AddAsync(Review review)
        {
            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Review review)
        {
            _context.Reviews.Update(review);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int reviewId)
        {
            var review = await _context.Reviews.FindAsync(reviewId);
            if (review != null)
            {
                _context.Reviews.Remove(review);
                await _context.SaveChangesAsync();
            }
        }
    }
}
