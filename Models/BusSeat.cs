namespace SmartRideBackend.Models
{
    /// <summary>
    /// Enum for seat status
    /// </summary>
    public enum SeatStatus
    {
        /// <summary>
        /// Seat is available for booking
        /// </summary>
        Available = 1,

        /// <summary>
        /// Seat is reserved
        /// </summary>
        Reserved = 2,

        /// <summary>
        /// Seat is booked
        /// </summary>
        Booked = 3
    }

    /// <summary>
    /// Represents a seat in a bus
    /// </summary>
    public class BusSeat
    {
        /// <summary>
        /// Unique identifier
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Foreign key to Bus
        /// </summary>
        public int BusId { get; set; }

        /// <summary>
        /// Seat number (e.g., "A1", "B2")
        /// </summary>
        public string SeatNumber { get; set; } = string.Empty;

        /// <summary>
        /// Current status of the seat
        /// </summary>
        public SeatStatus Status { get; set; } = SeatStatus.Available;

        /// <summary>
        /// Timestamp when seat was created
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Timestamp when seat was last updated
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Foreign key
        /// <summary>
        /// Navigation property to Bus
        /// </summary>
        public Bus? Bus { get; set; }

        // Navigation properties
        /// <summary>
        /// Collection of tickets associated with this seat
        /// </summary>
        public ICollection<TicketSeat> TicketSeats { get; set; } = new List<TicketSeat>();
    }
}
