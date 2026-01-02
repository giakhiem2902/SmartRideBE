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
    public class TripsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TripsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("search")]
        public async Task<ActionResult<ApiResponse<List<TripDto>>>> Search(
            string departureCity,
            string arrivalCity,
            DateTime date)
        {
            var trips = await _context.Trips
                .Include(t => t.BusCompany)
                .Where(t => !t.IsHidden && t.IsActive && !t.IsDeleted &&
                            t.DepartureCity == departureCity &&
                            t.ArrivalCity == arrivalCity &&
                            t.DepartureTime.Date == date.Date)
                .ToListAsync();

            var tripDtos = trips.Select(t => new TripDto
            {
                Id = t.Id,
                BusCompanyId = t.BusCompanyId,
                DepartureCity = t.DepartureCity,
                ArrivalCity = t.ArrivalCity,
                DepartureTime = t.DepartureTime,
                ArrivalTime = t.ArrivalTime,
                Price = t.Price,
                TotalSeats = t.TotalSeats,
                BookedSeats = t.BookedSeats,
                BusCompany = t.BusCompany != null ? new BusCompanyDto
                {
                    Id = t.BusCompany.Id,
                    Name = t.BusCompany.Name,
                    Logo = t.BusCompany.Logo,
                    Description = t.BusCompany.Description,
                    PhoneNumber = t.BusCompany.PhoneNumber,
                    Email = t.BusCompany.Email,
                    Address = t.BusCompany.Address,
                    IsActive = t.BusCompany.IsActive
                } : null
            }).ToList();

            return Ok(new ApiResponse<List<TripDto>>
            {
                Success = true,
                Message = "Trips found",
                Data = tripDtos
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<TripDto>>> GetById(int id)
        {
            var trip = await _context.Trips
                .Include(t => t.BusCompany)
                .FirstOrDefaultAsync(t => t.Id == id && !t.IsHidden && t.IsActive);

            if (trip == null)
                return NotFound(new ApiResponse<TripDto> { Success = false, Message = "Trip not found" });

            var tripDto = new TripDto
            {
                Id = trip.Id,
                BusCompanyId = trip.BusCompanyId,
                DepartureCity = trip.DepartureCity,
                ArrivalCity = trip.ArrivalCity,
                DepartureTime = trip.DepartureTime,
                ArrivalTime = trip.ArrivalTime,
                Price = trip.Price,
                TotalSeats = trip.TotalSeats,
                BookedSeats = trip.BookedSeats,
                BusCompany = trip.BusCompany != null ? new BusCompanyDto
                {
                    Id = trip.BusCompany.Id,
                    Name = trip.BusCompany.Name,
                    Logo = trip.BusCompany.Logo,
                    Description = trip.BusCompany.Description,
                    PhoneNumber = trip.BusCompany.PhoneNumber,
                    Email = trip.BusCompany.Email,
                    Address = trip.BusCompany.Address,
                    IsActive = trip.BusCompany.IsActive
                } : null
            };

            return Ok(new ApiResponse<TripDto>
            {
                Success = true,
                Data = tripDto
            });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<ApiResponse<TripDto>>> Create([FromBody] CreateTripDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<TripDto> { Success = false, Message = "Invalid data" });

            var bus = await _context.Buses.FindAsync(dto.BusId);
            if (bus == null)
                return BadRequest(new ApiResponse<TripDto> { Success = false, Message = "Bus not found" });

            var company = await _context.BusCompanies.FindAsync(dto.BusCompanyId);
            if (company == null)
                return BadRequest(new ApiResponse<TripDto> { Success = false, Message = "Company not found" });

            var trip = new Trip
            {
                BusId = dto.BusId,
                BusCompanyId = dto.BusCompanyId,
                DepartureCity = dto.DepartureCity,
                ArrivalCity = dto.ArrivalCity,
                DepartureTime = dto.DepartureTime,
                ArrivalTime = dto.ArrivalTime,
                Price = dto.Price,
                TotalSeats = bus.TotalSeats,
                IsActive = true
            };

            _context.Trips.Add(trip);
            await _context.SaveChangesAsync();

            var tripDto = new TripDto
            {
                Id = trip.Id,
                BusCompanyId = trip.BusCompanyId,
                DepartureCity = trip.DepartureCity,
                ArrivalCity = trip.ArrivalCity,
                DepartureTime = trip.DepartureTime,
                ArrivalTime = trip.ArrivalTime,
                Price = trip.Price,
                TotalSeats = trip.TotalSeats,
                BookedSeats = trip.BookedSeats
            };

            return CreatedAtAction(nameof(GetById), new { id = trip.Id },
                new ApiResponse<TripDto>
                {
                    Success = true,
                    Message = "Trip created successfully",
                    Data = tripDto
                });
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateTripDto dto)
        {
            var trip = await _context.Trips.FindAsync(id);
            if (trip == null)
                return NotFound(new ApiResponse<string> { Success = false, Message = "Trip not found" });

            trip.DepartureCity = dto.DepartureCity;
            trip.ArrivalCity = dto.ArrivalCity;
            trip.DepartureTime = dto.DepartureTime;
            trip.ArrivalTime = dto.ArrivalTime;
            trip.Price = dto.Price;
            trip.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<string>
            {
                Success = true,
                Message = "Trip updated successfully"
            });
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var trip = await _context.Trips.FindAsync(id);
            if (trip == null)
                return NotFound(new ApiResponse<string> { Success = false, Message = "Trip not found" });

            trip.IsDeleted = true;
            trip.IsActive = false;
            trip.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<string>
            {
                Success = true,
                Message = "Trip deleted successfully"
            });
        }

        [HttpGet("{tripId}/seats")]
        public async Task<ActionResult<ApiResponse<List<SeatDto>>>> GetSeats(int tripId)
        {
            var trip = await _context.Trips.Include(t => t.Bus).FirstOrDefaultAsync(t => t.Id == tripId);
            if (trip == null)
                return NotFound(new ApiResponse<List<SeatDto>> { Success = false, Message = "Trip not found" });

            var seats = await _context.BusSeats
                .Where(s => s.BusId == trip.BusId)
                .ToListAsync();

            var seatDtos = seats.Select(s => new SeatDto
            {
                Id = s.Id,
                SeatNumber = s.SeatNumber,
                Status = s.Status.ToString()
            }).ToList();

            return Ok(new ApiResponse<List<SeatDto>>
            {
                Success = true,
                Data = seatDtos
            });
        }
    }
}
