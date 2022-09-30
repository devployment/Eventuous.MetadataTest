// See https://aka.ms/new-console-template for more information

using EventStore.Client;
using Eventuous;
using Eventuous.EventStore;
using EventuousMetadata;

var store = new EsdbEventStore(EventStoreClientSettings.Create("esdb://localhost:2113?tls=false"));
var aggregateStore = new AggregateStore(store, null);

//This expects Dummy-2 to be available
var qualification = await aggregateStore.Load<Dummy>(StreamName.For<Dummy>("2"), CancellationToken.None);

Console.WriteLine(qualification.State);