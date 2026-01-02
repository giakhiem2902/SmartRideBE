using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SmartRideBackend.Data;
using SmartRideBackend.Models;

namespace SmartRideBackend.Services
{
    public interface IAuthService
    {
        Task<(bool Success, string Message)> RegisterAsync(string email, string userName, string fullName, string password, string phoneNumber);
        Task<(bool Success, string Token, User? User)> LoginAsync(string email, string password);
        Task<User?> GetUserByIdAsync(int userId);
    }

    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<(bool Success, string Message)> RegisterAsync(
            string email,
            string userName,
            string fullName,
            string password,
            string phoneNumber)
        {
            var user = new ApplicationUser
            {
                UserName = userName,
                Email = email,
                PhoneNumber = phoneNumber,
                FullName = fullName
            };

            var result = await _userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return (false, errors);
            }

            await _userManager.AddToRoleAsync(user, "User");
            return (true, "User registered successfully");
        }

        public async Task<(bool Success, string Token, User? User)> LoginAsync(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return (false, "", null);

            var result = await _signInManager.PasswordSignInAsync(user, password, false, false);
            if (!result.Succeeded)
                return (false, "", null);

            // Generate token - Implementation in controller
            return (true, "", null);
        }

        public async Task<User?> GetUserByIdAsync(int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
                return null;

            var roles = await _userManager.GetRolesAsync(user);
            return new User
            {
                Id = user.Id,
                UserName = user.UserName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                Avatar = user.Avatar,
                Roles = roles
            };
        }
    }

    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Avatar { get; set; }
        public ICollection<string> Roles { get; set; } = new List<string>();
    }

    public interface ITripService
    {
        Task<List<Trip>> SearchTripsAsync(string departureCity, string arrivalCity, DateTime date);
        Task<Trip?> GetTripByIdAsync(int id);
        Task<int> GetAvailableSeatsAsync(int tripId);
    }

    public class TripService : ITripService
    {
        private readonly ApplicationDbContext _context;

        public TripService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Trip>> SearchTripsAsync(string departureCity, string arrivalCity, DateTime date)
        {
            return await _context.Trips
                .Where(t => t.DepartureCity == departureCity &&
                           t.ArrivalCity == arrivalCity &&
                           t.DepartureTime.Date == date.Date &&
                           t.IsActive &&
                           !t.IsHidden &&
                           !t.IsDeleted)
                .ToListAsync();
        }

        public async Task<Trip?> GetTripByIdAsync(int id)
        {
            return await _context.Trips
                .Include(t => t.BusCompany)
                .FirstOrDefaultAsync(t => t.Id == id && t.IsActive && !t.IsHidden);
        }

        public async Task<int> GetAvailableSeatsAsync(int tripId)
        {
            var trip = await _context.Trips.FirstOrDefaultAsync(t => t.Id == tripId);
            return trip?.AvailableSeats ?? 0;
        }
    }

    public interface ITicketService
    {
        Task<(bool Success, string Message, Ticket? Ticket)> CreateTicketAsync(int userId, int tripId, List<int> seatIds);
        Task<Ticket?> GetTicketByIdAsync(int id);
        Task<List<Ticket>> GetUserTicketsAsync(int userId);
        Task<(bool Success, string Message)> ConfirmTicketAsync(int id);
        Task<(bool Success, string Message)> CancelTicketAsync(int id, int userId);
    }

    public class TicketService : ITicketService
    {
        private readonly ApplicationDbContext _context;

        public TicketService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<(bool Success, string Message, Ticket? Ticket)> CreateTicketAsync(int userId, int tripId, List<int> seatIds)
        {
            // Validation logic
            if (seatIds.Count > 7)
                return (false, "Maximum 7 seats per booking", null);

            var trip = await _context.Trips.FirstOrDefaultAsync(t => t.Id == tripId);
            if (trip == null)
                return (false, "Trip not found", null);

            if (trip.AvailableSeats < seatIds.Count)
                return (false, "Not enough seats available", null);

            // Create ticket logic (implementation details in controller)
            return (true, "Ticket created", null);
        }

        public async Task<Ticket?> GetTicketByIdAsync(int id)
        {
            return await _context.Tickets
                .Include(t => t.Trip)
                .Include(t => t.TicketSeats)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<List<Ticket>> GetUserTicketsAsync(int userId)
        {
            return await _context.Tickets
                .Where(t => t.UserId == userId && !t.IsDeleted)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<(bool Success, string Message)> ConfirmTicketAsync(int id)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null)
                return (false, "Ticket not found");

            ticket.Status = TicketStatus.Used;
            ticket.BoardingDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return (true, "Ticket confirmed");
        }

        public async Task<(bool Success, string Message)> CancelTicketAsync(int id, int userId)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null)
                return (false, "Ticket not found");

            if (ticket.UserId != userId)
                return (false, "Unauthorized");

            ticket.IsDeleted = true;
            ticket.Status = TicketStatus.Cancelled;
            await _context.SaveChangesAsync();

            return (true, "Ticket cancelled");
        }
    }
}
