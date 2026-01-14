using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using SmartRideBackend.Data;
using SmartRideBackend.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace SmartRideBackend.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ApplicationDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public ChatHub(ApplicationDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        private int GetUserId()
        {
            var claim = Context.User?.FindFirst(ClaimTypes.NameIdentifier);
            return int.Parse(claim?.Value ?? "0");
        }

        /// <summary>
        /// Join a chat room
        /// </summary>
        public async Task JoinRoom(int roomId)
        {
            try
            {
                var userId = GetUserId();

                // Check if user is a member of this room
                var isMember = await _context.ChatRoomMembers
                    .AnyAsync(m => m.ChatRoomId == roomId && m.UserId == userId && m.IsActive);

                if (isMember)
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, $"room_{roomId}");
                    await Clients.Group($"room_{roomId}")
                        .SendAsync("UserJoined", new { userId, userName = "", roomId });
                }
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("Error", ex.Message);
            }
        }

        /// <summary>
        /// Leave a chat room
        /// </summary>
        public async Task LeaveRoom(int roomId)
        {
            try
            {
                var userId = GetUserId();
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"room_{roomId}");
                await Clients.Group($"room_{roomId}")
                    .SendAsync("UserLeft", new { userId, roomId });
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("Error", ex.Message);
            }
        }

        /// <summary>
        /// Send message to chat room
        /// </summary>
        public async Task SendMessage(int roomId, string content)
        {
            try
            {
                var userId = GetUserId();

                // Check if user is a member
                var isMember = await _context.ChatRoomMembers
                    .AnyAsync(m => m.ChatRoomId == roomId && m.UserId == userId && m.IsActive);

                if (!isMember)
                {
                    await Clients.Caller.SendAsync("Error", "You are not a member of this room");
                    return;
                }

                // Save message to database
                var message = new ChatMessage
                {
                    ChatRoomId = roomId,
                    UserId = userId,
                    Content = content,
                    CreatedAt = DateTime.UtcNow
                };

                _context.ChatMessages.Add(message);
                await _context.SaveChangesAsync();

                // Load user info
                var user = await _context.Users.FindAsync(userId);

                // Send to all clients in the room
                await Clients.Group($"room_{roomId}")
                    .SendAsync("MessageReceived", new
                    {
                        id = message.Id,
                        roomId = message.ChatRoomId,
                        userId = message.UserId,
                        userName = user?.FullName ?? "Unknown",
                        content = message.Content,
                        createdAt = message.CreatedAt,
                        isOwn = false
                    });
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("Error", ex.Message);
            }
        }

        /// <summary>
        /// Send typing indicator
        /// </summary>
        public async Task SendTypingIndicator(int roomId, bool isTyping)
        {
            try
            {
                var userId = GetUserId();
                var user = await _context.Users.FindAsync(userId);

                await Clients.Group($"room_{roomId}")
                    .SendAsync("TypingIndicator", new
                    {
                        userId = userId,
                        userName = user?.FullName ?? "Unknown",
                        isTyping = isTyping
                    });
            }
            catch (Exception ex)
            {
                await Clients.Caller.SendAsync("Error", ex.Message);
            }
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await base.OnDisconnectedAsync(exception);
        }
    }
}
