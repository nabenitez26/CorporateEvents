using CorporateEvents.Abstractions.Events;

namespace CorporateEvents.SampleApi.Events
{
    public class PolicyIssuedEvent : EventBase
    {
        public required string PolicyNumber { get; init; }
        public required string CustomerName { get; init; }
        public required string Email { get; init; }
        public required string Phone { get; init; }
    }
}
