using Microsoft.AspNetCore.Mvc;
using IncidentReporting.Application.DTOs;
using IncidentReporting.Application.Interfaces;

namespace IncidentReporting.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        // POST: api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            // Debug: Check if DTO is being received correctly
            if (loginDto == null)
            {
                return BadRequest(new { message = "Request body is required" });
            }

            if (string.IsNullOrWhiteSpace(loginDto.Username) || string.IsNullOrWhiteSpace(loginDto.Password))
            {
                return BadRequest(new { message = "Username and password are required" });
            }

            var result = await _authService.LoginAsync(loginDto);

            if (result == null)
            {
                return Unauthorized(new { message = "Invalid username or password" });
            }

            return Ok(result);
        }

        // POST: api/auth/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
        {
            if (string.IsNullOrWhiteSpace(registerDto.Username) || 
                string.IsNullOrWhiteSpace(registerDto.Email) || 
                string.IsNullOrWhiteSpace(registerDto.Password))
            {
                return BadRequest(new { message = "Username, email, and password are required" });
            }

            if (registerDto.Password.Length < 6)
            {
                return BadRequest(new { message = "Password must be at least 6 characters long" });
            }

            var result = await _authService.RegisterAsync(registerDto);

            if (result == null)
            {
                return Conflict(new { message = "Username or email already exists" });
            }

            return Ok(result);
        }
    }
}

