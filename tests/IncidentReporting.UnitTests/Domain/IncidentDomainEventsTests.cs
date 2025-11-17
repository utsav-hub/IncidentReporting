using Xunit;
using IncidentReporting.Domain.Entities;
using IncidentReporting.Domain.DomainEvents;

namespace IncidentReporting.UnitTests.Domain
{
    public class IncidentDomainEventsTests
    {
        [Fact]
        public void Close_Should_Raise_IncidentClosedEvent()
        {
            // Arrange
            var incident = new Incident("Test", "Test Description", userId: 1);
            typeof(Incident).GetProperty("Id")!.SetValue(incident, 100);

            // Act
            incident.StartProgress();               // Valid transition
            incident.Close("Issue resolved");       // Should raise domain event

            var events = incident.DomainEvents;

            // Assert
            Assert.Single(events);
            Assert.IsType<IncidentClosedEvent>(events.First());

            var evt = events.First() as IncidentClosedEvent;

            Assert.Equal(100, evt!.IncidentId);
            Assert.Equal("Issue resolved", evt.Resolution);
            Assert.True(evt.ClosedAt <= DateTime.UtcNow);
        }

        [Fact]
        public void PopDomainEvents_Should_Clear_Events()
        {
            var incident = new Incident("Test", "Test Description", userId: 1);
            typeof(Incident).GetProperty("Id")!.SetValue(incident, 200);

            // Fire Close → adds domain event
            incident.StartProgress();
            incident.Close("Fixed");

            var eventsBeforePop = incident.DomainEvents;

            Assert.Single(eventsBeforePop);

            // Act
            var popped = incident.PopDomainEvents();

            // Assert
            Assert.Single(popped);
            Assert.Empty(incident.DomainEvents); // events cleared after pop
        }

        [Fact]
        public void StartProgress_Should_Not_Raise_Event()
        {
            var incident = new Incident("Test", "Test Description", userId: 1);

            incident.StartProgress();

            Assert.Empty(incident.DomainEvents);
        }

        [Fact]
        public void Reopen_Should_Not_Raise_Event_By_Default()
        {
            var incident = new Incident("Test", "Desc", userId: 1);

            incident.StartProgress();
            incident.Close("Done");

            incident.Reopen();

            // Close raised ONE event
            Assert.Single(incident.DomainEvents);

            // Reopen does not raise an event (per your design)
        }
    }
}
