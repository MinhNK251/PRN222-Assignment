using AirWaterStore.Data.Models;

namespace AirWaterStore.Business.Interfaces
{
    public interface IGameService
    {
        Task<List<Game>> GetAllAsync(int pageNumber = 1, int pageSize = 10);
        Task<Game?> GetByIdAsync(int gameId);
        Task<int> GetTotalCountAsync();
        Task AddAsync(Game game);
        Task UpdateAsync(Game game);
        Task DeleteAsync(int gameId);
    }
}
