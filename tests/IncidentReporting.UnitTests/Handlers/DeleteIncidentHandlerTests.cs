using Xunit;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using IncidentReporting.Application.Handlers;
using IncidentReporting.Application.Interfaces;
using IncidentReporting.Application.Requests;
using IncidentReporting.Domain.Entities;

namespace IncidentReporting.UnitTests.Handlers
{
    public class DeleteIncidentHandlerTests
    {
        private readonly Mock<IIncidentRepository> _repoMock;
        private readonly DeleteIncidentHandler _handler;

        public DeleteIncidentHandlerTests()
        {
            _repoMock = new Mock<IIncidentRepository>();
            _handler = new DeleteIncidentHandler(_repoMock.Object);
        }

        [Fact]
        public async Task Handle_Should_Return_False_If_Incident_Not_Found()
        {
            // Arrange
            _repoMock.Setup(r => r.GetAsync(111, It.IsAny<CancellationToken>()))
                     .ReturnsAsync((Incident?)null);

            var command = new DeleteIncidentCommand(111);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result);
            _repoMock.Verify(r => r.DeleteAsync(It.IsAny<Incident>(), It.IsAny<CancellationToken>()), Times.Never);
            _repoMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task Handle_Should_Delete_Existing_Incident()
        {
            // Arrange
            var incident = new Incident("Test", "Desc");
            typeof(Incident).GetProperty("Id")!.SetValue(incident, 1);

            _repoMock.Setup(r => r.GetAsync(1, It.IsAny<CancellationToken>()))
                     .ReturnsAsync(incident);

            var command = new DeleteIncidentCommand(1);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result);

            // Repository calls
            _repoMock.Verify(r => r.DeleteAsync(incident, It.IsAny<CancellationToken>()), Times.Once);
            _repoMock.Verify(r => r.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
