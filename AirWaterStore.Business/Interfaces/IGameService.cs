using AirWaterStore.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirWaterStore.Business.Interfaces
{
    public interface IGameService
    {
        Task<List<Game>> GetAllAsync(int pageNumber = 1, int pageSize = 10);
        Task<Game> GetByIdAsync(int gameId);
        Task<int> GetTotalCountAsync();
        Task AddAsync(Game game);
        Task UpdateAsync(Game game);
        Task DeleteAsync(int gameId);
    }
}
