namespace SmartRideBackend.DTOs
{
    /// <summary>
    /// DTO for trip response
    /// </summary>
    public class TripDto
    {
        /// <summary>
        /// Trip ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Bus ID for this trip
        /// </summary>
        public int BusId { get; set; }

        /// <summary>
        /// Bus company ID
        /// </summary>
        public int BusCompanyId { get; set; }

        /// <summary>
        /// Bus company information
        /// </summary>
        public BusCompanyDto? BusCompany { get; set; }

        /// <summary>
        /// Departure city/province
        /// </summary>
        public string DepartureCity { get; set; } = string.Empty;

        /// <summary>
        /// Arrival city/province
        /// </summary>
        public string ArrivalCity { get; set; } = string.Empty;

        /// <summary>
        /// Departure date and time
        /// </summary>
        public DateTime DepartureTime { get; set; }

        /// <summary>
        /// Estimated arrival date and time
        /// </summary>
        public DateTime ArrivalTime { get; set; }

        /// <summary>
        /// Price per seat (VND)
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Total seats in the bus
        /// </summary>
        public int TotalSeats { get; set; }

        /// <summary>
        /// Number of booked seats
        /// </summary>
        public int BookedSeats { get; set; }

        /// <summary>
        /// Number of available seats (calculated)
        /// </summary>
        public int AvailableSeats => TotalSeats - BookedSeats;

        /// <summary>
        /// Indicates if trip is active
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Indicates if trip is hidden from users
        /// </summary>
        public bool IsHidden { get; set; }

        /// <summary>
        /// Indicates if trip is deleted
        /// </summary>
        public bool IsDeleted { get; set; }
    }

    /// <summary>
    /// DTO for creating a new trip (Admin only)
    /// </summary>
    public class CreateTripDto
    {
        /// <summary>
        /// Bus ID for this trip (required)
        /// </summary>
        public int BusId { get; set; }

        /// <summary>
        /// Bus company ID (required)
        /// </summary>
        public int BusCompanyId { get; set; }

        /// <summary>
        /// Departure city/province (required)
        /// </summary>
        public string DepartureCity { get; set; } = string.Empty;

        /// <summary>
        /// Arrival city/province (required)
        /// </summary>
        public string ArrivalCity { get; set; } = string.Empty;

        /// <summary>
        /// Departure date and time (required)
        /// </summary>
        public DateTime DepartureTime { get; set; }

        /// <summary>
        /// Estimated arrival date and time (required)
        /// </summary>
        public DateTime ArrivalTime { get; set; }

        /// <summary>
        /// Price per seat in VND (required)
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// Number of booked seats (default 0)
        /// </summary>
        public int BookedSeats { get; set; } = 0;
    }
}
