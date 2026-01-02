namespace SmartRideBackend.DTOs
{
    /// <summary>
    /// DTO for ticket response
    /// </summary>
    public class TicketDto
    {
        /// <summary>
        /// Ticket ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Unique ticket number
        /// </summary>
        public string TicketNumber { get; set; } = string.Empty;

        /// <summary>
        /// User ID who booked the ticket
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Trip ID for this ticket
        /// </summary>
        public int TripId { get; set; }

        /// <summary>
        /// Trip information
        /// </summary>
        public TripDto? Trip { get; set; }

        /// <summary>
        /// QR code for ticket validation
        /// </summary>
        public string? QRCode { get; set; }

        /// <summary>
        /// Number of seats booked
        /// </summary>
        public int NumberOfSeats { get; set; }

        /// <summary>
        /// Total price for all seats (VND)
        /// </summary>
        public decimal TotalPrice { get; set; }

        /// <summary>
        /// List of seat numbers booked
        /// </summary>
        public ICollection<string> SeatNumbers { get; set; } = new List<string>();

        /// <summary>
        /// Ticket status
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// Booking date and time
        /// </summary>
        public DateTime BookingDate { get; set; }

        /// <summary>
        /// Payment date (nullable)
        /// </summary>
        public DateTime? PaymentDate { get; set; }

        /// <summary>
        /// Boarding date (nullable)
        /// </summary>
        public DateTime? BoardingDate { get; set; }

        /// <summary>
        /// Indicates if ticket is active
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Indicates if ticket is hidden
        /// </summary>
        public bool IsHidden { get; set; }

        /// <summary>
        /// Indicates if ticket is deleted
        /// </summary>
        public bool IsDeleted { get; set; }
    }

    /// <summary>
    /// DTO for creating a new ticket (Authenticated users only)
    /// </summary>
    public class CreateTicketDto
    {
        /// <summary>
        /// Trip ID to book (required)
        /// </summary>
        public int TripId { get; set; }

        /// <summary>
        /// List of seat numbers to book (required, max 7 seats per ticket)
        /// </summary>
        public ICollection<string> SeatNumbers { get; set; } = new List<string>();

        /// <summary>
        /// List of seat IDs to book (alternative to SeatNumbers)
        /// </summary>
        public ICollection<int> SelectedSeatIds { get; set; } = new List<int>();

        /// <summary>
        /// Number of seats to book (must match SeatNumbers.Count or SelectedSeatIds.Count)
        /// </summary>
        public int NumberOfSeats { get; set; }
    }
}
