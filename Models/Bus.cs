namespace SmartRideBackend.Models
{
    /// <summary>
    /// Enum for bus types
    /// </summary>
    public enum BusType
    {
        /// <summary>
        /// Limousine bus type
        /// </summary>
        Limousine = 1,

        /// <summary>
        /// Two-story bus type
        /// </summary>
        TwoStory = 2
    }

    /// <summary>
    /// Represents a bus vehicle
    /// </summary>
    public class Bus
    {
        /// <summary>
        /// Unique identifier
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Foreign key to BusCompany
        /// </summary>
        public int BusCompanyId { get; set; }

        /// <summary>
        /// Bus license plate number
        /// </summary>
        public string LicensePlate { get; set; } = string.Empty;

        /// <summary>
        /// Type of bus (Limousine or TwoStory)
        /// </summary>
        public BusType BusType { get; set; }

        /// <summary>
        /// Total number of seats in the bus
        /// </summary>
        public int TotalSeats { get; set; }

        /// <summary>
        /// Flag to indicate if bus is active
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Flag to indicate if bus is hidden from UI
        /// </summary>
        public bool IsHidden { get; set; } = false;

        /// <summary>
        /// Timestamp when bus was created
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Timestamp when bus was last updated
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Foreign key
        /// <summary>
        /// Navigation property to BusCompany
        /// </summary>
        public BusCompany? BusCompany { get; set; }

        // Navigation properties
        /// <summary>
        /// Collection of seats in this bus
        /// </summary>
        public ICollection<BusSeat> BusSeats { get; set; } = new List<BusSeat>();

        /// <summary>
        /// Collection of trips using this bus
        /// </summary>
        public ICollection<Trip> Trips { get; set; } = new List<Trip>();
    }
}
