using MediatR;
using IncidentReporting.Application.Interfaces;
using IncidentReporting.Application.Requests;

namespace IncidentReporting.Application.Handlers
{
    public class DeleteIncidentHandler : IRequestHandler<DeleteIncidentCommand, bool>
    {
        private readonly IIncidentRepository _repo;

        public DeleteIncidentHandler(IIncidentRepository repo)
        {
            _repo = repo;
        }

        public async Task<bool> Handle(DeleteIncidentCommand request, CancellationToken ct)
        {
            var incident = await _repo.GetAsync(request.Id, ct);

            if (incident == null)
                return false;

            await _repo.DeleteAsync(incident, ct);
            await _repo.SaveChangesAsync(ct);

            return true;
        }
    }
}
