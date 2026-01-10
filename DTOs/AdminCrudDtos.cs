namespace SmartRideBackend.DTOs
{
    /// <summary>
    /// DTO for updating an existing bus company
    /// </summary>
    public class UpdateBusCompanyDto
    {
        /// <summary>
        /// Company name
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Company phone number
        /// </summary>
        public string? PhoneNumber { get; set; }

        /// <summary>
        /// Company email
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Company address
        /// </summary>
        public string? Address { get; set; }

        /// <summary>
        /// Company description
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Company logo URL
        /// </summary>
        public string? Logo { get; set; }

        /// <summary>
        /// Is company active
        /// </summary>
        public bool? IsActive { get; set; }
    }

    /// <summary>
    /// DTO for updating an existing trip
    /// </summary>
    public class UpdateTripDto
    {
        /// <summary>
        /// Departure city name
        /// </summary>
        public string? DepartureCity { get; set; }

        /// <summary>
        /// Arrival city name
        /// </summary>
        public string? ArrivalCity { get; set; }

        /// <summary>
        /// Departure date and time
        /// </summary>
        public DateTime? DepartureTime { get; set; }

        /// <summary>
        /// Expected arrival date and time
        /// </summary>
        public DateTime? ArrivalTime { get; set; }

        /// <summary>
        /// Price per seat
        /// </summary>
        public decimal? Price { get; set; }

        /// <summary>
        /// Total seats available
        /// </summary>
        public int? TotalSeats { get; set; }

        /// <summary>
        /// Is trip active
        /// </summary>
        public bool? IsActive { get; set; }

        /// <summary>
        /// Is trip hidden from users
        /// </summary>
        public bool? IsHidden { get; set; }
    }

    /// <summary>
    /// DTO for toggling trip visibility
    /// </summary>
    public class ToggleTripVisibilityDto
    {
        /// <summary>
        /// Hide or show trip
        /// </summary>
        public bool IsHidden { get; set; }
    }

    /// <summary>
    /// DTO for toggling user status
    /// </summary>
    public class ToggleUserStatusDto
    {
        /// <summary>
        /// User active status
        /// </summary>
        public bool IsActive { get; set; }
    }
}
