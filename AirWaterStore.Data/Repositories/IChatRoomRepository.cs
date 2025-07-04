using AirWaterStore.Data.Models;

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
