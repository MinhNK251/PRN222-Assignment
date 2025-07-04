using AirWaterStore.Business.Interfaces;
using AirWaterStore.Data.Models;
using AirWaterStore.Web.Helper;
using Microsoft.AspNetCore.SignalR;

namespace AirWaterStore.Web.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IMessageService _messageService;
        private readonly IChatRoomService _chatRoomService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ChatHub(IMessageService messageService, IChatRoomService chatRoomService, IHttpContextAccessor httpContextAccessor)
        {
            _messageService = messageService;
            _chatRoomService = chatRoomService;
            _httpContextAccessor = httpContextAccessor;
        }

        public override async Task OnConnectedAsync()
        {
            var userId = GetUserId();
            if (userId.HasValue)
            {
                // Add user to their personal group
                await Groups.AddToGroupAsync(Context.ConnectionId, $"user-{userId}");
            }

            await base.OnConnectedAsync();
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var userId = GetUserId();
            if (userId.HasValue)
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"user-{userId}");
            }

            await base.OnDisconnectedAsync(exception);
        }

        public async Task JoinChatRoom(int chatRoomId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"chatroom-{chatRoomId}");
        }

        public async Task LeaveChatRoom(int chatRoomId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"chatroom-{chatRoomId}");
        }

        public async Task SendMessage(int chatRoomId, string messageContent)
        {
            var userId = GetUserId();
            if (!userId.HasValue || string.IsNullOrWhiteSpace(messageContent))
                return;

            // Verify user has access to this chat room
            var chatRoom = await _chatRoomService.GetChatRoomByIdAsync(chatRoomId);
            if (chatRoom == null)
                return;

            var userRole = GetUserRole();

            // Check authorization
            if (userRole == 1 && chatRoom.CustomerId != userId.Value)
                return;

            if (userRole == 2 && chatRoom.StaffId != null && chatRoom.StaffId != userId.Value)
            {
                // If staff is not assigned, assign them
                if (chatRoom.StaffId == null)
                {
                    await _chatRoomService.AssignStaffToChatRoomAsync(chatRoomId, userId.Value);
                }
            }

            // Save message
            var message = new Message
            {
                ChatRoomId = chatRoomId,
                UserId = userId.Value,
                Content = messageContent.Trim(),
                SentAt = DateTime.Now
            };

            await _messageService.AddMessageAsync(message);

            // Send message to all users in the chat room
            await Clients.Group($"chatroom-{chatRoomId}").SendAsync("ReceiveMessage", new
            {
                messageId = message.MessageId,
                userId = message.UserId,
                content = message.Content,
                sentAt = message.SentAt?.ToString("HH:mm"),
                username = GetUsername()
            });

            // Notify staff if this is from a customer
            if (userRole == 1 && chatRoom.StaffId.HasValue)
            {
                await Clients.Group($"user-{chatRoom.StaffId}").SendAsync("NewCustomerMessage", new
                {
                    chatRoomId = chatRoomId,
                    customerName = GetUsername(),
                    message = messageContent
                });
            }
        }

        public async Task NotifyTyping(int chatRoomId, bool isTyping)
        {
            var userId = GetUserId();
            var username = GetUsername();

            if (userId.HasValue)
            {
                await Clients.OthersInGroup($"chatroom-{chatRoomId}").SendAsync("UserTyping", new
                {
                    userId = userId,
                    username = username,
                    isTyping = isTyping
                });
            }
        }

        private int? GetUserId()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            return httpContext?.Session.GetInt32(SessionParams.UserId);
        }

        private int? GetUserRole()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            return httpContext?.Session.GetInt32(SessionParams.UserRole);
        }

        private string GetUsername()
        {
            var httpContext = _httpContextAccessor.HttpContext;
            return httpContext?.Session.GetString(SessionParams.UserName) ?? "Unknown User";
        }
    }
}