namespace SmartRideBackend.Models
{
    /// <summary>
    /// Represents a bus company/operator
    /// </summary>
    public class BusCompany
    {
        /// <summary>
        /// Unique identifier
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Company name
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Company logo URL
        /// </summary>
        public string Logo { get; set; } = string.Empty;

        /// <summary>
        /// Company description
        /// </summary>
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Company phone number
        /// </summary>
        public string PhoneNumber { get; set; } = string.Empty;

        /// <summary>
        /// Company email address
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Company address
        /// </summary>
        public string Address { get; set; } = string.Empty;

        /// <summary>
        /// Flag to indicate if company is active
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Flag to indicate if company is hidden from UI
        /// </summary>
        public bool IsHidden { get; set; } = false;

        /// <summary>
        /// Timestamp when company was created
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Timestamp when company was last updated
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        /// <summary>
        /// Collection of buses owned by this company
        /// </summary>
        public ICollection<Bus> Buses { get; set; } = new List<Bus>();

        /// <summary>
        /// Collection of trips operated by this company
        /// </summary>
        public ICollection<Trip> Trips { get; set; } = new List<Trip>();
    }
}
