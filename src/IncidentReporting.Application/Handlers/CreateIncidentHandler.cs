using MediatR;
using IncidentReporting.Application.DTOs;
using IncidentReporting.Application.Interfaces;
using IncidentReporting.Application.Requests;
using IncidentReporting.Domain.Entities;

namespace IncidentReporting.Application.Handlers
{
    public class CreateIncidentHandler : IRequestHandler<CreateIncidentCommand, IncidentResponseDto>
    {
        private readonly IIncidentRepository _repo;
        private readonly INotificationService _notificationService;

        public CreateIncidentHandler(IIncidentRepository repo, INotificationService notificationService)
        {
            _repo = repo;
            _notificationService = notificationService;
        }

        public async Task<IncidentResponseDto> Handle(CreateIncidentCommand request, CancellationToken ct)
        {
            var dto = request.Dto;

            // Create incident with user ID
            var incident = new Incident(dto.Title, dto.Description, request.UserId, dto.CategoryId);

            await _repo.AddAsync(incident, ct);
            await _repo.SaveChangesAsync(ct);

            // Create notification for the user
            await _notificationService.CreateNotificationAsync(
                userId: request.UserId,
                title: "Incident Created Successfully",
                message: $"Your incident '{incident.Title}' has been created and is now being tracked.",
                type: "Success",
                incidentId: incident.Id
            );

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
