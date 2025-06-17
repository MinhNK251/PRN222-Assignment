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
    public class GameService : IGameService
    {
        private readonly IGameRepository _repository;
        public GameService(IGameRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<Game>> GetAllAsync(int pageNumber = 1, int pageSize = 10)
        {
            return await _repository.GetAllAsync(pageNumber, pageSize);
        }

        public async Task<Game> GetByIdAsync(int gameId)
        {
            return await _repository.GetByIdAsync(gameId);
        }

        public async Task<int> GetTotalCountAsync()
        {
            return await _repository.GetTotalCountAsync();
        }

        public async Task AddAsync(Game game)
        {
            await _repository.AddAsync(game);
        }

        public async Task UpdateAsync(Game game)
        {
            await _repository.UpdateAsync(game);
        }

        public async Task DeleteAsync(int gameId)
        {
            await _repository.DeleteAsync(gameId);
        }
    }
}
