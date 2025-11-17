using MediatR;
using IncidentReporting.Application.DTOs;

namespace IncidentReporting.Application.Requests
{
    public record UpdateIncidentCommand(int Id, IncidentUpdateDto Dto, int UserId)
        : IRequest<IncidentResponseDto?>;
}
