using AirWaterStore.Business.Interfaces;
using AirWaterStore.Data.Models;
using AirWaterStore.Web.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AirWaterStore.Web.Pages.Chat
{
    public class IndexModel : PageModel
    {
        private readonly IChatRoomService _chatRoomService;
        private readonly IMessageService _messageService;
        private readonly IUserService _userService;

        public IndexModel(IChatRoomService chatRoomService, IMessageService messageService, IUserService userService)
        {
            _chatRoomService = chatRoomService;
            _messageService = messageService;
            _userService = userService;
        }

        public ChatRoom ChatRoom { get; set; } = default!;
        public List<Message> Messages { get; set; } = new List<Message>();
        public Dictionary<int, string> UserNames { get; set; } = new Dictionary<int, string>();
        // public int CurrentUserId => HttpContext.Session.GetInt32(SessionParams.UserId) ?? 0;

        public async Task<IActionResult> OnGetAsync()
        {
            // var userId = HttpContext.Session.GetInt32(SessionParams.UserId);
            // var userRole = HttpContext.Session.GetInt32(SessionParams.UserRole);
            var userId = this.GetCurrentUserId();
            var userRole = this.GetCurrentUserRole();

            if (!this.IsAuthenticated())
            {
                return RedirectToPage("/Login");
            }

            // Only customers can access this page
            if (userRole != 1)
            {
                return RedirectToPage("/Admin/Chat/Index");
            }

            // Get or create chat room for customer
            ChatRoom = await _chatRoomService.GetOrCreateChatRoomAsync(userId);

            // Get messages
            Messages = await _messageService.GetMessagesByChatRoomIdAsync(ChatRoom.ChatRoomId);

            // Load usernames
            var userIds = Messages.Select(m => m.UserId).Distinct().ToList();
            userIds.Add(userId);

            foreach (var id in userIds)
            {
                var user = await _userService.GetByIdAsync(id);
                UserNames[id] = user?.Username ?? "Unknown User";
            }

            return Page();
        }

        public async Task<IActionResult> OnPostSendMessageAsync(string messageContent)
        {
            var userId = HttpContext.Session.GetInt32(SessionParams.UserId);
            if (!userId.HasValue || string.IsNullOrWhiteSpace(messageContent))
            {
                return RedirectToPage();
            }

            var chatRoom = await _chatRoomService.GetOrCreateChatRoomAsync(userId.Value);

            var message = new Message
            {
                ChatRoomId = chatRoom.ChatRoomId,
                UserId = userId.Value,
                Content = messageContent.Trim(),
                SentAt = DateTime.Now
            };

            await _messageService.AddMessageAsync(message);

            return RedirectToPage();
        }

        public string GetUsername(int userId)
        {
            return UserNames.TryGetValue(userId, out var username) ? username : "Unknown User";
        }
    }
}