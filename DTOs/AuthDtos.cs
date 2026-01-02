namespace SmartRideBackend.DTOs
{
    /// <summary>
    /// DTO for user registration request
    /// </summary>
    public class RegisterRequestDto
    {
        /// <summary>
        /// User email address
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Unique username
        /// </summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// Full name of the user
        /// </summary>
        public string FullName { get; set; } = string.Empty;

        /// <summary>
        /// Password (min 6 characters)
        /// </summary>
        public string Password { get; set; } = string.Empty;

        /// <summary>
        /// Password confirmation must match Password
        /// </summary>
        public string ConfirmPassword { get; set; } = string.Empty;

        /// <summary>
        /// Phone number (optional)
        /// </summary>
        public string PhoneNumber { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO for user login request
    /// </summary>
    public class LoginRequestDto
    {
        /// <summary>
        /// User email address
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// User password
        /// </summary>
        public string Password { get; set; } = string.Empty;
    }

    /// <summary>
    /// DTO for authentication response
    /// </summary>
    public class AuthResponseDto
    {
        /// <summary>
        /// Indicates if authentication was successful
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Response message
        /// </summary>
        public string Message { get; set; } = string.Empty;

        /// <summary>
        /// JWT token (if successful)
        /// </summary>
        public string? Token { get; set; }

        /// <summary>
        /// User information (if successful)
        /// </summary>
        public UserDto? User { get; set; }

        /// <summary>
        /// Error list (if failed)
        /// </summary>
        public IEnumerable<string>? Errors { get; set; }
    }

    /// <summary>
    /// DTO representing a user
    /// </summary>
    public class UserDto
    {
        /// <summary>
        /// User ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Username
        /// </summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// Email address
        /// </summary>
        public string Email { get; set; } = string.Empty;

        /// <summary>
        /// Full name
        /// </summary>
        public string? FullName { get; set; }

        /// <summary>
        /// Phone number
        /// </summary>
        public string? PhoneNumber { get; set; }

        /// <summary>
        /// Avatar URL
        /// </summary>
        public string? Avatar { get; set; }

        /// <summary>
        /// Collection of user roles
        /// </summary>
        public ICollection<string> Roles { get; set; } = new List<string>();
    }
}
