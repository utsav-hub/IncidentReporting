using IncidentReporting.Application.DTOs;
using IncidentReporting.Domain.Entities;

namespace IncidentReporting.Application.Interfaces
{
    public interface IAuthService
    {
        Task<AuthResponseDto?> LoginAsync(LoginDto loginDto, CancellationToken cancellationToken = default);
        Task<AuthResponseDto?> RegisterAsync(RegisterDto registerDto, CancellationToken cancellationToken = default);
        string GenerateJwtToken(User user);
    }
}

