using System;

namespace SmartRideBackend.Models
{
    public class SupportTicket
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; }
        public string Status { get; set; } = "Open";
        public int Priority { get; set; } = 1;
        public DateTime CreatedAt { get; set; }
        public DateTime? ResolvedAt { get; set; }
        public int? AssignedToUserId { get; set; }
        public string? Resolution { get; set; }
        public bool IsDeleted { get; set; } = false;

        // Navigation properties
        public ApplicationUser? User { get; set; }
        public ApplicationUser? AssignedToUser { get; set; }
    }
}
