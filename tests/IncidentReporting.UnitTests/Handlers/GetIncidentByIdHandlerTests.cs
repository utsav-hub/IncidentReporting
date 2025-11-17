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
    public class GetIncidentByIdHandlerTests
    {
        private readonly Mock<IIncidentRepository> _repoMock;
        private readonly GetIncidentByIdHandler _handler;

        public GetIncidentByIdHandlerTests()
        {
            _repoMock = new Mock<IIncidentRepository>();
            _handler = new GetIncidentByIdHandler(_repoMock.Object);
        }

        [Fact]
        public async Task Handle_Should_Return_Null_When_Not_Found()
        {
            // Arrange
            _repoMock.Setup(r => r.GetAsync(100, It.IsAny<CancellationToken>()))
                     .ReturnsAsync((Incident?)null);

            var query = new GetIncidentByIdQuery(100);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.Null(result);

            _repoMock.Verify(r => r.GetAsync(100, It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_Return_IncidentResponse_When_Found()
        {
            // Arrange
            var incident = new Incident("Server Down", "Critical outage");
            typeof(Incident).GetProperty("Id")!.SetValue(incident, 1);

            _repoMock.Setup(r => r.GetAsync(1, It.IsAny<CancellationToken>()))
                     .ReturnsAsync(incident);

            var query = new GetIncidentByIdQuery(1);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("Server Down", result.Title);
            Assert.Equal("Critical outage", result.Description);
            Assert.Equal(IncidentStatus.Open, result.Status);

            _repoMock.Verify(r => r.GetAsync(1, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
