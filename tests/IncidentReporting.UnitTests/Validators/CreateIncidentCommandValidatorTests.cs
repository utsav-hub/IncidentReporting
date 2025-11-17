using Xunit;
using FluentValidation.TestHelper;
using IncidentReporting.Application.DTOs;
using IncidentReporting.Application.Requests;
using IncidentReporting.Application.Validators;

namespace IncidentReporting.UnitTests.Validators
{
    public class CreateIncidentCommandValidatorTests
    {
        private readonly CreateIncidentCommandValidator _validator;

        public CreateIncidentCommandValidatorTests()
        {
            _validator = new CreateIncidentCommandValidator();
        }

        [Fact]
        public void Should_Pass_When_Command_Is_Valid()
        {
            var dto = new IncidentCreateDto
            {
                Title = "Valid Title",
                Description = "Some description"
            };

            var command = new CreateIncidentCommand(dto);

            var result = _validator.TestValidate(command);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Should_Fail_When_Dto_Is_Null()
        {
            var command = new CreateIncidentCommand(null!);

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.Dto);
        }

        [Fact]
        public void Should_Fail_When_Title_Is_Empty()
        {
            var dto = new IncidentCreateDto
            {
                Title = "",
                Description = "Desc"
            };

            var command = new CreateIncidentCommand(dto);

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.Dto.Title);
        }

        [Fact]
        public void Should_Fail_When_Title_Is_Too_Long()
        {
            var dto = new IncidentCreateDto
            {
                Title = new string('A', 201)
            };

            var command = new CreateIncidentCommand(dto);

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.Dto.Title);
        }

        [Fact]
        public void Should_Fail_When_Description_Too_Long()
        {
            var dto = new IncidentCreateDto
            {
                Title = "Valid",
                Description = new string('B', 1001) // limit 1000
            };

            var command = new CreateIncidentCommand(dto);

            var result = _validator.TestValidate(command);

            result.ShouldHaveValidationErrorFor(x => x.Dto.Description);
        }
    }
}
