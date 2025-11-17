using Xunit;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using IncidentReporting.Application.Handlers;
using IncidentReporting.Application.Interfaces;
using IncidentReporting.Application.DTOs;
using IncidentReporting.Application.Requests;
using IncidentReporting.Domain.Entities;

namespace IncidentReporting.UnitTests.Handlers
{
    public class CreateIncidentHandlerTests
    {
        private readonly Mock<IIncidentRepository> _repoMock;
        private readonly CreateIncidentHandler _handler;

        public CreateIncidentHandlerTests()
        {
            _repoMock = new Mock<IIncidentRepository>();
            _handler = new CreateIncidentHandler(_repoMock.Object);
        }

        [Fact]
        public async Task Handle_Should_Create_New_Incident()
        {
            // Arrange
            var dto = new IncidentCreateDto
            {
                Title = "Network Down",
                Description = "Cannot access internet"
            };

            var command = new CreateIncidentCommand(dto, UserId: 1);

            // Setup repo to assign Id when adding
            _repoMock.Setup(r => r.AddAsync(It.IsAny<Incident>(), It.IsAny<CancellationToken>()))
                     .Callback<Incident, CancellationToken>((incident, ct) =>
                     {
                         // Simulate EF Core assigning ID
                         typeof(Incident)
                             .GetProperty("Id")!
                             .SetValue(incident, 1);
                     });

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("Network Down", result.Title);
            Assert.Equal("Cannot access internet", result.Description);
            Assert.Equal(IncidentStatus.Open, result.Status);

            // Verify repository calls
            _repoMock.Verify(r => r.AddAsync(It.IsAny<Incident>(), It.IsAny<CancellationToken>()), Times.Once);
            _repoMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
