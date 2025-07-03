using AirWaterStore.Business.Interfaces;
using AirWaterStore.Data.Models;
using AirWaterStore.Web.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AirWaterStore.Web.Pages.Admin.Chat
{
    public class RoomModel : PageModel
    {
        private readonly IChatRoomService _chatRoomService;
        private readonly IMessageService _messageService;
        private readonly IUserService _userService;

        public RoomModel(IChatRoomService chatRoomService, IMessageService messageService, IUserService userService)
        {
            _chatRoomService = chatRoomService;
            _messageService = messageService;
            _userService = userService;
        }

        public ChatRoom ChatRoom { get; set; } = default!;
        public List<Message> Messages { get; set; } = new List<Message>();
        public string CustomerName { get; set; } = string.Empty;
        public Dictionary<int, string> UserNames { get; set; } = new Dictionary<int, string>();
        // public int CurrentUserId => HttpContext.Session.GetInt32(SessionParams.UserId) ?? 0;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            // Check if user is staff
            // if (HttpContext.Session.GetInt32(SessionParams.UserRole) != 2)
            if (!this.IsStaff())
            {
                return RedirectToPage("/Login");
            }

            var chatRoom = await _chatRoomService.GetChatRoomByIdAsync(id);
            if (chatRoom == null)
            {
                return NotFound();
            }

            ChatRoom = chatRoom;

            // Get messages
            Messages = await _messageService.GetMessagesByChatRoomIdAsync(id);

            // Load usernames
            var userIds = Messages.Select(m => m.UserId).Distinct().ToList();
            userIds.Add(ChatRoom.CustomerId);
            if (ChatRoom.StaffId.HasValue)
                userIds.Add(ChatRoom.StaffId.Value);

            foreach (var userId in userIds)
            {
                var user = await _userService.GetByIdAsync(userId);
                UserNames[userId] = user?.Username ?? "Unknown User";
            }

            CustomerName = GetUsername(ChatRoom.CustomerId);

            return Page();
        }

        public async Task<IActionResult> OnPostSendMessageAsync(int chatRoomId, string messageContent)
        {
            if (HttpContext.Session.GetInt32(SessionParams.UserRole) != 2 || string.IsNullOrWhiteSpace(messageContent))
            {
                return RedirectToPage();
            }

            var chatRoom = await _chatRoomService.GetChatRoomByIdAsync(chatRoomId);
            if (chatRoom == null)
            {
                return NotFound();
            }

            // If chat is unassigned, assign it to current staff
            if (!chatRoom.StaffId.HasValue)
            {
                await _chatRoomService.AssignStaffToChatRoomAsync(chatRoomId, this.GetCurrentUserId());
            }

            // Send message
            var message = new Message
            {
                ChatRoomId = chatRoomId,
                UserId = this.GetCurrentUserId(),
                Content = messageContent.Trim(),
                SentAt = DateTime.Now
            };

            await _messageService.AddMessageAsync(message);

            return RedirectToPage(new { id = chatRoomId });
        }

        public string GetUsername(int userId)
        {
            return UserNames.TryGetValue(userId, out var username) ? username : "Unknown User";
        }
    }
}