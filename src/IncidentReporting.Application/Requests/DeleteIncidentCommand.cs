using MediatR;

namespace IncidentReporting.Application.Requests
{
    public record DeleteIncidentCommand(int Id, int UserId)
        : IRequest<bool>;
}
