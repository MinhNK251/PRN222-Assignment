using AirWaterStore.Data.Models;

namespace AirWaterStore.Business.Interfaces
{
    public interface IMessageService
    {
        Task<List<Message>> GetMessagesByChatRoomIdAsync(int chatRoomId);
        Task AddMessageAsync(Message message);
    }
}
