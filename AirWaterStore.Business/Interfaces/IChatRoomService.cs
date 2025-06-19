using AirWaterStore.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirWaterStore.Business.Interfaces
{
    public interface IChatRoomService
    {
        Task<ChatRoom> GetOrCreateChatRoomAsync(int customerId);
        Task<ChatRoom?> GetChatRoomByIdAsync(int chatRoomId);
        Task<List<ChatRoom>> GetChatRoomsByUserIdAsync(int userId);
        Task AssignStaffToChatRoomAsync(int chatRoomId, int staffId);
    }
}
