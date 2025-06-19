using AirWaterStore.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirWaterStore.Business.Interfaces
{
    public interface IReviewService
    {
        Task<List<Review>> GetAllByGameIdAsync(int gameId);
        Task<Review> GetByIdAsync(int reviewId);
        Task AddAsync(Review review);
        Task UpdateAsync(Review review);
        Task DeleteAsync(int reviewId);
    }
}
