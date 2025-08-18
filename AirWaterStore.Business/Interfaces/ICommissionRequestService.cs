using AirWaterStore.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirWaterStore.Business.Interfaces
{
    public interface ICommissionRequestService
    {
        Task<List<CommissionRequest>> GetAllAsync(int pageNumber = 1, int pageSize = 10);
        Task<CommissionRequest> GetByIdAsync(int commissionRequestId);
        Task<int> GetTotalCountAsync();
        Task AddAsync(CommissionRequest commissionRequest);
        Task UpdateAsync(CommissionRequest commissionRequest);
        Task DeleteAsync(int commissionRequestId);
    }
}
