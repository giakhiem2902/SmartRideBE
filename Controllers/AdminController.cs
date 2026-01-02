using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartRideBackend.Data;
using SmartRideBackend.DTOs;

namespace SmartRideBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class AdminController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get dashboard statistics
        /// </summary>
        [HttpGet("stats")]
        public async Task<ActionResult<ApiResponse<AdminStatsDto>>> GetStats()
        {
            try
            {
                var totalUsers = await _context.Users.CountAsync();
                var totalCompanies = await _context.BusCompanies.CountAsync();
                var totalTrips = await _context.Trips.CountAsync();
                
                // Calculate total revenue from tickets
                var totalRevenue = await _context.Tickets
                    .Where(t => t.IsActive && !t.IsDeleted)
                    .SumAsync(t => (decimal?)t.TotalPrice) ?? 0;

                var stats = new AdminStatsDto
                {
                    TotalUsers = totalUsers,
                    TotalCompanies = totalCompanies,
                    TotalTrips = totalTrips,
                    TotalRevenue = (double)totalRevenue
                };

                return Ok(new ApiResponse<AdminStatsDto>
                {
                    Success = true,
                    Data = stats
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<AdminStatsDto>
                {
                    Success = false,
                    Message = $"Error fetching stats: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Get all bus companies with details
        /// </summary>
        [HttpGet("companies")]
        public async Task<ActionResult<ApiResponse<List<BusCompanyDto>>>> GetCompanies()
        {
            try
            {
                var companies = await _context.BusCompanies
                    .AsNoTracking()
                    .Select(c => new BusCompanyDto
                    {
                        Id = c.Id,
                        Name = c.Name,
                        Email = c.Email,
                        PhoneNumber = c.PhoneNumber,
                        Address = c.Address,
                        Logo = c.Logo,
                        Description = c.Description,
                        IsActive = c.IsActive,
                        IsHidden = c.IsHidden
                    })
                    .OrderByDescending(c => c.Id)
                    .ToListAsync();

                return Ok(new ApiResponse<List<BusCompanyDto>>
                {
                    Success = true,
                    Data = companies
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<List<BusCompanyDto>>
                {
                    Success = false,
                    Message = $"Error fetching companies: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Get all trips with company information
        /// </summary>
        [HttpGet("trips")]
        public async Task<ActionResult<ApiResponse<List<AdminTripDto>>>> GetTrips()
        {
            try
            {
                var trips = await _context.Trips
                    .Include(t => t.BusCompany)
                    .AsNoTracking()
                    .Select(t => new AdminTripDto
                    {
                        Id = t.Id,
                        DepartureCity = t.DepartureCity,
                        ArrivalCity = t.ArrivalCity,
                        DepartureTime = t.DepartureTime,
                        ArrivalTime = t.ArrivalTime,
                        Price = t.Price,
                        TotalSeats = t.TotalSeats,
                        BookedSeats = t.BookedSeats,
                        CompanyId = t.BusCompanyId,
                        CompanyName = t.BusCompany != null ? t.BusCompany.Name : "Unknown",
                        IsActive = t.IsActive,
                        IsHidden = t.IsHidden
                    })
                    .OrderByDescending(t => t.Id)
                    .ToListAsync();

                return Ok(new ApiResponse<List<AdminTripDto>>
                {
                    Success = true,
                    Data = trips
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<List<AdminTripDto>>
                {
                    Success = false,
                    Message = $"Error fetching trips: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Get all users
        /// </summary>
        [HttpGet("users")]
        public async Task<ActionResult<ApiResponse<List<AdminUserDto>>>> GetUsers()
        {
            try
            {
                var users = await _context.Users
                    .AsNoTracking()
                    .Select(u => new AdminUserDto
                    {
                        Id = u.Id,
                        UserName = u.UserName ?? string.Empty,
                        Email = u.Email ?? string.Empty,
                        FullName = u.FullName ?? string.Empty,
                        PhoneNumber = u.PhoneNumber ?? string.Empty,
                        Avatar = u.Avatar,
                        IsActive = u.IsActive,
                        CreatedAt = u.CreatedAt
                    })
                    .OrderByDescending(u => u.Id)
                    .ToListAsync();

                return Ok(new ApiResponse<List<AdminUserDto>>
                {
                    Success = true,
                    Data = users
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<List<AdminUserDto>>
                {
                    Success = false,
                    Message = $"Error fetching users: {ex.Message}"
                });
            }
        }
    }

    /// <summary>
    /// Admin stats response
    /// </summary>
    public class AdminStatsDto
    {
        public int TotalUsers { get; set; }
        public int TotalCompanies { get; set; }
        public int TotalTrips { get; set; }
        public double TotalRevenue { get; set; }
    }

    /// <summary>
    /// Admin trip DTO with company information
    /// </summary>
    public class AdminTripDto
    {
        public int Id { get; set; }
        public string DepartureCity { get; set; } = string.Empty;
        public string ArrivalCity { get; set; } = string.Empty;
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public decimal Price { get; set; }
        public int TotalSeats { get; set; }
        public int BookedSeats { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public bool IsHidden { get; set; }
    }

    /// <summary>
    /// Admin user DTO
    /// </summary>
    public class AdminUserDto
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string? Avatar { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
