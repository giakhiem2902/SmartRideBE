namespace SmartRideBackend.Models
{
    /// <summary>
    /// Enum for ticket status
    /// </summary>
    public enum TicketStatus
    {
        /// <summary>
        /// Ticket is pending payment
        /// </summary>
        Pending = 1,

        /// <summary>
        /// Ticket is confirmed
        /// </summary>
        Confirmed = 2,

        /// <summary>
        /// Ticket is cancelled
        /// </summary>
        Cancelled = 3,

        /// <summary>
        /// Ticket has been used (passenger boarded)
        /// </summary>
        Used = 4
    }

    /// <summary>
    /// Represents a ticket for a bus trip
    /// </summary>
    public class Ticket
    {
        /// <summary>
        /// Unique identifier
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Foreign key to ApplicationUser
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Foreign key to Trip
        /// </summary>
        public int TripId { get; set; }

        /// <summary>
        /// Unique ticket number
        /// </summary>
        public string TicketNumber { get; set; } = string.Empty;

        /// <summary>
        /// QR code data for the ticket
        /// </summary>
        public string QRCode { get; set; } = string.Empty;

        /// <summary>
        /// Number of seats booked in this ticket
        /// </summary>
        public int NumberOfSeats { get; set; }

        /// <summary>
        /// Total price for all seats
        /// </summary>
        public decimal TotalPrice { get; set; }

        /// <summary>
        /// Current status of the ticket
        /// </summary>
        public TicketStatus Status { get; set; } = TicketStatus.Pending;

        /// <summary>
        /// Date when ticket was booked
        /// </summary>
        public DateTime BookingDate { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Date when payment was made
        /// </summary>
        public DateTime? PaymentDate { get; set; }

        /// <summary>
        /// Date when passenger boarded the bus
        /// </summary>
        public DateTime? BoardingDate { get; set; }

        /// <summary>
        /// Flag to indicate if ticket is active
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Flag to indicate if ticket is hidden from UI
        /// </summary>
        public bool IsHidden { get; set; } = false;

        /// <summary>
        /// Flag to indicate if ticket is deleted
        /// </summary>
        public bool IsDeleted { get; set; } = false;

        /// <summary>
        /// Timestamp when ticket was created
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Timestamp when ticket was last updated
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Foreign keys
        /// <summary>
        /// Navigation property to ApplicationUser
        /// </summary>
        public ApplicationUser? User { get; set; }

        /// <summary>
        /// Navigation property to Trip
        /// </summary>
        public Trip? Trip { get; set; }

        // Navigation properties
        /// <summary>
        /// Collection of seats booked in this ticket
        /// </summary>
        public ICollection<TicketSeat> TicketSeats { get; set; } = new List<TicketSeat>();
    }
}
