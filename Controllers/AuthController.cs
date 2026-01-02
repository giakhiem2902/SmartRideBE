using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SmartRideBackend.DTOs;
using SmartRideBackend.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SmartRideBackend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IConfiguration _configuration;

        public AuthController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequestDto model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();
                Console.WriteLine($"ModelState errors: {string.Join(", ", errors)}");
                return BadRequest(new AuthResponseDto 
                { 
                    Success = false, 
                    Message = "Invalid data",
                    Errors = errors
                });
            }

            if (model.Password != model.ConfirmPassword)
                return BadRequest(new AuthResponseDto { Success = false, Message = "Passwords do not match" });

            var user = new ApplicationUser
            {
                UserName = model.UserName,
                Email = model.Email,
                PhoneNumber = model.PhoneNumber,
                FullName = model.FullName
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                return BadRequest(new AuthResponseDto
                {
                    Success = false,
                    Message = "Registration failed",
                    Errors = result.Errors.Select(e => e.Description).ToList()
                });
            }

            // Assign User role by default
            await _userManager.AddToRoleAsync(user, "User");

            return Ok(new AuthResponseDto
            {
                Success = true,
                Message = "User registered successfully"
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return Unauthorized(new AuthResponseDto { Success = false, Message = "Invalid credentials" });

            var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);
            if (!result.Succeeded)
                return Unauthorized(new AuthResponseDto { Success = false, Message = "Invalid credentials" });

            var token = GenerateJwtToken(user);
            var roles = await _userManager.GetRolesAsync(user);

            return Ok(new AuthResponseDto
            {
                Success = true,
                Message = "Login successful",
                Token = token,
                User = new UserDto
                {
                    Id = user.Id,
                    UserName = user.UserName ?? string.Empty,
                    Email = user.Email ?? string.Empty,
                    FullName = user.FullName,
                    PhoneNumber = user.PhoneNumber,
                    Avatar = user.Avatar,
                    Roles = roles
                }
            });
        }

        [Authorize]
        [HttpGet("profile")]
        public async Task<IActionResult> GetProfile()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userId, out var id))
                return Unauthorized();

            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
                return NotFound();

            var roles = await _userManager.GetRolesAsync(user);

            return Ok(new ApiResponse<UserDto>
            {
                Success = true,
                Data = new UserDto
                {
                    Id = user.Id,
                    UserName = user.UserName ?? string.Empty,
                    Email = user.Email ?? string.Empty,
                    FullName = user.FullName,
                    PhoneNumber = user.PhoneNumber,
                    Avatar = user.Avatar,
                    Roles = roles
                }
            });
        }

        private string GenerateJwtToken(ApplicationUser user)
        {
            var jwtSettings = _configuration.GetSection("Jwt");
            var secretKey = Encoding.ASCII.GetBytes(jwtSettings["SecretKey"] ?? "default-secret-key");

            var tokenHandler = new JwtSecurityTokenHandler();
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.UserName ?? string.Empty),
                new Claim(ClaimTypes.Email, user.Email ?? string.Empty)
            };

            // Add roles to JWT token
            var roles = _userManager.GetRolesAsync(user).GetAwaiter().GetResult();
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(double.Parse(jwtSettings["ExpirationMinutes"] ?? "60")),
                Issuer = jwtSettings["Issuer"],
                Audience = jwtSettings["Audience"],
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(secretKey),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
