using AirWaterStore.Data.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirWaterStore.Data.Repositories
{
    public class ChatRoomRepository : IChatRoomRepository
    {
        private readonly AirWaterStoreContext _context;

        public ChatRoomRepository(AirWaterStoreContext context)
        {
            _context = context;
        }

        public async Task<ChatRoom> GetOrCreateChatRoomAsync(int customerId)
        {
            var existing = await _context.ChatRooms
                .Include(c => c.Messages)
                .FirstOrDefaultAsync(c => c.CustomerId == customerId);

            if (existing != null)
                return existing;

            var newRoom = new ChatRoom
            {
                CustomerId = customerId,
                StaffId = null
            };

            _context.ChatRooms.Add(newRoom);
            await _context.SaveChangesAsync();

            return newRoom;
        }

        public async Task<ChatRoom?> GetChatRoomByIdAsync(int chatRoomId)
        {
            return await _context.ChatRooms
                .Include(c => c.Messages)
                .FirstOrDefaultAsync(c => c.ChatRoomId == chatRoomId);
        }

        public async Task<List<ChatRoom>> GetChatRoomsByUserIdAsync(int userId)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user == null) return [];

            if (user.Role == 1) // Customer
            {
                return await _context.ChatRooms
                    .Where(c => c.CustomerId == userId)
                    .ToListAsync();
            }
            else if (user.Role == 2) // Staff
            {
                return await _context.ChatRooms
                    .Where(c => (c.StaffId == null || c.StaffId == userId) && _context.Messages.Any(m => m.ChatRoomId == c.ChatRoomId))
                    .ToListAsync();
            }

            return new();
        }

        public async Task AssignStaffToChatRoomAsync(int chatRoomId, int staffId)
        {
            var chatRoom = await _context.ChatRooms.FindAsync(chatRoomId);
            if (chatRoom == null || chatRoom.StaffId != null) return;

            chatRoom.StaffId = staffId;
            _context.ChatRooms.Update(chatRoom);
            await _context.SaveChangesAsync();
        }
    }
}
