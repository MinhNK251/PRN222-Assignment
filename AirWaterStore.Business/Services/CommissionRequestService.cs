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
    public class CommissionRequestService : ICommissionRequestService
    {
        private readonly ICommissionRequestRepository _repository;
        public CommissionRequestService(ICommissionRequestRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<CommissionRequest>> GetAllAsync(int pageNumber = 1, int pageSize = 10)
        {
            return await _repository.GetAllAsync(pageNumber, pageSize);
        }

        public async Task<CommissionRequest> GetByIdAsync(int commissionRequestId)
        {
            return await _repository.GetByIdAsync(commissionRequestId);
        }

        public async Task<int> GetTotalCountAsync()
        {
            return await _repository.GetTotalCountAsync();
        }

        public async Task AddAsync(CommissionRequest commissionRequest)
        {
            await _repository.AddAsync(commissionRequest);
        }

        public async Task UpdateAsync(CommissionRequest commissionRequest)
        {
            await _repository.UpdateAsync(commissionRequest);
        }

        public async Task DeleteAsync(int commissionRequestId)
        {
            await _repository.DeleteAsync(commissionRequestId);
        }
    }
}
