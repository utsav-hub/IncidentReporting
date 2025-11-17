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
            // Get incidents only for the specified user
            var incidents = await _repo.GetAllByUserIdAsync(request.UserId, ct);

            return incidents.Select(incident => new IncidentResponseDto
            {
                Id = incident.Id,
                Title = incident.Title,
                Description = incident.Description,
                CategoryId = incident.CategoryId,
                CategoryName = incident.Category?.Name,
                Status = incident.Status,
                Resolution = incident.Resolution,
                CreatedAt = incident.CreatedAt,
                UpdatedAt = incident.UpdatedAt
            }).ToList();
        }
    }
}
