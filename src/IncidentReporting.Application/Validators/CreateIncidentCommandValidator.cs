using FluentValidation;
using IncidentReporting.Application.Requests;

namespace IncidentReporting.Application.Validators
{
    public class CreateIncidentCommandValidator : AbstractValidator<CreateIncidentCommand>
    {
        public CreateIncidentCommandValidator()
        {
            RuleFor(x => x.Dto)
                .NotNull();

            RuleFor(x => x.Dto.Title)
                .NotEmpty()
                .MaximumLength(200);

            RuleFor(x => x.Dto.Description)
                .MaximumLength(1000);
        }
    }
}
