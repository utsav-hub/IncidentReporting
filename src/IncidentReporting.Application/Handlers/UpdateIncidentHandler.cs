using MediatR;
using IncidentReporting.Application.DTOs;
using IncidentReporting.Application.Interfaces;
using IncidentReporting.Application.Requests;
using IncidentReporting.Domain.Entities;

namespace IncidentReporting.Application.Handlers
{
    public class UpdateIncidentHandler : IRequestHandler<UpdateIncidentCommand, IncidentResponseDto?>
    {
        private readonly IIncidentRepository _repo;

        public UpdateIncidentHandler(IIncidentRepository repo)
        {
            _repo = repo;
        }

        public async Task<IncidentResponseDto?> Handle(UpdateIncidentCommand request, CancellationToken ct)
        {
            var incident = await _repo.GetAsync(request.Id, ct);

            if (incident == null)
                return null;

            var dto = request.Dto;

            // Workflow operations based on status
            switch (dto.Status)
            {
                case IncidentStatus.Open:
                    incident.Reopen();
                    break;

                case IncidentStatus.InProgress:
                    incident.StartProgress();
                    break;

                case IncidentStatus.Closed:
                    incident.Close(dto.Resolution ?? string.Empty);
                    break;
            }

            // Update fields
            incident.UpdateDetails(dto.Description);

            await _repo.UpdateAsync(incident, ct);
            await _repo.SaveChangesAsync(ct);

            return new IncidentResponseDto
            {
                Id = incident.Id,
                Title = incident.Title,
                Description = incident.Description,
                Status = incident.Status,
                Resolution = incident.Resolution,
                CreatedAt = incident.CreatedAt,
                UpdatedAt = incident.UpdatedAt
            };
        }
    }
}
