using AirWaterStore.Data.Models;

namespace AirWaterStore.Business.Interfaces
{
    public interface IReviewService
    {
        Task<List<Review>> GetAllByGameIdAsync(int gameId);
        Task<Review?> GetByIdAsync(int reviewId);
        Task AddAsync(Review review);
        Task UpdateAsync(Review review);
        Task DeleteAsync(int reviewId);
    }
}
