using Microsoft.AspNetCore.Identity;

namespace SmartRideBackend.Models
{
    /// <summary>
    /// Represents an application user, extending IdentityUser for authentication.
    /// </summary>
    public class ApplicationUser : IdentityUser<int>
    {
        /// <summary>
        /// Full name of the user
        /// </summary>
        public string? FullName { get; set; }

        /// <summary>
        /// User avatar URL
        /// </summary>
        public string? Avatar { get; set; }

        /// <summary>
        /// Flag to indicate if user is active
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Timestamp when user was created
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Timestamp when user was last updated
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        /// <summary>
        /// Collection of tickets owned by this user
        /// </summary>
        public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    }
}
