using AirWaterStore.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirWaterStore.Business.Interfaces
{
    public interface IMessageService
    {
        Task<List<Message>> GetMessagesByChatRoomIdAsync(int chatRoomId);
        Task AddMessageAsync(Message message);
    }
}
