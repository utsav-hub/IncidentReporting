using FluentValidation;
using IncidentReporting.Application.Requests;
using IncidentReporting.Domain.Entities;

namespace IncidentReporting.Application.Validators
{
    public class UpdateIncidentCommandValidator : AbstractValidator<UpdateIncidentCommand>
    {
        public UpdateIncidentCommandValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0);

            RuleFor(x => x.Dto)
                .NotNull();

            RuleFor(x => x.Dto.Status)
                .IsInEnum();

            RuleFor(x => x.Dto.Description)
                .MaximumLength(1000);

            When(x => x.Dto.Status == IncidentStatus.Closed, () =>
            {
                RuleFor(x => x.Dto.Resolution)
                    .NotEmpty()
                    .WithMessage("Resolution is required when closing an incident.");
            });
        }
    }
}
