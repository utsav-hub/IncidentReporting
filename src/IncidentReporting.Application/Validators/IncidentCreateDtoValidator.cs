using FluentValidation;
using IncidentReporting.Application.DTOs;

namespace IncidentReporting.Application.Validators
{
    public class IncidentCreateDtoValidator : AbstractValidator<IncidentCreateDto>
    {
        public IncidentCreateDtoValidator()
        {
            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(200);

            RuleFor(x => x.Description)
                .MaximumLength(1000);
        }
    }
}
