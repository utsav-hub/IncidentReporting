using MediatR;
using IncidentReporting.Application.DTOs;

namespace IncidentReporting.Application.Requests
{
    public record GetAllIncidentsQuery(int UserId)
        : IRequest<List<IncidentResponseDto>>;
}
