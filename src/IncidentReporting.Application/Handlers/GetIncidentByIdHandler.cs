using MediatR;
using IncidentReporting.Application.DTOs;
using IncidentReporting.Application.Interfaces;
using IncidentReporting.Application.Requests;

namespace IncidentReporting.Application.Handlers
{
    public class GetIncidentByIdHandler : IRequestHandler<GetIncidentByIdQuery, IncidentResponseDto?>
    {
        private readonly IIncidentRepository _repo;

        public GetIncidentByIdHandler(IIncidentRepository repo)
        {
            _repo = repo;
        }

        public async Task<IncidentResponseDto?> Handle(GetIncidentByIdQuery request, CancellationToken ct)
        {
            // Get incident only if it belongs to the user
            var incident = await _repo.GetAsync(request.Id, request.UserId, ct);

            if (incident == null)
                return null;

            return new IncidentResponseDto
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
            };
        }
    }
}
