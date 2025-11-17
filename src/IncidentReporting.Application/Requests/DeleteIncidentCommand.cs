using MediatR;

namespace IncidentReporting.Application.Requests
{
    public record DeleteIncidentCommand(int Id)
        : IRequest<bool>;
}
