using Xunit;
using IncidentReporting.Domain.Entities;
using System;

namespace IncidentReporting.UnitTests.Domain
{
    public class IncidentStateMachineTests
    {
        [Fact]
        public void New_Incident_Should_Start_In_Open_State()
        {
            var incident = new Incident("Test", "Description", userId: 1);

            Assert.Equal(IncidentStatus.Open, incident.Status);
        }

        [Fact]
        public void Should_Move_From_Open_To_InProgress()
        {
            var incident = new Incident("Test", "Description", userId: 1);

            incident.StartProgress();

            Assert.Equal(IncidentStatus.InProgress, incident.Status);
        }

        [Fact]
        public void Should_Move_From_InProgress_To_Closed()
        {
            var incident = new Incident("Test", "Description", userId: 1);

            incident.StartProgress();       // Open -> InProgress
            incident.Close("Resolved");     // InProgress -> Closed

            Assert.Equal(IncidentStatus.Closed, incident.Status);
            Assert.Equal("Resolved", incident.Resolution);
        }

        [Fact]
        public void Should_Reopen_From_Closed()
        {
            var incident = new Incident("Test", "Description", userId: 1);

            incident.StartProgress();
            incident.Close("Done");
            incident.Reopen();

            Assert.Equal(IncidentStatus.Open, incident.Status);
        }

        [Fact]
        public void Should_Throw_On_Invalid_Transition_Open_To_Closed_Without_Closing()
        {
            var incident = new Incident("Test", "Description", userId: 1);

            // ❗ Directly firing Close is valid in your configuration
            // because Open → Closed is permitted.
            // So we test an ACTUAL invalid path:
            // e.g., Closed → InProgress (invalid)

            incident.StartProgress();
            incident.Close("Done");

            Assert.ThrowsAny<Exception>(() =>
            {
                // Closed → StartProgress is NOT permitted
                incident.StartProgress();
            });
        }

        [Fact]
        public void Should_Update_UpdatedAt_When_State_Changes()
        {
            var incident = new Incident("Test", "Description", userId: 1);

            var before = incident.UpdatedAt;

            incident.StartProgress();

            Assert.NotNull(incident.UpdatedAt);
            Assert.NotEqual(before, incident.UpdatedAt);
        }
    }
}
