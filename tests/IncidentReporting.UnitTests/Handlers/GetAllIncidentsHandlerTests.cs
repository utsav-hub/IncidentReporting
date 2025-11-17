using Xunit;
using Moq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using IncidentReporting.Application.Handlers;
using IncidentReporting.Application.Interfaces;
using IncidentReporting.Application.Requests;
using IncidentReporting.Application.DTOs;
using IncidentReporting.Domain.Entities;

namespace IncidentReporting.UnitTests.Handlers
{
    public class GetAllIncidentsHandlerTests
    {
        private readonly Mock<IIncidentRepository> _repoMock;
        private readonly GetAllIncidentsHandler _handler;

        public GetAllIncidentsHandlerTests()
        {
            _repoMock = new Mock<IIncidentRepository>();
            _handler = new GetAllIncidentsHandler(_repoMock.Object);
        }

        [Fact]
        public async Task Handle_Should_Return_Empty_List_When_No_Incidents()
        {
            // Arrange
            _repoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                     .ReturnsAsync(new List<Incident>());

            var query = new GetAllIncidentsQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);

            _repoMock.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_Return_All_Incidents()
        {
            // Arrange
            var incident1 = new Incident("Server Down", "Critical failure");
            typeof(Incident).GetProperty("Id")!.SetValue(incident1, 1);

            var incident2 = new Incident("Email Issue", "Cannot send emails");
            typeof(Incident).GetProperty("Id")!.SetValue(incident2, 2);

            var list = new List<Incident> { incident1, incident2 };

            _repoMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                     .ReturnsAsync(list);

            var query = new GetAllIncidentsQuery();

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);

            Assert.Contains(result, x => x.Id == 1 && x.Title == "Server Down");
            Assert.Contains(result, x => x.Id == 2 && x.Title == "Email Issue");

            _repoMock.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}
