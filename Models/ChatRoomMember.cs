using System;

namespace SmartRideBackend.Models
{
    public class ChatRoomMember
    {
        public int Id { get; set; }
        public int ChatRoomId { get; set; }
        public int UserId { get; set; }
        public DateTime JoinedAt { get; set; }
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public ChatRoom? ChatRoom { get; set; }
        public ApplicationUser? User { get; set; }
    }
}
