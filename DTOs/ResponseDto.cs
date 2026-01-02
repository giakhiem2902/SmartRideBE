namespace SmartRideBackend.DTOs
{
    /// <summary>
    /// Generic API response wrapper
    /// </summary>
    /// <typeparam name="T">Type of data being returned</typeparam>
    public class ApiResponse<T>
    {
        /// <summary>
        /// Indicates if the request was successful
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Response message
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// Response data (nullable)
        /// </summary>
        public T? Data { get; set; }

        /// <summary>
        /// Error details (nullable)
        /// </summary>
        public string? Error { get; set; }

        /// <summary>
        /// Timestamp of the response
        /// </summary>
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Paged API response for paginated data
    /// </summary>
    /// <typeparam name="T">Type of items in the page</typeparam>
    public class PagedResponse<T>
    {
        /// <summary>
        /// Indicates if the request was successful
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Response message
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// List of items in the current page
        /// </summary>
        public ICollection<T> Data { get; set; } = new List<T>();

        /// <summary>
        /// Current page number (1-based)
        /// </summary>
        public int PageNumber { get; set; }

        /// <summary>
        /// Number of items per page
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// Total number of items across all pages
        /// </summary>
        public int TotalItems { get; set; }

        /// <summary>
        /// Total number of pages
        /// </summary>
        public int TotalPages => (TotalItems + PageSize - 1) / PageSize;

        /// <summary>
        /// Error details (nullable)
        /// </summary>
        public string? Error { get; set; }

        /// <summary>
        /// Timestamp of the response
        /// </summary>
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
