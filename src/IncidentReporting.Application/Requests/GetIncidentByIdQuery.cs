using MediatR;
using IncidentReporting.Application.DTOs;

namespace IncidentReporting.Application.Requests
{
    public record GetIncidentByIdQuery(int Id, int UserId)
        : IRequest<IncidentResponseDto?>;
}
