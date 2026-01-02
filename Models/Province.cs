namespace SmartRideBackend.Models
{
    /// <summary>
    /// Represents a Vietnamese province or city
    /// </summary>
    public class Province
    {
        /// <summary>
        /// Unique identifier
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Province/City name
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Province/City code (ISO code if available)
        /// </summary>
        public string Code { get; set; } = string.Empty;

        /// <summary>
        /// Flag to indicate if province is active
        /// </summary>
        public bool IsActive { get; set; } = true;

        /// <summary>
        /// Timestamp when province was created
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
