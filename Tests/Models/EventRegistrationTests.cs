using CampsiteBooking.Models;
using CampsiteBooking.Models.Common;
using CampsiteBooking.Models.ValueObjects;
using Xunit;

namespace CampsiteBooking.Tests.Models;

public class EventRegistrationTests
{
    private EventRegistration CreateValidRegistration(int numberOfParticipants = 2, string participantNames = "John Doe, Jane Doe")
    {
        return EventRegistration.Create(CampsiteBooking.Models.ValueObjects.EventId.Create(1), UserId.Create(1), numberOfParticipants, participantNames);
    }

    [Fact]
    public void EventRegistration_CanBeCreated_WithValidData()
    {
        var registration = CreateValidRegistration();
        Assert.NotNull(registration);
        Assert.Equal(2, registration.NumberOfParticipants);
        Assert.Equal("Confirmed", registration.Status);
    }

    [Fact]
    public void EventRegistration_Create_ThrowsException_WhenNumberOfParticipantsIsZero()
    {
        Assert.Throws<DomainException>(() => CreateValidRegistration(numberOfParticipants: 0));
    }

    [Fact]
    public void EventRegistration_Create_ThrowsException_WhenNumberOfParticipantsExceeds10()
    {
        Assert.Throws<DomainException>(() => CreateValidRegistration(numberOfParticipants: 11));
    }

    [Fact]
    public void EventRegistration_Create_ThrowsException_WhenParticipantNamesIsEmpty()
    {
        Assert.Throws<DomainException>(() => CreateValidRegistration(participantNames: ""));
    }

    [Fact]
    public void EventRegistration_Cancel_ChangesStatusToCancelled()
    {
        var registration = CreateValidRegistration();
        registration.Cancel();
        Assert.Equal("Cancelled", registration.Status);
    }

    [Fact]
    public void EventRegistration_Cancel_ThrowsException_WhenAlreadyCancelled()
    {
        var registration = CreateValidRegistration();
        registration.Cancel();
        Assert.Throws<DomainException>(() => registration.Cancel());
    }

    [Fact]
    public void EventRegistration_Confirm_ChangesStatusToConfirmed()
    {
        var registration = CreateValidRegistration();
        registration.Confirm();
        Assert.Equal("Confirmed", registration.Status);
    }

    [Fact]
    public void EventRegistration_UpdateSpecialRequests_UpdatesValue()
    {
        var registration = CreateValidRegistration();
        registration.UpdateSpecialRequests("Vegetarian meal");
        Assert.Equal("Vegetarian meal", registration.SpecialRequests);
    }
}
