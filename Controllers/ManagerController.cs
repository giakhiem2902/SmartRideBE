using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartRideBackend.Data;
using SmartRideBackend.DTOs;
using SmartRideBackend.Models;

namespace SmartRideBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Manager")]
    public class ManagerController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public ManagerController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        /// <summary>
        /// Get all trips managed by the current manager (all active trips for their company)
        /// </summary>
        [HttpGet("trips")]
        public async Task<ActionResult<ApiResponse<List<ManagerTripDto>>>> GetManagerTrips()
        {
            try
            {
                // For now, return all active trips. In a real app, filter by manager's company
                var trips = await _context.Trips
                    .Include(t => t.BusCompany)
                    .Where(t => t.IsActive && !t.IsDeleted)
                    .AsNoTracking()
                    .Select(t => new ManagerTripDto
                    {
                        Id = t.Id,
                        DepartureCity = t.DepartureCity,
                        ArrivalCity = t.ArrivalCity,
                        DepartureTime = t.DepartureTime,
                        ArrivalTime = t.ArrivalTime,
                        Price = t.Price,
                        TotalSeats = t.TotalSeats,
                        BookedSeats = t.BookedSeats,
                        AvailableSeats = t.TotalSeats - t.BookedSeats,
                        CompanyId = t.BusCompanyId,
                        CompanyName = t.BusCompany != null ? t.BusCompany.Name : "Unknown",
                        BusId = t.BusId,
                        Status = t.IsHidden ? "Hidden" : "Active"
                    })
                    .OrderByDescending(t => t.DepartureTime)
                    .ToListAsync();

                return Ok(new ApiResponse<List<ManagerTripDto>>
                {
                    Success = true,
                    Data = trips
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<List<ManagerTripDto>>
                {
                    Success = false,
                    Message = $"Error fetching trips: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Get all passengers (users with tickets) for a specific trip
        /// </summary>
        [HttpGet("trips/{tripId}/passengers")]
        public async Task<ActionResult<ApiResponse<List<ManagerPassengerDto>>>> GetTripPassengers(int tripId)
        {
            try
            {
                var trip = await _context.Trips.FindAsync(tripId);
                if (trip == null)
                    return NotFound(new ApiResponse<List<ManagerPassengerDto>>
                    {
                        Success = false,
                        Message = "Trip not found"
                    });

                var passengers = await _context.Tickets
                    .Include(t => t.User)
                    .Include(t => t.TicketSeats)
                    .Where(t => t.TripId == tripId && t.IsActive && !t.IsDeleted)
                    .AsNoTracking()
                    .Select(t => new ManagerPassengerDto
                    {
                        TicketId = t.Id,
                        TicketNumber = t.TicketNumber,
                        UserId = t.UserId,
                        UserName = t.User != null ? t.User.UserName ?? "" : "",
                        UserFullName = t.User != null ? t.User.FullName ?? "" : "",
                        UserPhoneNumber = t.User != null ? t.User.PhoneNumber ?? "" : "",
                        UserEmail = t.User != null ? t.User.Email ?? "" : "",
                        NumberOfSeats = t.NumberOfSeats,
                        TotalPrice = t.TotalPrice,
                        QRCode = t.QRCode,
                        Status = t.Status.ToString(),
                        BookingDate = t.BookingDate,
                        BoardingDate = t.BoardingDate,
                        SeatNumbers = t.TicketSeats != null
                            ? t.TicketSeats
                                .Join(_context.BusSeats, ts => ts.BusSeatId, bs => bs.Id, (ts, bs) => bs.SeatNumber)
                                .ToList()
                            : new List<string>()
                    })
                    .OrderBy(p => p.TicketNumber)
                    .ToListAsync();

                return Ok(new ApiResponse<List<ManagerPassengerDto>>
                {
                    Success = true,
                    Data = passengers
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<List<ManagerPassengerDto>>
                {
                    Success = false,
                    Message = $"Error fetching passengers: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Scan QR code and confirm passenger boarding
        /// </summary>
        [HttpPost("tickets/{ticketId}/confirm-boarding")]
        public async Task<ActionResult<ApiResponse<ConfirmBoardingResponseDto>>> ConfirmBoarding(int ticketId, [FromBody] ConfirmBoardingRequestDto request)
        {
            try
            {
                var ticket = await _context.Tickets
                    .Include(t => t.User)
                    .FirstOrDefaultAsync(t => t.Id == ticketId && t.IsActive && !t.IsDeleted);

                if (ticket == null)
                    return NotFound(new ApiResponse<ConfirmBoardingResponseDto>
                    {
                        Success = false,
                        Message = "Ticket not found"
                    });

                // Verify QR code matches
                if (ticket.QRCode != request.QRCode)
                {
                    return BadRequest(new ApiResponse<ConfirmBoardingResponseDto>
                    {
                        Success = false,
                        Message = "QR Code không khớp. Vui lòng kiểm tra lại."
                    });
                }

                // Check if already boarded
                if (ticket.BoardingDate.HasValue)
                {
                    return BadRequest(new ApiResponse<ConfirmBoardingResponseDto>
                    {
                        Success = false,
                        Message = "Hành khách này đã lên xe rồi.",
                        Data = new ConfirmBoardingResponseDto
                        {
                            IsAlreadyBoarded = true,
                            TicketNumber = ticket.TicketNumber,
                            PassengerName = ticket.User?.FullName ?? "Unknown"
                        }
                    });
                }

                // Update ticket with boarding confirmation
                ticket.BoardingDate = DateTime.Now;
                ticket.Status = TicketStatus.Used;
                ticket.UpdatedAt = DateTime.Now;

                await _context.SaveChangesAsync();

                return Ok(new ApiResponse<ConfirmBoardingResponseDto>
                {
                    Success = true,
                    Message = $"Xác nhận lên xe thành công - {ticket.User?.FullName}",
                    Data = new ConfirmBoardingResponseDto
                    {
                        TicketNumber = ticket.TicketNumber,
                        PassengerName = ticket.User?.FullName ?? "Unknown",
                        NumberOfSeats = ticket.NumberOfSeats,
                        IsAlreadyBoarded = false
                    }
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<ConfirmBoardingResponseDto>
                {
                    Success = false,
                    Message = $"Error confirming boarding: {ex.Message}"
                });
            }
        }

        /// <summary>
        /// Get passenger details by QR code
        /// </summary>
        [HttpPost("search-by-qr")]
        public async Task<ActionResult<ApiResponse<ManagerPassengerDetailDto>>> SearchByQRCode([FromBody] QRCodeScanRequestDto request)
        {
            try
            {
                var ticket = await _context.Tickets
                    .Include(t => t.User)
                    .Include(t => t.Trip)
                    .Include(t => t.TicketSeats)
                    .FirstOrDefaultAsync(t => t.QRCode == request.QRCode && t.IsActive && !t.IsDeleted);

                if (ticket == null)
                    return NotFound(new ApiResponse<ManagerPassengerDetailDto>
                    {
                        Success = false,
                        Message = "Vé không tìm thấy"
                    });

                var seatNumbers = new List<string>();
                if (ticket.TicketSeats != null && ticket.TicketSeats.Any())
                {
                    var seatIds = ticket.TicketSeats.Select(ts => ts.BusSeatId).ToList();
                    var seats = await _context.BusSeats
                        .Where(bs => seatIds.Contains(bs.Id))
                        .Select(bs => bs.SeatNumber)
                        .ToListAsync();
                    seatNumbers = seats;
                }

                var passengerDetail = new ManagerPassengerDetailDto
                {
                    TicketId = ticket.Id,
                    TicketNumber = ticket.TicketNumber,
                    PassengerName = ticket.User?.FullName ?? "Unknown",
                    PassengerPhone = ticket.User?.PhoneNumber ?? "N/A",
                    PassengerEmail = ticket.User?.Email ?? "N/A",
                    TripDepartureCity = ticket.Trip?.DepartureCity ?? "Unknown",
                    TripArrivalCity = ticket.Trip?.ArrivalCity ?? "Unknown",
                    DepartureTime = ticket.Trip?.DepartureTime ?? DateTime.MinValue,
                    NumberOfSeats = ticket.NumberOfSeats,
                    SeatNumbers = seatNumbers,
                    TotalPrice = ticket.TotalPrice,
                    BookingDate = ticket.BookingDate,
                    BoardingDate = ticket.BoardingDate,
                    Status = ticket.Status.ToString(),
                    IsBoarded = ticket.BoardingDate.HasValue
                };

                return Ok(new ApiResponse<ManagerPassengerDetailDto>
                {
                    Success = true,
                    Data = passengerDetail
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<ManagerPassengerDetailDto>
                {
                    Success = false,
                    Message = $"Error searching ticket: {ex.Message}"
                });
            }
        }
    }

    // DTOs for Manager endpoints
    public class ManagerTripDto
    {
        public int Id { get; set; }
        public string DepartureCity { get; set; } = string.Empty;
        public string ArrivalCity { get; set; } = string.Empty;
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public decimal Price { get; set; }
        public int TotalSeats { get; set; }
        public int BookedSeats { get; set; }
        public int AvailableSeats { get; set; }
        public int CompanyId { get; set; }
        public string CompanyName { get; set; } = string.Empty;
        public int BusId { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class ManagerPassengerDto
    {
        public int TicketId { get; set; }
        public string TicketNumber { get; set; } = string.Empty;
        public int UserId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string UserFullName { get; set; } = string.Empty;
        public string UserPhoneNumber { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public int NumberOfSeats { get; set; }
        public decimal TotalPrice { get; set; }
        public string? QRCode { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime BookingDate { get; set; }
        public DateTime? BoardingDate { get; set; }
        public List<string> SeatNumbers { get; set; } = new();
    }

    public class ManagerPassengerDetailDto
    {
        public int TicketId { get; set; }
        public string TicketNumber { get; set; } = string.Empty;
        public string PassengerName { get; set; } = string.Empty;
        public string PassengerPhone { get; set; } = string.Empty;
        public string PassengerEmail { get; set; } = string.Empty;
        public string TripDepartureCity { get; set; } = string.Empty;
        public string TripArrivalCity { get; set; } = string.Empty;
        public DateTime DepartureTime { get; set; }
        public int NumberOfSeats { get; set; }
        public List<string> SeatNumbers { get; set; } = new();
        public decimal TotalPrice { get; set; }
        public DateTime BookingDate { get; set; }
        public DateTime? BoardingDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public bool IsBoarded { get; set; }
    }

    public class ConfirmBoardingRequestDto
    {
        public string QRCode { get; set; } = string.Empty;
    }

    public class ConfirmBoardingResponseDto
    {
        public string TicketNumber { get; set; } = string.Empty;
        public string PassengerName { get; set; } = string.Empty;
        public int NumberOfSeats { get; set; }
        public bool IsAlreadyBoarded { get; set; }
    }

    public class QRCodeScanRequestDto
    {
        public string QRCode { get; set; } = string.Empty;
    }
}
