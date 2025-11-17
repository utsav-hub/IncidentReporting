using MediatR;
using IncidentReporting.Application.DTOs;

namespace IncidentReporting.Application.Requests
{
    public record CreateIncidentCommand(IncidentCreateDto Dto)
        : IRequest<IncidentResponseDto>;
}
