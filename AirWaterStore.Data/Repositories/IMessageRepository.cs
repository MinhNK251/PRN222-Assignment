using AirWaterStore.Data.Models;

namespace AirWaterStore.Data.Repositories
{
    public interface IMessageRepository
    {
        Task<List<Message>> GetMessagesByChatRoomIdAsync(int chatRoomId);
        Task AddMessageAsync(Message message);
    }
}
