using AirWaterStore.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirWaterStore.Data.Repositories
{
    public class MessageRepository : IMessageRepository
    {
        private readonly AirWaterStoreContext _context;

        public MessageRepository(AirWaterStoreContext context)
        {
            _context = context;
        }

        public async Task<List<Message>> GetMessagesByChatRoomIdAsync(int chatRoomId)
        {
            return await _context.Messages
                .Where(m => m.ChatRoomId == chatRoomId)
                .OrderBy(m => m.SentAt)
                .ToListAsync();
        }

        public async Task AddMessageAsync(Message message)
        {
            _context.Messages.Add(message);
            await _context.SaveChangesAsync();
        }
    }
}
