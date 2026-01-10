using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartRideBackend.Data;
using SmartRideBackend.DTOs;
using SmartRideBackend.Models;

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
                    .Where(c => !c.IsDeleted)
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
                    .Where(t => !t.IsDeleted)
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
                    .Include(u => u.Tickets)
                    .ToListAsync();

                var userDtos = users
                    .Select(u => new AdminUserDto
                    {
                        Id = u.Id,
                        UserName = u.UserName ?? string.Empty,
                        Email = u.Email ?? string.Empty,
                        FullName = u.FullName ?? string.Empty,
                        PhoneNumber = u.PhoneNumber ?? string.Empty,
                        Avatar = u.Avatar,
                        IsActive = u.IsActive,
                        CreatedAt = u.CreatedAt,
                        BookingCount = u.Tickets?.Where(t => !t.IsDeleted).Count() ?? 0
                    })
                    .OrderByDescending(u => u.Id)
                    .ToList();

                return Ok(new ApiResponse<List<AdminUserDto>>
                {
                    Success = true,
                    Data = userDtos
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

        /// <summary>
        /// Get all tickets with details
        /// </summary>
        [HttpGet("tickets")]
        public async Task<ActionResult<ApiResponse<List<AdminTicketDto>>>> GetTickets()
        {
            try
            {
                var tickets = await _context.Tickets
                    .AsNoTracking()
                    .Where(t => !t.IsDeleted)
                    .Include(t => t.Trip)
                    .ThenInclude(tr => tr!.BusCompany)
                    .Include(t => t.User)
                    .Include(t => t.TicketSeats)
                    .ThenInclude(ts => ts.BusSeat)
                    .Select(t => new AdminTicketDto
                    {
                        Id = t.Id,
                        TicketNumber = t.TicketNumber,
                        UserId = t.UserId,
                        UserName = (t.User != null ? (t.User.FullName ?? t.User.UserName) : "Unknown") ?? "Unknown",
                        TripId = t.TripId,
                        TripRoute = t.Trip != null ? $"{t.Trip.DepartureCity} → {t.Trip.ArrivalCity}" : "N/A",
                        NumberOfSeats = t.NumberOfSeats,
                        TotalPrice = t.TotalPrice,
                        SeatNumbers = t.TicketSeats != null 
                            ? t.TicketSeats.Select(ts => ts.BusSeat != null ? ts.BusSeat.SeatNumber : ts.BusSeatId.ToString()).ToList()
                            : new List<string>(),
                        Status = t.Status.ToString(),
                        BookingDate = t.CreatedAt,
                        PaymentDate = t.PaymentDate,
                        BoardingDate = null,
                        IsActive = t.IsActive,
                        IsHidden = t.IsHidden,
                        IsDeleted = t.IsDeleted,
                        Trip = t.Trip != null ? new TripDto
                        {
                            Id = t.Trip.Id,
                            DepartureCity = t.Trip.DepartureCity,
                            ArrivalCity = t.Trip.ArrivalCity,
                            DepartureTime = t.Trip.DepartureTime,
                            ArrivalTime = t.Trip.ArrivalTime,
                            Price = t.Trip.Price,
                            TotalSeats = t.Trip.TotalSeats,
                            BusCompanyId = t.Trip.BusCompanyId,
                        } : null
                    })
                    .OrderByDescending(t => t.BookingDate)
                    .ToListAsync();

                return Ok(new ApiResponse<List<AdminTicketDto>>
                {
                    Success = true,
                    Data = tickets
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<List<AdminTicketDto>>
                {
                    Success = false,
                    Message = $"Error fetching tickets: {ex.Message}"
                });
            }
        }

        // ==================== COMPANY CRUD ====================

        /// <summary>
        /// Create a new bus company
        /// </summary>
        [HttpPost("companies")]
        public async Task<ActionResult<ApiResponse<BusCompanyDto>>> CreateCompany([FromBody] CreateBusCompanyDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();
                    var errorMessage = string.Join(", ", errors);
                    Console.WriteLine($"[CreateCompany] ModelState errors: {errorMessage}");
                    Console.WriteLine($"[CreateCompany] Received DTO: Name={dto.Name}, Phone={dto.PhoneNumber}, Email={dto.Email}, Address={dto.Address}");
                    return BadRequest(new ApiResponse<BusCompanyDto> 
                    { 
                        Success = false, 
                        Message = $"Invalid data: {errorMessage}"
                    });
                }

                // Check if company name already exists
                var existingCompany = await _context.BusCompanies
                    .FirstOrDefaultAsync(c => c.Name == dto.Name);
                
                if (existingCompany != null)
                    return BadRequest(new ApiResponse<BusCompanyDto> 
                    { 
                        Success = false, 
                        Message = "Company with this name already exists" 
                    });

                var company = new BusCompany
                {
                    Name = dto.Name,
                    PhoneNumber = dto.PhoneNumber ?? string.Empty,
                    Email = dto.Email ?? string.Empty,
                    Address = dto.Address ?? string.Empty,
                    Description = dto.Description ?? string.Empty,
                    Logo = dto.Logo ?? string.Empty,
                    IsActive = true,
                    IsHidden = false,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.BusCompanies.Add(company);
                await _context.SaveChangesAsync();

                var companyDto = new BusCompanyDto
                {
                    Id = company.Id,
                    Name = company.Name,
                    PhoneNumber = company.PhoneNumber,
                    Email = company.Email,
                    Address = company.Address,
                    Description = company.Description,
                    Logo = company.Logo,
                    IsActive = company.IsActive,
                    IsHidden = company.IsHidden
                };

                return Ok(new ApiResponse<BusCompanyDto>
                {
                    Success = true,
                    Message = "Company created successfully",
                    Data = companyDto
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<BusCompanyDto>
                {
                    Success = false,
                    Message = $"Error creating company: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Update an existing bus company
        /// </summary>
        [HttpPut("companies/{id}")]
        public async Task<ActionResult<ApiResponse<BusCompanyDto>>> UpdateCompany(int id, [FromBody] UpdateBusCompanyDto dto)
        {
            try
            {
                var company = await _context.BusCompanies.FindAsync(id);
                if (company == null)
                    return NotFound(new ApiResponse<BusCompanyDto> { Success = false, Message = "Company not found" });

                if (!string.IsNullOrEmpty(dto.Name) && dto.Name != company.Name)
                {
                    var existingCompany = await _context.BusCompanies
                        .FirstOrDefaultAsync(c => c.Name == dto.Name && c.Id != id);
                    if (existingCompany != null)
                        return BadRequest(new ApiResponse<BusCompanyDto> 
                        { 
                            Success = false, 
                            Message = "Company with this name already exists" 
                        });
                }

                // Update fields if provided
                if (!string.IsNullOrEmpty(dto.Name))
                    company.Name = dto.Name;
                if (!string.IsNullOrEmpty(dto.PhoneNumber))
                    company.PhoneNumber = dto.PhoneNumber;
                if (!string.IsNullOrEmpty(dto.Email))
                    company.Email = dto.Email;
                if (!string.IsNullOrEmpty(dto.Address))
                    company.Address = dto.Address;
                if (!string.IsNullOrEmpty(dto.Description))
                    company.Description = dto.Description;
                if (!string.IsNullOrEmpty(dto.Logo))
                    company.Logo = dto.Logo;
                if (dto.IsActive.HasValue)
                    company.IsActive = dto.IsActive.Value;

                company.UpdatedAt = DateTime.UtcNow;
                _context.BusCompanies.Update(company);
                await _context.SaveChangesAsync();

                var companyDto = new BusCompanyDto
                {
                    Id = company.Id,
                    Name = company.Name,
                    PhoneNumber = company.PhoneNumber,
                    Email = company.Email,
                    Address = company.Address,
                    Description = company.Description,
                    Logo = company.Logo,
                    IsActive = company.IsActive,
                    IsHidden = company.IsHidden
                };

                return Ok(new ApiResponse<BusCompanyDto>
                {
                    Success = true,
                    Message = "Company updated successfully",
                    Data = companyDto
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<BusCompanyDto>
                {
                    Success = false,
                    Message = $"Error updating company: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Delete a bus company (soft delete)
        /// </summary>
        [HttpDelete("companies/{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteCompany(int id)
        {
            try
            {
                var company = await _context.BusCompanies.FindAsync(id);
                if (company == null)
                    return NotFound(new ApiResponse<bool> { Success = false, Message = "Company not found" });

                // Check if company has active trips
                var activeTrips = await _context.Trips
                    .Where(t => t.BusCompanyId == id && !t.IsDeleted)
                    .CountAsync();

                if (activeTrips > 0)
                    return BadRequest(new ApiResponse<bool> 
                    { 
                        Success = false, 
                        Message = $"Cannot delete company with {activeTrips} active trips" 
                    });

                // Soft delete
                company.IsDeleted = true;
                company.UpdatedAt = DateTime.UtcNow;
                _context.BusCompanies.Update(company);
                await _context.SaveChangesAsync();

                return Ok(new ApiResponse<bool>
                {
                    Success = true,
                    Message = "Company deleted successfully",
                    Data = true
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<bool>
                {
                    Success = false,
                    Message = $"Error deleting company: {ex.Message}"
                });
            }
        }

        // ==================== TRIP CRUD ====================

        /// <summary>
        /// Create a new trip
        /// </summary>
        [HttpPost("trips")]
        public async Task<ActionResult<ApiResponse<AdminTripDto>>> CreateTrip([FromBody] CreateTripDto dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(new ApiResponse<AdminTripDto> { Success = false, Message = "Invalid data" });

                // Validate bus exists
                var bus = await _context.Buses.FindAsync(dto.BusId);
                if (bus == null)
                    return BadRequest(new ApiResponse<AdminTripDto> { Success = false, Message = "Bus not found" });

                // Validate company exists
                var company = await _context.BusCompanies.FindAsync(dto.BusCompanyId);
                if (company == null)
                    return BadRequest(new ApiResponse<AdminTripDto> { Success = false, Message = "Company not found" });

                // Validate times
                if (dto.DepartureTime >= dto.ArrivalTime)
                    return BadRequest(new ApiResponse<AdminTripDto> 
                    { 
                        Success = false, 
                        Message = "Departure time must be before arrival time" 
                    });

                var trip = new Trip
                {
                    BusId = dto.BusId,
                    BusCompanyId = dto.BusCompanyId,
                    DepartureProvinceId = dto.DepartureProvinceId,
                    ArrivalProvinceId = dto.ArrivalProvinceId,
                    DepartureCity = dto.DepartureCity,
                    ArrivalCity = dto.ArrivalCity,
                    DepartureTime = dto.DepartureTime,
                    ArrivalTime = dto.ArrivalTime,
                    Price = dto.Price,
                    TotalSeats = dto.TotalSeats,
                    BookedSeats = 0,
                    IsActive = true,
                    IsHidden = false,
                    IsDeleted = false,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };

                _context.Trips.Add(trip);
                await _context.SaveChangesAsync();

                var tripDto = new AdminTripDto
                {
                    Id = trip.Id,
                    DepartureCity = trip.DepartureCity,
                    ArrivalCity = trip.ArrivalCity,
                    DepartureTime = trip.DepartureTime,
                    ArrivalTime = trip.ArrivalTime,
                    Price = trip.Price,
                    TotalSeats = trip.TotalSeats,
                    BookedSeats = trip.BookedSeats,
                    CompanyId = trip.BusCompanyId,
                    CompanyName = company.Name,
                    IsActive = trip.IsActive,
                    IsHidden = trip.IsHidden
                };

                return Ok(new ApiResponse<AdminTripDto>
                {
                    Success = true,
                    Message = "Trip created successfully",
                    Data = tripDto
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<AdminTripDto>
                {
                    Success = false,
                    Message = $"Error creating trip: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Update an existing trip
        /// </summary>
        [HttpPut("trips/{id}")]
        public async Task<ActionResult<ApiResponse<AdminTripDto>>> UpdateTrip(int id, [FromBody] UpdateTripDto dto)
        {
            try
            {
                var trip = await _context.Trips
                    .Include(t => t.BusCompany)
                    .FirstOrDefaultAsync(t => t.Id == id);
                
                if (trip == null)
                    return NotFound(new ApiResponse<AdminTripDto> { Success = false, Message = "Trip not found" });

                // Update fields if provided
                if (!string.IsNullOrEmpty(dto.DepartureCity))
                    trip.DepartureCity = dto.DepartureCity;
                if (!string.IsNullOrEmpty(dto.ArrivalCity))
                    trip.ArrivalCity = dto.ArrivalCity;
                if (dto.DepartureTime.HasValue)
                    trip.DepartureTime = dto.DepartureTime.Value;
                if (dto.ArrivalTime.HasValue)
                    trip.ArrivalTime = dto.ArrivalTime.Value;
                if (dto.Price.HasValue)
                    trip.Price = dto.Price.Value;
                if (dto.TotalSeats.HasValue)
                    trip.TotalSeats = dto.TotalSeats.Value;
                if (dto.IsActive.HasValue)
                    trip.IsActive = dto.IsActive.Value;
                if (dto.IsHidden.HasValue)
                    trip.IsHidden = dto.IsHidden.Value;

                // Validate times
                if (trip.DepartureTime >= trip.ArrivalTime)
                    return BadRequest(new ApiResponse<AdminTripDto> 
                    { 
                        Success = false, 
                        Message = "Departure time must be before arrival time" 
                    });

                trip.UpdatedAt = DateTime.UtcNow;
                _context.Trips.Update(trip);
                await _context.SaveChangesAsync();

                var tripDto = new AdminTripDto
                {
                    Id = trip.Id,
                    DepartureCity = trip.DepartureCity,
                    ArrivalCity = trip.ArrivalCity,
                    DepartureTime = trip.DepartureTime,
                    ArrivalTime = trip.ArrivalTime,
                    Price = trip.Price,
                    TotalSeats = trip.TotalSeats,
                    BookedSeats = trip.BookedSeats,
                    CompanyId = trip.BusCompanyId,
                    CompanyName = trip.BusCompany?.Name ?? "Unknown",
                    IsActive = trip.IsActive,
                    IsHidden = trip.IsHidden
                };

                return Ok(new ApiResponse<AdminTripDto>
                {
                    Success = true,
                    Message = "Trip updated successfully",
                    Data = tripDto
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<AdminTripDto>
                {
                    Success = false,
                    Message = $"Error updating trip: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Delete a trip (soft delete)
        /// </summary>
        [HttpDelete("trips/{id}")]
        public async Task<ActionResult<ApiResponse<bool>>> DeleteTrip(int id)
        {
            try
            {
                var trip = await _context.Trips.FindAsync(id);
                if (trip == null)
                    return NotFound(new ApiResponse<bool> { Success = false, Message = "Trip not found" });

                // Check if trip has bookings
                var bookingCount = await _context.Tickets
                    .Where(t => t.TripId == id && !t.IsDeleted && t.Status != TicketStatus.Cancelled)
                    .CountAsync();

                if (bookingCount > 0)
                    return BadRequest(new ApiResponse<bool> 
                    { 
                        Success = false, 
                        Message = $"Cannot delete trip with {bookingCount} active bookings" 
                    });

                // Soft delete
                trip.IsDeleted = true;
                trip.UpdatedAt = DateTime.UtcNow;
                _context.Trips.Update(trip);
                await _context.SaveChangesAsync();

                return Ok(new ApiResponse<bool>
                {
                    Success = true,
                    Message = "Trip deleted successfully",
                    Data = true
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<bool>
                {
                    Success = false,
                    Message = $"Error deleting trip: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Hide or show a trip from users
        /// </summary>
        [HttpPut("trips/{id}/visibility")]
        public async Task<ActionResult<ApiResponse<bool>>> ToggleTripVisibility(int id, [FromBody] ToggleTripVisibilityDto dto)
        {
            try
            {
                var trip = await _context.Trips.FindAsync(id);
                if (trip == null)
                    return NotFound(new ApiResponse<bool> { Success = false, Message = "Trip not found" });

                trip.IsHidden = dto.IsHidden;
                trip.UpdatedAt = DateTime.UtcNow;
                _context.Trips.Update(trip);
                await _context.SaveChangesAsync();

                return Ok(new ApiResponse<bool>
                {
                    Success = true,
                    Message = dto.IsHidden ? "Trip hidden successfully" : "Trip shown successfully",
                    Data = true
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<bool>
                {
                    Success = false,
                    Message = $"Error toggling trip visibility: {ex.Message}"
                });
            }
        }

        // ==================== USER MANAGEMENT ====================

        /// <summary>
        /// Toggle user active status
        /// </summary>
        [HttpPut("users/{id}/status")]
        public async Task<ActionResult<ApiResponse<bool>>> ToggleUserStatus(int id, [FromBody] ToggleUserStatusDto dto)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                    return NotFound(new ApiResponse<bool> { Success = false, Message = "User not found" });

                user.IsActive = dto.IsActive;
                user.UpdatedAt = DateTime.UtcNow;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                return Ok(new ApiResponse<bool>
                {
                    Success = true,
                    Message = dto.IsActive ? "User activated successfully" : "User deactivated successfully",
                    Data = true
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<bool>
                {
                    Success = false,
                    Message = $"Error updating user status: {ex.Message}"
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
        public int BookingCount { get; set; }
    }
}
