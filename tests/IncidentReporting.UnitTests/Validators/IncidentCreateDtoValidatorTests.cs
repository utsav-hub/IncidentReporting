using Xunit;
using FluentValidation.TestHelper;
using IncidentReporting.Application.DTOs;
using IncidentReporting.Application.Validators;

namespace IncidentReporting.UnitTests.Validators
{
    public class IncidentCreateDtoValidatorTests
    {
        private readonly IncidentCreateDtoValidator _validator;

        public IncidentCreateDtoValidatorTests()
        {
            _validator = new IncidentCreateDtoValidator();
        }

        [Fact]
        public void Should_Pass_When_Dto_Is_Valid()
        {
            var dto = new IncidentCreateDto
            {
                Title = "Valid Title",
                Description = "Valid Description"
            };

            var result = _validator.TestValidate(dto);

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Fact]
        public void Should_Fail_When_Title_Is_Empty()
        {
            var dto = new IncidentCreateDto
            {
                Title = "",
                Description = "Something"
            };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Title)
                  .WithErrorMessage("Title is required.");
        }

        [Fact]
        public void Should_Fail_When_Title_Exceeds_Max_Length()
        {
            var dto = new IncidentCreateDto
            {
                Title = new string('A', 201) // max is 200
            };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Title);
        }

        [Fact]
        public void Should_Fail_When_Description_Exceeds_Max_Length()
        {
            var dto = new IncidentCreateDto
            {
                Title = "Valid",
                Description = new string('B', 1001) // max is 1000
            };

            var result = _validator.TestValidate(dto);

            result.ShouldHaveValidationErrorFor(x => x.Description);
        }
    }
}
