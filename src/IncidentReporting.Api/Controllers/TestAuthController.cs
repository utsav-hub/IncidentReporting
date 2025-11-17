using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using IncidentReporting.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace IncidentReporting.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestAuthController : ControllerBase
    {
        private readonly AppDbContext _context;

        public TestAuthController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/testauth/users
        [HttpGet("users")]
        public async Task<IActionResult> GetUsers()
        {
            var users = await _context.Users
                .Select(u => new
                {
                    u.Id,
                    u.Username,
                    u.Email,
                    u.IsActive,
                    PasswordHashLength = u.PasswordHash.Length,
                    CreatedAt = u.CreatedAt
                })
                .ToListAsync();

            return Ok(users);
        }

        // POST: api/testauth/test-password
        [HttpPost("test-password")]
        public IActionResult TestPassword([FromBody] TestPasswordDto dto)
        {
            if (string.IsNullOrEmpty(dto.Password))
            {
                return BadRequest(new { message = "Password is required" });
            }

            var hash = HashPassword(dto.Password);
            return Ok(new
            {
                originalPassword = dto.Password,
                passwordLength = dto.Password.Length,
                hash = hash,
                hashLength = hash.Length
            });
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }
    }

    public class TestPasswordDto
    {
        public string Password { get; set; } = string.Empty;
    }
}

