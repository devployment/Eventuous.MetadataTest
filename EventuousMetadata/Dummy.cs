using Eventuous;

namespace EventuousMetadata;

public class Dummy : Aggregate<DummyState>
{
}

public record DummyState : AggregateState<DummyState>
{
    public long JustSomething { get; set; }
    
    public DummyState()
    {
        On<DummyEvents.V1.DummyEventHappened>(HandleDummyEvent);
    }
    
    static DummyState HandleDummyEvent(DummyState state, DummyEvents.V1.DummyEventHappened evt)
        => state with
        {
            JustSomething = evt.JustSomething
        };
}

public static class DummyEvents
{
    public static class V1
    {
        [EventType("V1.DummyEventHappened")]
        public record DummyEventHappened(
            long JustSomething
        );
    }
}