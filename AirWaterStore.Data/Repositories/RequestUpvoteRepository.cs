using AirWaterStore.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirWaterStore.Data.Repositories
{
    public class RequestUpvoteRepository : IRequestUpvoteRepository
    {
        private readonly AirWaterStoreContext _context;
        public RequestUpvoteRepository(AirWaterStoreContext context)
        {
            _context = context;
        }

        public async Task<bool> UpvoteRequestAsync(int commissionRequestId, int userId)
        {
            var existingUpvote = await _context.CommissionRequestUpvotes
                .FirstOrDefaultAsync(u => u.CommissionRequestId == commissionRequestId && u.UserId == userId);
            var commissionRequest = await _context.CommissionRequests
                .FirstOrDefaultAsync(c => c.CommissionRequestId == commissionRequestId);
            if (existingUpvote != null)
            {
                _context.CommissionRequestUpvotes.Remove(existingUpvote);
                await _context.SaveChangesAsync();
                commissionRequest.Upvotes = commissionRequest.Upvotes - 1;
                _context.CommissionRequests.Update(commissionRequest);
                await _context.SaveChangesAsync();
                return false; // removed
            }
            else
            {
                var newUpvote = new CommissionRequestUpvote
                {
                    CommissionRequestId = commissionRequestId,
                    UserId = userId,
                    UpvotedAt = DateTime.UtcNow
                };
                _context.CommissionRequestUpvotes.Add(newUpvote);
                await _context.SaveChangesAsync();
                commissionRequest.Upvotes = commissionRequest.Upvotes + 1;
                _context.CommissionRequests.Update(commissionRequest);
                await _context.SaveChangesAsync();
                return true; // added
            }
        }

        public async Task<bool> HasUserUpvotedAsync(int commissionRequestId, int userId)
        {
            var upvote = await _context.CommissionRequestUpvotes
                .FirstOrDefaultAsync(u => u.CommissionRequestId == commissionRequestId && u.UserId == userId);
            return upvote != null;
        }

        public async Task<int> GetUpvoteCountAsync(int commissionRequestId)
        {
            return await _context.CommissionRequestUpvotes
                .Where(u => u.CommissionRequestId == commissionRequestId)
                .CountAsync();
        }
    }
}
