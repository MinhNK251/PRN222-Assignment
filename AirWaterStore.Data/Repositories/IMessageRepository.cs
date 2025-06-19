using AirWaterStore.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirWaterStore.Data.Repositories
{
    public interface IMessageRepository
    {
        Task<List<Message>> GetMessagesByChatRoomIdAsync(int chatRoomId);
        Task AddMessageAsync(Message message);
    }
}
