using AirWaterStore.Business.Interfaces;
using AirWaterStore.Data.Models;
using AirWaterStore.Data.Repositories;

namespace AirWaterStore.Business.Services
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _repository;
        public MessageService(IMessageRepository repository)
        {
            _repository = repository;
        }

        public async Task AddMessageAsync(Message message)
        {
            await _repository.AddMessageAsync(message);
        }

        public async Task<List<Message>> GetMessagesByChatRoomIdAsync(int chatRoomId)
        {
            return await _repository.GetMessagesByChatRoomIdAsync(chatRoomId);
        }
    }
}
