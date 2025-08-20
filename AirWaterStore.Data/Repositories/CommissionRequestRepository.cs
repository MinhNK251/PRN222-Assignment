using AirWaterStore.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirWaterStore.Data.Repositories
{
    public class CommissionRequestRepository : ICommissionRequestRepository
    {
        private readonly AirWaterStoreContext _context;
        public CommissionRequestRepository(AirWaterStoreContext context)
        {
            _context = context;
        }

        public async Task<List<CommissionRequest>> GetAllAsync(int pageNumber = 1, int pageSize = 10)
        {
            return await _context.CommissionRequests
               .Skip((pageNumber - 1) * pageSize)
               .Take(pageSize)
               .ToListAsync();
        }

        public async Task<CommissionRequest> GetByIdAsync(int commissionRequestId)
        {
            return await _context.CommissionRequests
                .FirstOrDefaultAsync(c => c.CommissionRequestId == commissionRequestId);  // Find by ID
        }

        public async Task<int> GetTotalCountAsync()
        {
            return await _context.CommissionRequests
                .CountAsync();
        }

        public async Task AddAsync(CommissionRequest commissionRequest)
        {
            await _context.CommissionRequests.AddAsync(commissionRequest);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(CommissionRequest commissionRequest)
        {
            _context.CommissionRequests.Update(commissionRequest);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int commissionRequestId)
        {
            // Remove all upvotes related to this request first
            var upvotes = await _context.CommissionRequestUpvotes
                .Where(u => u.CommissionRequestId == commissionRequestId)
                .ToListAsync();

            if (upvotes.Any())
            {
                _context.CommissionRequestUpvotes.RemoveRange(upvotes);
            }

            // Now remove the commission request
            var commissionRequest = await _context.CommissionRequests.FindAsync(commissionRequestId);
            if (commissionRequest != null)
            {
                _context.CommissionRequests.Remove(commissionRequest);
            }

            await _context.SaveChangesAsync();
        }

    }
}
