namespace SmartRideBackend.DTOs
{
    /// <summary>
    /// DTO for bus seat response
    /// </summary>
    public class SeatDto
    {
        /// <summary>
        /// Seat ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Seat number/identifier (e.g., A1, A2, B1)
        /// </summary>
        public string SeatNumber { get; set; } = string.Empty;

        /// <summary>
        /// Seat status (Available, Reserved, Booked)
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// Bus ID this seat belongs to
        /// </summary>
        public int BusId { get; set; }
    }
}
