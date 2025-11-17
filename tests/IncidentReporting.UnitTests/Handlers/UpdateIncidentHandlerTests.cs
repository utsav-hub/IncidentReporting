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
    public class UpdateIncidentHandlerTests
    {
        private readonly Mock<IIncidentRepository> _repoMock;
        private readonly UpdateIncidentHandler _handler;

        public UpdateIncidentHandlerTests()
        {
            _repoMock = new Mock<IIncidentRepository>();
            _handler = new UpdateIncidentHandler(_repoMock.Object);
        }

        [Fact]
        public async Task Handle_Should_Return_Null_If_Incident_Not_Found()
        {
            // Arrange
            _repoMock.Setup(r => r.GetAsync(999, It.IsAny<CancellationToken>()))
                     .ReturnsAsync((Incident?)null);

            var command = new UpdateIncidentCommand(
                999,
                new IncidentUpdateDto { Description = "test", Status = IncidentStatus.Open }
            );

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task Handle_Should_Update_Description()
        {
            // Arrange
            var incident = new Incident("Old Title", "Old Description");

            typeof(Incident).GetProperty("Id")!.SetValue(incident, 1);

            _repoMock.Setup(r => r.GetAsync(1, It.IsAny<CancellationToken>()))
                     .ReturnsAsync(incident);

            var updateDto = new IncidentUpdateDto
            {
                Description = "Updated Description",
                Status = IncidentStatus.Open
            };

            var command = new UpdateIncidentCommand(1, updateDto);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Updated Description", result.Description);

            _repoMock.Verify(r => r.UpdateAsync(incident, It.IsAny<CancellationToken>()), Times.Once);
            _repoMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_Start_Progress()
        {
            // Arrange
            var incident = new Incident("Test", "Desc");
            typeof(Incident).GetProperty("Id")!.SetValue(incident, 2);

            _repoMock.Setup(r => r.GetAsync(2, It.IsAny<CancellationToken>()))
                     .ReturnsAsync(incident);

            var dto = new IncidentUpdateDto
            {
                Status = IncidentStatus.InProgress
            };

            var command = new UpdateIncidentCommand(2, dto);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(IncidentStatus.InProgress, result.Status);
        }

        [Fact]
        public async Task Handle_Should_Close_Incident()
        {
            // Arrange
            var incident = new Incident("Test", "Desc");
            typeof(Incident).GetProperty("Id")!.SetValue(incident, 3);

            _repoMock.Setup(r => r.GetAsync(3, It.IsAny<CancellationToken>()))
                     .ReturnsAsync(incident);

            var dto = new IncidentUpdateDto
            {
                Status = IncidentStatus.Closed,
                Resolution = "Issue Fixed"
            };

            var command = new UpdateIncidentCommand(3, dto);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(IncidentStatus.Closed, result.Status);
            Assert.Equal("Issue Fixed", result.Resolution);

            // Ensure update and save were called
            _repoMock.Verify(r => r.UpdateAsync(incident, It.IsAny<CancellationToken>()), Times.Once);
            _repoMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_Reopen_Incident()
        {
            // Arrange
            var incident = new Incident("Test", "Desc");
            typeof(Incident).GetProperty("Id")!.SetValue(incident, 4);

            // Move to closed state first
            incident.Close("done");

            _repoMock.Setup(r => r.GetAsync(4, It.IsAny<CancellationToken>()))
                     .ReturnsAsync(incident);

            var dto = new IncidentUpdateDto
            {
                Status = IncidentStatus.Open
            };

            var command = new UpdateIncidentCommand(4, dto);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(IncidentStatus.Open, result.Status);
        }
    }
}
