using AirWaterStore.Business.Interfaces;
using AirWaterStore.Data.Models;
using AirWaterStore.Web.Helper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AirWaterStore.Web.Pages.Admin.Chat
{
    public class IndexModel : PageModel
    {
        private readonly IChatRoomService _chatRoomService;
        private readonly IUserService _userService;

        public IndexModel(IChatRoomService chatRoomService, IUserService userService)
        {
            _chatRoomService = chatRoomService;
            _userService = userService;
        }

        public List<ChatRoom> ChatRooms { get; set; } = new List<ChatRoom>();
        public Dictionary<int, string> UserNames { get; set; } = new Dictionary<int, string>();
        public int? SelectedRoomId { get; set; }
        // public int CurrentUserId => HttpContext.Session.GetInt32(SessionParams.UserId) ?? 0;

        public async Task<IActionResult> OnGetAsync(int? selectedRoom = null)
        {
            // Check if user is staff
            // if (HttpContext.Session.GetInt32(SessionParams.UserRole) != 2)
            if (!this.IsStaff())
            {
                return RedirectToPage("/Login");
            }

            SelectedRoomId = selectedRoom;

            // Get all chat rooms (assigned to this staff or unassigned)
            ChatRooms = await _chatRoomService.GetChatRoomsByUserIdAsync(this.GetCurrentUserId());

            // Load usernames
            var userIds = new HashSet<int>();
            foreach (var room in ChatRooms)
            {
                userIds.Add(room.CustomerId);
                if (room.StaffId.HasValue)
                    userIds.Add(room.StaffId.Value);
            }

            foreach (var userId in userIds)
            {
                var user = await _userService.GetByIdAsync(userId);
                UserNames[userId] = user?.Username ?? "Unknown User";
            }

            return Page();
        }

        public string GetCustomerName(int userId)
        {
            return UserNames.TryGetValue(userId, out var name) ? name : "Unknown Customer";
        }

        public string GetStaffName(int userId)
        {
            return UserNames.TryGetValue(userId, out var name) ? name : "Unknown Staff";
        }
    }
}