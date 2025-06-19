using AirWaterStore.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirWaterStore.Data.Repositories
{
    public interface IChatRoomRepository
    {
        Task<ChatRoom> GetOrCreateChatRoomAsync(int customerId);
        Task<ChatRoom?> GetChatRoomByIdAsync(int chatRoomId);
        Task<List<ChatRoom>> GetChatRoomsByUserIdAsync(int userId);
        Task AssignStaffToChatRoomAsync(int chatRoomId, int staffId);
    }
}
