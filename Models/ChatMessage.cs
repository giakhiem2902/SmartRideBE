using System;

namespace SmartRideBackend.Models
{
    public class ChatMessage
    {
        public int Id { get; set; }
        public int ChatRoomId { get; set; }
        public int UserId { get; set; }
        public string? Content { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? EditedAt { get; set; }
        public bool IsDeleted { get; set; } = false;

        // Navigation properties
        public ChatRoom? ChatRoom { get; set; }
        public ApplicationUser? User { get; set; }
    }
}
