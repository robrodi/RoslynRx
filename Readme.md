# Roslyn Rx
A test project where I want to play with Roslyn and Reactive Extensions.

## String based queries
Create string based filters and aggregations which are compiled into working Observable Queries.  I haven't yet gotten to removing compiled types altogether, but the properties used in the query don't need to be defined at compile time.

A where clause:

```csharp
var filter = new Filter<Event<long>>("@event => @event.Type == 1L && @event.Data > 30 && @event != null && @event.Data != 203123123");

var simpleObservable = Observable.Interval(TimeSpan.FromSeconds(1), scheduler)
                .Take(numberOfEvents)
                .Select(i => new Event<long>((int)i % numberOfTypes, i));

simpleObservable.Link(filter).Count().Subscribe(i => count = i).Subscribe(Console.Writeline);
```

An Rx scan:

```csharp
var summer = new Scan<Event<long>>("(sum, @event) => new RoslynRx.Tests.Event<long>(0, sum.Data + @event.Data)");
```

Demuxing a stream into smaller streams:
```csharp
    var dm = new Demuxer<int, Event<long>>(knownTypes, "@event => @event.Type");
    
    simpleObservable.Interval.Subscribe(dm);
    foreach (var stream in dm.Streams)
    {
        stream.Value.Count().Subscribe(Console.WriteLine);
    }
```

## Todo
*   Remove types
*   Allow output of different types (i.e. summing gives an integer, rather than an instance of the event).
* Composition

```csharp
    var simpleObservable = Observable.Interval(TimeSpan.FromSeconds(1), scheduler)
                .Take(someEvents)
                .Select(i => new Event<long>((int)i % 12, i));    
// Someday
    
    var query = @"
        where("@event => @event.Data > 3")        
        demux("@event => @event.Type", new[] { Type1, Type2, Type3 })
        count()
    ";
    simpleObservable.Link(new Query(query)).Subscribe();
    
    // yields
    //  1: 20 received
    //  2: 21 received
    //  3: 22 received
```