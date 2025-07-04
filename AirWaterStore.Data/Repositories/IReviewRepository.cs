using AirWaterStore.Data.Models;

namespace AirWaterStore.Data.Repositories
{
    public interface IReviewRepository
    {
        Task<List<Review>> GetAllByGameIdAsync(int gameId);
        Task<Review?> GetByIdAsync(int reviewId);
        Task AddAsync(Review review);
        Task UpdateAsync(Review review);
        Task DeleteAsync(int reviewId);
    }
}
