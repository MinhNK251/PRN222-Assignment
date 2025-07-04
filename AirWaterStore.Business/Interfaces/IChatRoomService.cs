using AirWaterStore.Data.Models;

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
