using AirWaterStore.Business.Interfaces;
using AirWaterStore.Data.Models;
using AirWaterStore.Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirWaterStore.Business.Services
{
    public class ChatRoomService : IChatRoomService
    {
        private readonly IChatRoomRepository _repository;
        public ChatRoomService(IChatRoomRepository repository)
        {
            _repository = repository;
        }

        public async Task AssignStaffToChatRoomAsync(int chatRoomId, int staffId)
        {
            await _repository.AssignStaffToChatRoomAsync(chatRoomId, staffId);
        }

        public async Task<ChatRoom?> GetChatRoomByIdAsync(int chatRoomId)
        {
            return await _repository.GetChatRoomByIdAsync(chatRoomId);
        }

        public async Task<List<ChatRoom>> GetChatRoomsByUserIdAsync(int userId)
        {
            return await _repository.GetChatRoomsByUserIdAsync(userId);
        }

        public async Task<ChatRoom> GetOrCreateChatRoomAsync(int customerId)
        {
            return await _repository.GetOrCreateChatRoomAsync(customerId);
        }
    }
}
