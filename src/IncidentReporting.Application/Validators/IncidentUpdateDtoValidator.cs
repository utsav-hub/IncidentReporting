using FluentValidation;
using IncidentReporting.Application.DTOs;
using IncidentReporting.Domain.Entities;

namespace IncidentReporting.Application.Validators
{
    public class IncidentUpdateDtoValidator : AbstractValidator<IncidentUpdateDto>
    {
        public IncidentUpdateDtoValidator()
        {
            RuleFor(x => x.Description)
                .MaximumLength(1000);

            RuleFor(x => x.Status)
                .IsInEnum()
                .WithMessage("Invalid status value.");

            When(x => x.Status == IncidentStatus.Closed, () =>
            {
                RuleFor(x => x.Resolution)
                    .NotEmpty().WithMessage("Resolution is required when closing an incident.")
                    .MaximumLength(1000);
            });
        }
    }
}
