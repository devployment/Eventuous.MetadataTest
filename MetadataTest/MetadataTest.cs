using System.Text;
using EventStore.Client;
using Eventuous;
using Eventuous.EventStore;
using EventuousMetadata;
using Newtonsoft.Json;

namespace MetadataTest;

public class MetadataTest
{
    private EventStoreClient _esdbClient;
    private readonly AggregateStore _aggregateStore;
    private readonly long _streamId;
    private readonly string _streamName;
    private readonly DummyEvents.V1.DummyEventHappened _dummyEvent;
    private readonly EventTypeAttribute? _dummyEventTypeAttribute;

    public MetadataTest()
    {
        var settings = EventStoreClientSettings.Create("esdb://localhost:2113?tls=false");
        _esdbClient = new EventStoreClient(settings);

        var store = new EsdbEventStore(EventStoreClientSettings.Create("esdb://localhost:2113?tls=false"));
        _aggregateStore = new AggregateStore(store);

        _streamId = DateTime.Now.ToFileTimeUtc();
        _streamName = $"Dummy-{_streamId}";

        _dummyEvent = new DummyEvents.V1.DummyEventHappened(_streamId);
        _dummyEventTypeAttribute = Attribute.GetCustomAttribute(_dummyEvent.GetType(), typeof(EventTypeAttribute)) as EventTypeAttribute;
    }

    [Fact]
    public async Task AggregateState_NoMetadataGiven_ShouldNotFail()
    {
        var eventDataWithoutMetadata = new EventData(
            Uuid.FromGuid(Guid.NewGuid()),
            _dummyEventTypeAttribute.EventType,
            new ReadOnlyMemory<byte>(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(_dummyEvent)))
        );

        //Add event by regular ESDB client to proof our scenario
        var list = new List<EventData> {eventDataWithoutMetadata};
        await _esdbClient.AppendToStreamAsync(_streamName, StreamState.NoStream, list);

        var qualification =
            await _aggregateStore.Load<Dummy>(StreamName.For<Dummy>(_streamId.ToString()), CancellationToken.None);

        Assert.Equal(qualification.State.JustSomething, _streamId);
    }

    [Fact]
    public async Task AggregateState_MetadataGiven_ShouldNotFail()
    {
        var defaultMetadata = Encoding.UTF8.GetBytes("{}");

        var eventDataWithoutMetadata = new EventData(
            Uuid.FromGuid(Guid.NewGuid()),
            _dummyEventTypeAttribute.EventType,
            new ReadOnlyMemory<byte>(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(_dummyEvent))),
            defaultMetadata
        );

        //Add event by regular ESDB client to proof our scenario
        var list = new List<EventData> {eventDataWithoutMetadata};
        await _esdbClient.AppendToStreamAsync(_streamName, StreamState.NoStream, list);

        var qualification =
            await _aggregateStore.Load<Dummy>(StreamName.For<Dummy>(_streamId.ToString()), CancellationToken.None);

        Assert.Equal(qualification.State.JustSomething, _streamId);
    }
}