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
    public class BusCompaniesController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public BusCompaniesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<List<BusCompanyDto>>>> GetAll()
        {
            var companies = await _context.BusCompanies
                .Where(c => !c.IsHidden && c.IsActive)
                .ToListAsync();

            var companyDtos = companies.Select(c => new BusCompanyDto
            {
                Id = c.Id,
                Name = c.Name,
                Logo = c.Logo,
                Description = c.Description,
                PhoneNumber = c.PhoneNumber,
                Email = c.Email,
                Address = c.Address,
                IsActive = c.IsActive
            }).ToList();

            return Ok(new ApiResponse<List<BusCompanyDto>>
            {
                Success = true,
                Message = "Companies retrieved successfully",
                Data = companyDtos
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<BusCompanyDto>>> GetById(int id)
        {
            var company = await _context.BusCompanies.FindAsync(id);
            if (company == null || company.IsHidden)
                return NotFound(new ApiResponse<BusCompanyDto> { Success = false, Message = "Company not found" });

            var companyDto = new BusCompanyDto
            {
                Id = company.Id,
                Name = company.Name,
                Logo = company.Logo,
                Description = company.Description,
                PhoneNumber = company.PhoneNumber,
                Email = company.Email,
                Address = company.Address,
                IsActive = company.IsActive
            };

            return Ok(new ApiResponse<BusCompanyDto>
            {
                Success = true,
                Data = companyDto
            });
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<ApiResponse<BusCompanyDto>>> Create([FromBody] CreateBusCompanyDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<BusCompanyDto> { Success = false, Message = "Invalid data" });

            var company = new BusCompany
            {
                Name = dto.Name,
                Logo = dto.Logo,
                Description = dto.Description,
                PhoneNumber = dto.PhoneNumber,
                Email = dto.Email,
                Address = dto.Address,
                IsActive = true
            };

            _context.BusCompanies.Add(company);
            await _context.SaveChangesAsync();

            var companyDto = new BusCompanyDto
            {
                Id = company.Id,
                Name = company.Name,
                Logo = company.Logo,
                Description = company.Description,
                PhoneNumber = company.PhoneNumber,
                Email = company.Email,
                Address = company.Address,
                IsActive = company.IsActive
            };

            return CreatedAtAction(nameof(GetById), new { id = company.Id }, 
                new ApiResponse<BusCompanyDto>
                {
                    Success = true,
                    Message = "Company created successfully",
                    Data = companyDto
                });
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateBusCompanyDto dto)
        {
            var company = await _context.BusCompanies.FindAsync(id);
            if (company == null)
                return NotFound(new ApiResponse<BusCompanyDto> { Success = false, Message = "Company not found" });

            company.Name = dto.Name;
            company.Logo = dto.Logo;
            company.Description = dto.Description;
            company.PhoneNumber = dto.PhoneNumber;
            company.Email = dto.Email;
            company.Address = dto.Address;
            company.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<string>
            {
                Success = true,
                Message = "Company updated successfully"
            });
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var company = await _context.BusCompanies.FindAsync(id);
            if (company == null)
                return NotFound(new ApiResponse<string> { Success = false, Message = "Company not found" });

            company.IsActive = false;
            company.IsHidden = true;
            company.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<string>
            {
                Success = true,
                Message = "Company deleted successfully"
            });
        }

        [Authorize(Roles = "Admin")]
        [HttpPatch("{id}/hide")]
        public async Task<IActionResult> Hide(int id)
        {
            var company = await _context.BusCompanies.FindAsync(id);
            if (company == null)
                return NotFound(new ApiResponse<string> { Success = false, Message = "Company not found" });

            company.IsHidden = !company.IsHidden;
            company.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return Ok(new ApiResponse<string>
            {
                Success = true,
                Message = company.IsHidden ? "Company hidden" : "Company shown"
            });
        }
    }
}
