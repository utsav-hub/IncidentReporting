using MediatR;
using IncidentReporting.Application.DTOs;
using IncidentReporting.Application.Interfaces;
using IncidentReporting.Application.Requests;

namespace IncidentReporting.Application.Handlers
{
    public class GetAllIncidentsHandler : IRequestHandler<GetAllIncidentsQuery, List<IncidentResponseDto>>
    {
        private readonly IIncidentRepository _repo;

        public GetAllIncidentsHandler(IIncidentRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<IncidentResponseDto>> Handle(GetAllIncidentsQuery request, CancellationToken ct)
        {
            var incidents = await _repo.GetAllAsync(ct);

            return incidents.Select(incident => new IncidentResponseDto
            {
                Id = incident.Id,
                Title = incident.Title,
                Description = incident.Description,
                Status = incident.Status,
                Resolution = incident.Resolution,
                CreatedAt = incident.CreatedAt,
                UpdatedAt = incident.UpdatedAt
            }).ToList();
        }
    }
}
