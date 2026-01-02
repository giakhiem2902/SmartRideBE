using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartRideBackend.Data;
using SmartRideBackend.DTOs;
using SmartRideBackend.Models;
using System.Security.Claims;

namespace SmartRideBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TicketsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TicketsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<TicketDto>>> CreateTicket([FromBody] CreateTicketDto dto)
        {
            Console.WriteLine($"CreateTicket called with TripId={dto.TripId}, SeatNumbers count={dto.SeatNumbers?.Count ?? 0}, SeatIds count={dto.SelectedSeatIds?.Count ?? 0}");
            Console.WriteLine($"SeatNumbers: {string.Join(", ", dto.SeatNumbers ?? new List<string>())}");
            Console.WriteLine($"SelectedSeatIds: {string.Join(", ", dto.SelectedSeatIds ?? new List<int>())}");
            
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userId, out var id))
                return Unauthorized();

            // Verify user exists
            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return BadRequest(new ApiResponse<TicketDto> { Success = false, Message = "User not found in database" });

            // Get trip and validate
            var trip = await _context.Trips
                .Include(t => t.Bus)
                .FirstOrDefaultAsync(t => t.Id == dto.TripId);

            if (trip == null)
                return BadRequest(new ApiResponse<TicketDto> { Success = false, Message = "Trip not found" });

            Console.WriteLine($"Trip found: Id={trip.Id}, BusId={trip.BusId}");
            Console.WriteLine($"Looking for seatNumbers: {string.Join(", ", dto.SeatNumbers ?? new List<string>())}");

            // Determine which seat selection method is being used
            List<BusSeat> selectedSeats;
            
            if (dto.SeatNumbers != null && dto.SeatNumbers.Any())
            {
                // Use seat numbers (e.g., "A01", "B02")
                // Get all seats for this bus to debug
                var allSeatsForBus = await _context.BusSeats
                    .Where(s => s.BusId == trip.BusId)
                    .ToListAsync();
                Console.WriteLine($"All seats for BusId={trip.BusId}: {string.Join(", ", allSeatsForBus.Select(s => s.SeatNumber))}");
                
                selectedSeats = await _context.BusSeats
                    .Where(s => s.BusId == trip.BusId && dto.SeatNumbers.Contains(s.SeatNumber))
                    .ToListAsync();
                
                Console.WriteLine($"Found {selectedSeats.Count} matching seats: {string.Join(", ", selectedSeats.Select(s => s.SeatNumber))}");
            }
            else if (dto.SelectedSeatIds != null && dto.SelectedSeatIds.Any())
            {
                // Use seat IDs
                selectedSeats = await _context.BusSeats
                    .Where(s => dto.SelectedSeatIds.Contains(s.Id))
                    .ToListAsync();
            }
            else
            {
                return BadRequest(new ApiResponse<TicketDto> { Success = false, Message = "No seats selected" });
            }

            if (selectedSeats.Count == 0)
                return BadRequest(new ApiResponse<TicketDto> { Success = false, Message = "Seats not found" });

            if (selectedSeats.Count > 7)
                return BadRequest(new ApiResponse<TicketDto> { Success = false, Message = "Maximum 7 seats per booking" });

            // Check available seats
            if (trip.AvailableSeats < selectedSeats.Count)
                return BadRequest(new ApiResponse<TicketDto> { Success = false, Message = "Not enough seats available" });

            if (selectedSeats.Any(s => s.Status != SeatStatus.Available))
                return BadRequest(new ApiResponse<TicketDto> { Success = false, Message = "Some seats are not available" });

            // Create ticket
            var ticketNumber = GenerateTicketNumber();
            var qrCode = GenerateQRCode(ticketNumber);

            var ticket = new Ticket
            {
                UserId = id,
                TripId = trip.Id,
                TicketNumber = ticketNumber,
                QRCode = qrCode,
                NumberOfSeats = selectedSeats.Count,
                TotalPrice = selectedSeats.Count * trip.Price,
                Status = TicketStatus.Confirmed,
                PaymentDate = DateTime.UtcNow
            };

            // Update seat status and add to ticket
            foreach (var seat in selectedSeats)
            {
                seat.Status = SeatStatus.Booked;
                ticket.TicketSeats.Add(new TicketSeat { BusSeat = seat });
            }

            // Update trip booked seats
            trip.BookedSeats += selectedSeats.Count;

            _context.Tickets.Add(ticket);
            await _context.SaveChangesAsync();

            var seatNumbers = selectedSeats.Select(s => s.SeatNumber).ToList();

            var ticketDto = new TicketDto
            {
                Id = ticket.Id,
                TicketNumber = ticket.TicketNumber,
                QRCode = ticket.QRCode,
                NumberOfSeats = ticket.NumberOfSeats,
                TotalPrice = ticket.TotalPrice,
                Status = ticket.Status.ToString(),
                SeatNumbers = seatNumbers
            };

            return CreatedAtAction(nameof(GetTicketById), new { id = ticket.Id },
                new ApiResponse<TicketDto>
                {
                    Success = true,
                    Message = "Ticket created successfully",
                    Data = ticketDto
                });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<TicketDto>>> GetTicketById(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userId, out var currentUserId))
                return Unauthorized();

            var ticket = await _context.Tickets
                .Include(t => t.Trip)
                    .ThenInclude(tr => tr!.BusCompany)
                .Include(t => t.TicketSeats)
                    .ThenInclude(ts => ts.BusSeat)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (ticket == null)
                return NotFound(new ApiResponse<TicketDto> { Success = false, Message = "Ticket not found" });

            // Check authorization
            if (ticket.UserId != currentUserId && !User.IsInRole("Admin"))
                return Forbid();

            var seatNumbers = ticket.TicketSeats.Select(ts => ts.BusSeat!.SeatNumber).ToList();

            var ticketDto = new TicketDto
            {
                Id = ticket.Id,
                TicketNumber = ticket.TicketNumber,
                QRCode = ticket.QRCode,
                NumberOfSeats = ticket.NumberOfSeats,
                TotalPrice = ticket.TotalPrice,
                Status = ticket.Status.ToString(),
                Trip = ticket.Trip != null ? new TripDto
                {
                    Id = ticket.Trip.Id,
                    BusCompanyId = ticket.Trip.BusCompanyId,
                    DepartureCity = ticket.Trip.DepartureCity,
                    ArrivalCity = ticket.Trip.ArrivalCity,
                    DepartureTime = ticket.Trip.DepartureTime,
                    ArrivalTime = ticket.Trip.ArrivalTime,
                    Price = ticket.Trip.Price,
                    TotalSeats = ticket.Trip.TotalSeats,
                    BookedSeats = ticket.Trip.BookedSeats,
                    BusCompany = ticket.Trip.BusCompany != null ? new BusCompanyDto
                    {
                        Id = ticket.Trip.BusCompany.Id,
                        Name = ticket.Trip.BusCompany.Name,
                        Logo = ticket.Trip.BusCompany.Logo,
                        Description = ticket.Trip.BusCompany.Description,
                        PhoneNumber = ticket.Trip.BusCompany.PhoneNumber,
                        Email = ticket.Trip.BusCompany.Email,
                        Address = ticket.Trip.BusCompany.Address,
                        IsActive = ticket.Trip.BusCompany.IsActive
                    } : null
                } : null,
                SeatNumbers = seatNumbers
            };

            return Ok(new ApiResponse<TicketDto>
            {
                Success = true,
                Data = ticketDto
            });
        }

        [HttpGet("my-tickets")]
        public async Task<ActionResult<ApiResponse<List<TicketDto>>>> GetMyTickets()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userId, out var id))
                return Unauthorized();

            var tickets = await _context.Tickets
                .Include(t => t.Trip)
                    .ThenInclude(tr => tr!.BusCompany)
                .Include(t => t.TicketSeats)
                    .ThenInclude(ts => ts.BusSeat)
                .Where(t => t.UserId == id && !t.IsDeleted)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();

            var ticketDtos = tickets.Select(t => new TicketDto
            {
                Id = t.Id,
                TicketNumber = t.TicketNumber,
                QRCode = t.QRCode,
                NumberOfSeats = t.NumberOfSeats,
                TotalPrice = t.TotalPrice,
                Status = t.Status.ToString(),
                Trip = t.Trip != null ? new TripDto
                {
                    Id = t.Trip.Id,
                    BusCompanyId = t.Trip.BusCompanyId,
                    DepartureCity = t.Trip.DepartureCity,
                    ArrivalCity = t.Trip.ArrivalCity,
                    DepartureTime = t.Trip.DepartureTime,
                    ArrivalTime = t.Trip.ArrivalTime,
                    Price = t.Trip.Price,
                    TotalSeats = t.Trip.TotalSeats,
                    BookedSeats = t.Trip.BookedSeats
                } : null,
                SeatNumbers = t.TicketSeats.Select(ts => ts.BusSeat!.SeatNumber).ToList()
            }).ToList();

            return Ok(new ApiResponse<List<TicketDto>>
            {
                Success = true,
                Data = ticketDtos
            });
        }

        [Authorize(Roles = "Manager")]
        [HttpPatch("{id}/confirm")]
        public async Task<IActionResult> ConfirmTicket(int id)
        {
            var ticket = await _context.Tickets.FindAsync(id);
            if (ticket == null)
                return NotFound(new ApiResponse<string> { Success = false, Message = "Ticket not found" });

            ticket.Status = TicketStatus.Used;
            ticket.BoardingDate = DateTime.UtcNow;
            ticket.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<string>
            {
                Success = true,
                Message = "Ticket confirmed successfully"
            });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> CancelTicket(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userId, out var currentUserId))
                return Unauthorized();

            var ticket = await _context.Tickets
                .Include(t => t.TicketSeats)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (ticket == null)
                return NotFound(new ApiResponse<string> { Success = false, Message = "Ticket not found" });

            // Check authorization
            if (ticket.UserId != currentUserId && !User.IsInRole("Admin"))
                return Forbid();

            // Update seat status back to available
            var seatIds = ticket.TicketSeats.Select(ts => ts.BusSeatId).ToList();
            var seats = await _context.BusSeats
                .Where(s => seatIds.Contains(s.Id))
                .ToListAsync();

            foreach (var seat in seats)
            {
                seat.Status = SeatStatus.Available;
            }

            // Update trip booked seats
            var trip = await _context.Trips.FindAsync(ticket.TripId);
            if (trip != null)
            {
                trip.BookedSeats -= ticket.NumberOfSeats;
            }

            ticket.IsDeleted = true;
            ticket.Status = TicketStatus.Cancelled;
            ticket.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<string>
            {
                Success = true,
                Message = "Ticket cancelled successfully"
            });
        }

        private string GenerateTicketNumber()
        {
            return $"SR{DateTime.UtcNow:yyyyMMddHHmmss}{new Random().Next(1000, 9999)}";
        }

        private string GenerateQRCode(string ticketNumber)
        {
            // In production, use a QR code library to generate actual QR code
            return $"https://smartride.vn/ticket/{ticketNumber}";
        }
    }
}
