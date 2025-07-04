using AirWaterStore.Data.Models;

namespace AirWaterStore.Data.Repositories
{
    public interface IGameRepository
    {
        Task<List<Game>> GetAllAsync(int pageNumber = 1, int pageSize = 10);
        Task<Game?> GetByIdAsync(int gameId);
        Task<int> GetTotalCountAsync();
        Task AddAsync(Game game);
        Task UpdateAsync(Game game);
        Task DeleteAsync(int gameId);
    }
}
