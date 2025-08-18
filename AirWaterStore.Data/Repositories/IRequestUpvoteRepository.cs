using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirWaterStore.Data.Repositories
{
    public interface IRequestUpvoteRepository
    {
        Task<bool> UpvoteRequestAsync(int commissionRequestId, int userId);
        Task<bool> HasUserUpvotedAsync(int commissionRequestId, int userId);
        Task<int> GetUpvoteCountAsync(int commissionRequestId);
    }
}
