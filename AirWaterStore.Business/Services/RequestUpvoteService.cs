using AirWaterStore.Business.Interfaces;
using AirWaterStore.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirWaterStore.Business.Services
{
    public class RequestUpvoteService : IRequestUpvoteService
    {
        private readonly IRequestUpvoteRepository _repository;
        public RequestUpvoteService(IRequestUpvoteRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> UpvoteRequestAsync(int commissionRequestId, int userId)
        {
            return await _repository.UpvoteRequestAsync(commissionRequestId, userId);
        }

        public async Task<bool> HasUserUpvotedAsync(int commissionRequestId, int userId)
        {
            return await _repository.HasUserUpvotedAsync(commissionRequestId, userId);
        }

        public async Task<int> GetUpvoteCountAsync(int commissionRequestId)
        {
            return await _repository.GetUpvoteCountAsync(commissionRequestId);
        }
    }
}
