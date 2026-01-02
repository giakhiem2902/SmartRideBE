namespace SmartRideBackend.Models
{
    /// <summary>
    /// Represents a trip (scheduled bus journey)
    /// </summary>
    public class Trip
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
        /// Foreign key to BusCompany
        /// </summary>
        public int BusCompanyId { get; set; }

        /// <summary>
        /// Foreign key to departure Province
        /// </summary>
        public int DepartureProvinceId { get; set; }

        /// <summary>
        /// Foreign key to arrival Province
        /// </summary>
        public int ArrivalProvinceId { get; set; }

        /// <summary>
        /// Departure city name
        /// </summary>
        public string DepartureCity { get; set; } = string.Empty;

        /// <summary>
        /// Arrival city name
        /// </summary>
        public string ArrivalCity { get; set; } = string.Empty;

        /// <summary>
        /// Departure time for the trip
        /// </summary>
        public DateTime DepartureTime { get; set; }

        /// <summary>
        /// Expected arrival time
        /// </summary>
        public DateTime ArrivalTime { get; set; }

        /// <summary>
        /// Price per seat
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Total seats available in the bus for this trip
        /// </summary>
        public int TotalSeats { get; set; }

        /// <summary>
        /// Number of seats already booked
        /// </summary>
        public int BookedSeats { get; set; } = 0;

        /// <summary>
        /// Number of available seats (calculated)
        /// </summary>
        public int AvailableSeats => TotalSeats - BookedSeats;

        /// <summary>
        /// Flag to indicate if trip is active
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Flag to indicate if trip is hidden from UI
        /// </summary>
        public bool IsHidden { get; set; } = false;

        /// <summary>
        /// Flag to indicate if trip is deleted
        /// </summary>
        public bool IsDeleted { get; set; } = false;

        /// <summary>
        /// Timestamp when trip was created
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Timestamp when trip was last updated
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Foreign keys
        /// <summary>
        /// Navigation property to Bus
        /// </summary>
        public Bus? Bus { get; set; }

        /// <summary>
        /// Navigation property to BusCompany
        /// </summary>
        public BusCompany? BusCompany { get; set; }

        // Navigation properties
        /// <summary>
        /// Collection of tickets for this trip
        /// </summary>
        public ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    }
}
