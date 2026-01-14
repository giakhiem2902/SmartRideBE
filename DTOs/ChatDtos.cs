using System;
using System.Collections.Generic;

namespace SmartRideBackend.DTOs
{
    /// <summary>
    /// DTO for creating a chat room
    /// </summary>
    public class CreateChatRoomDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO for chat room response
    /// </summary>
    public class ChatRoomDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int CreatedByUserId { get; set; }
        public string CreatedByUserName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; }
        public int MemberCount { get; set; }
        public int UnreadCount { get; set; }
        public ChatMessageDto? LastMessage { get; set; }
    }

    /// <summary>
    /// DTO for sending a chat message
    /// </summary>
    public class SendChatMessageDto
    {
        public int ChatRoomId { get; set; }
        public string Content { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO for chat message response
    /// </summary>
    public class ChatMessageDto
    {
        public int Id { get; set; }
        public int ChatRoomId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string UserAvatar { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? EditedAt { get; set; }
        public bool IsOwn { get; set; }
    }

    /// <summary>
    /// DTO for chat room members
    /// </summary>
    public class ChatRoomMemberDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public DateTime JoinedAt { get; set; }
        public bool IsActive { get; set; }
    }

    /// <summary>
    /// DTO for inviting user to chat room
    /// </summary>
    public class InviteUserToChatRoomDto
    {
        public int UserId { get; set; }
    }

    /// <summary>
    /// DTO for user search results
    /// </summary>
    public class UserSearchDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO for creating a support ticket
    /// </summary>
    public class CreateSupportTicketDto
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty; // "Payment", "Booking", "Technical", "General"
        public int Priority { get; set; } = 2; // 1-High, 2-Medium, 3-Low
    }

    /// <summary>
    /// DTO for support ticket response
    /// </summary>
    public class SupportTicketDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public int Priority { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ResolvedAt { get; set; }
        public string? AssignedToUserName { get; set; }
        public string? Resolution { get; set; }
    }

    /// <summary>
    /// DTO for updating support ticket (Admin/Support)
    /// </summary>
    public class UpdateSupportTicketDto
    {
        public string Status { get; set; } = string.Empty;
        public string? Resolution { get; set; }
        public int? Priority { get; set; }
    }

    /// <summary>
    /// DTO for FAQ items
    /// </summary>
    public class FaqItemDto
    {
        public int Id { get; set; }
        public string Question { get; set; } = string.Empty;
        public string Answer { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public int ViewCount { get; set; }
    }
}
