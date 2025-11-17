using Xunit;
using FluentValidation.TestHelper;
using IncidentReporting.Application.DTOs;
using IncidentReporting.Application.Validators;
using IncidentReporting.Domain.Entities;

namespace IncidentReporting.UnitTests.Validators
{
    public class IncidentUpdateDtoValidatorTests
    {
        private readonly IncidentUpdateDtoValidator _validator;

        public IncidentUpdateDtoValidatorTests()
        {
            _validator = new IncidentUpdateDtoValidator();
        }

        [Fact]
        public void Should_Pass_When_Dto_Is_Valid()
        {
            var dto = new IncidentUpdateDto
            {
                Description = "Updated description",
                Status = IncidentStatus.InProgress
            };

            var result = _validator.TestValidate(dto);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Should_Fail_When_Description_Too_Long()
        {
            var dto = new IncidentUpdateDto
            {
                Description = new string('A', 1001),
                Status = IncidentStatus.Open
            };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Description);
        }

        [Fact]
        public void Should_Fail_When_Status_Is_Invalid()
        {
            var dto = new IncidentUpdateDto
            {
                Description = "Valid",
                Status = (IncidentStatus)999 // invalid enum
            };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Status);
        }

        [Fact]
        public void Should_Fail_When_Closing_Without_Resolution()
        {
            var dto = new IncidentUpdateDto
            {
                Description = "Something",
                Status = IncidentStatus.Closed,
                Resolution = null // required when closing
            };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Resolution)
                  .WithErrorMessage("Resolution is required when closing an incident.");
        }

        [Fact]
        public void Should_Pass_When_Closing_With_Resolution()
        {
            var dto = new IncidentUpdateDto
            {
                Description = "Something",
                Status = IncidentStatus.Closed,
                Resolution = "Issue fixed"
            };

            var result = _validator.TestValidate(dto);

            result.ShouldNotHaveValidationErrorFor(x => x.Resolution);
        }
    }
}
