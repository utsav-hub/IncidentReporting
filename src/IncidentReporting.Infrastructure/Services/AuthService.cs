using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using IncidentReporting.Application.DTOs;
using IncidentReporting.Application.Interfaces;
using IncidentReporting.Domain.Entities;
using IncidentReporting.Infrastructure.Data;

namespace IncidentReporting.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthService(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<AuthResponseDto?> LoginAsync(LoginDto loginDto, CancellationToken cancellationToken = default)
        {
            // Trim and normalize username
            var normalizedUsername = loginDto.Username?.Trim();
            var password = loginDto.Password ?? string.Empty;
            
            if (string.IsNullOrWhiteSpace(normalizedUsername) || string.IsNullOrWhiteSpace(password))
                return null;

            // Get all active users and compare in memory (SQLite case-insensitive comparison)
            var users = await _context.Users
                .Where(u => u.IsActive)
                .ToListAsync(cancellationToken);

            var user = users.FirstOrDefault(u => 
                string.Equals(u.Username, normalizedUsername, StringComparison.OrdinalIgnoreCase));

            if (user == null)
                return null;

            // Verify password - hash the input password and compare
            var inputHash = HashPassword(password);
            if (inputHash != user.PasswordHash)
                return null;

            var token = GenerateJwtToken(user);

            return new AuthResponseDto
            {
                Token = token,
                Username = user.Username,
                Email = user.Email,
                ExpiresAt = DateTime.UtcNow.AddHours(24) // Token expires in 24 hours
            };
        }

        public async Task<AuthResponseDto?> RegisterAsync(RegisterDto registerDto, CancellationToken cancellationToken = default)
        {
            // Normalize input
            var normalizedUsername = registerDto.Username?.Trim();
            var normalizedEmail = registerDto.Email?.Trim().ToLower();
            var password = registerDto.Password ?? string.Empty;

            if (string.IsNullOrWhiteSpace(normalizedUsername) || string.IsNullOrWhiteSpace(normalizedEmail) || string.IsNullOrWhiteSpace(password))
                return null;

            // Check if username or email already exists (case-insensitive)
            // Load all users and compare in memory for case-insensitive comparison
            var allUsers = await _context.Users.ToListAsync(cancellationToken);
            var existingUser = allUsers.FirstOrDefault(u => 
                string.Equals(u.Username, normalizedUsername, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(u.Email, normalizedEmail, StringComparison.OrdinalIgnoreCase));

            if (existingUser != null)
                return null;

            // Create new user - hash password exactly as received
            var passwordHash = HashPassword(password);
            var user = new User
            {
                Username = normalizedUsername,
                Email = normalizedEmail,
                PasswordHash = passwordHash,
                FirstName = registerDto.FirstName?.Trim(),
                LastName = registerDto.LastName?.Trim(),
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync(cancellationToken);

            var token = GenerateJwtToken(user);

            return new AuthResponseDto
            {
                Token = token,
                Username = user.Username,
                Email = user.Email,
                ExpiresAt = DateTime.UtcNow.AddHours(24)
            };
        }

        public string GenerateJwtToken(User user)
        {
            var jwtSettings = _configuration.GetSection("JwtSettings");
            var secretKey = jwtSettings["SecretKey"] ?? "YourSuperSecretKeyThatShouldBeAtLeast32CharactersLongForProduction!";
            var issuer = jwtSettings["Issuer"] ?? "IncidentReportingApi";
            var audience = jwtSettings["Audience"] ?? "IncidentReportingClient";
            var expirationMinutes = int.Parse(jwtSettings["ExpirationMinutes"] ?? "1440"); // 24 hours

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expirationMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }

        private bool VerifyPassword(string password, string passwordHash)
        {
            if (string.IsNullOrEmpty(password) || string.IsNullOrEmpty(passwordHash))
                return false;
                
            var hashOfInput = HashPassword(password);
            return hashOfInput == passwordHash;
        }
    }
}

