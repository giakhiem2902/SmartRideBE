namespace SmartRideBackend.Models
{
    /// <summary>
    /// Join table representing the many-to-many relationship between Ticket and BusSeat
    /// </summary>
    public class TicketSeat
    {
        /// <summary>
        /// Unique identifier
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Foreign key to Ticket
        /// </summary>
        public int TicketId { get; set; }

        /// <summary>
        /// Foreign key to BusSeat
        /// </summary>
        public int BusSeatId { get; set; }

        /// <summary>
        /// Timestamp when ticket seat association was created
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Foreign keys
        /// <summary>
        /// Navigation property to Ticket
        /// </summary>
        public Ticket? Ticket { get; set; }

        /// <summary>
        /// Navigation property to BusSeat
        /// </summary>
        public BusSeat? BusSeat { get; set; }
    }
}
