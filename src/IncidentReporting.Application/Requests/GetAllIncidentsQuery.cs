using MediatR;
using IncidentReporting.Application.DTOs;

namespace IncidentReporting.Application.Requests
{
    public record GetAllIncidentsQuery()
        : IRequest<List<IncidentResponseDto>>;
}
