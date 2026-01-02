namespace SmartRideBackend.DTOs
{
    /// <summary>
    /// DTO for bus company response
    /// </summary>
    public class BusCompanyDto
    {
        /// <summary>
        /// Company ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Company name
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Company logo URL
        /// </summary>
        public string? Logo { get; set; }

        /// <summary>
        /// Company description
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Contact phone number
        /// </summary>
        public string? PhoneNumber { get; set; }

        /// <summary>
        /// Contact email address
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Company address
        /// </summary>
        public string? Address { get; set; }

        /// <summary>
        /// Indicates if company is active
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Indicates if company data is hidden from users
        /// </summary>
        public bool IsHidden { get; set; }
    }

    /// <summary>
    /// DTO for creating a new bus company (Admin only)
    /// </summary>
    public class CreateBusCompanyDto
    {
        /// <summary>
        /// Company name (required)
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Company logo URL (optional)
        /// </summary>
        public string? Logo { get; set; }

        /// <summary>
        /// Company description (optional)
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Contact phone number (optional)
        /// </summary>
        public string? PhoneNumber { get; set; }

        /// <summary>
        /// Contact email address (optional)
        /// </summary>
        public string? Email { get; set; }

        /// <summary>
        /// Company address (optional)
        /// </summary>
        public string? Address { get; set; }
    }
}
