# Lean Test Coding Patterns

This section is intended for collecting coding patterns that show how to handle different kinds of external dependencies - and more.

## Time as an External Dependency

It is a common coding pattern to have time dependencies simply represented by calling the static .NET `DateTime.UtcNow` in order to get the current time. This can be a small nuisance in tests, e.g. when asserting on expected outcome which contains time information. It's a bit more challenging when production code functionality depend on elapsed time, such as branching on short, long and very long durations. We want our tests to run fast, not to wait for whatever a *very long duration* amounts to. We also want to test tricky cases like passing midnight or a leap year without actually waiting for such events. Timers represent a special challenge, as they spin up threads at specified intervals; we want to put timer dependent production code under test without much waiting or synchronization.

In this example, we will walk through simple *time* dependencies as well as *timer* dependencies. The magic of the Lean Test builder pattern and `IMockForData` will ensure that this notion of time is consistent in both cases, across production and test code.

### The IDateTime Interface

The main trick is to treat *time* itself as an external dependency, so we have an `IDateTime` interface in our `ExternalDependencies` folder in production code. Simply injecting and using `IDateTime` in production code rather than using the static `DateTime` allows for L0 testing most time dependent functionality. This requires only a very minor change to production code.

The first step is to define an `IDateTime` interface and an implementation which wraps the static `DateTime`,

```csharp
public interface IDateTime
{
    DateTime UtcNow { get; }
    long Ticks { get; }
}

public class DateTimeMonoState : IDateTime
{
    public DateTime UtcNow => DateTime.UtcNow;
    public long Ticks => UtcNow.Ticks;
}
```

Note that we in this example have both `UtcNow` and `Ticks`, since that is exactly what our production code needs in this example. The interface `IDateTime` can be seen as a *façade* which represents exactly what the given client, our production code, needs.

The example production code we are going to use is a write cache which caches user settings.

The `IDateTime` interface is injected into constructors and then used
wherever it is needed, for example as follows,

```csharp
public UserSettingsWriteCache(IDateTime dateTime /*, ... */)
{
    _dateTime = dateTime;
    // ...  
}

public void Enqueue(UserSettingsPutCommand putCommand)
{
    putCommandCache.AddOrUpdate((putCommand.UserId, putCommand.AppId), putCommand, _dateTime.UtcNow);
    // ...  
}
```

The mock-for-data implementation is as simple as we are used to,

```csharp
public class MockForDataDateTime : IDateTime, IMockForData<DateTime>
{
    public DateTime UtcNow { get; private set; } = DateTime.UtcNow;
    public long Ticks => UtcNow.Ticks;
    public void WithData(DateTime data) => UtcNow = data;
    // ...  
}
```

We have chosen to give `UtcNow` a default value so that we only need to provide a value for test cases in which it will make a difference. For such a test, a simple `WithData` statement is all it takes to set e.g. the *Star Wars date 2020*,

```csharp
[TestMethod]
internal void PutMustUpdateWhenTimeBetweenCacheFlushesHasNotElapsedButDelayIsChangedToZero()
{
    DateTime startDateTime = new DateTime(2020, 5, 4);
    _contextBuilder
        .WithData(startDateTime)
        // ...  
        .Build();
    // ...  
}
```

### The ITimer Interface

Controlling time as shown above is very simple and useful, but it is a more interesting challenge when we have a *timer*.

In our example write cache, entries expire after a certain amount of time, the default being 20 seconds, and the production code must check for expired items to flush every second.

Needless to say, we don't want our tests to wait one, 20 or any other number of seconds when we test this functionality.

Again, the trick is to treat time itself as an external dependency.

Unfortunately, this is not what MS does with .NET, so we cannot inject `IDateTime`, or anything similar, into the standard timers.

Instead, we will wrap `System.Timers.Timer` similar to how we wrapped `DateTime`

```csharp
public interface ITimer
{
    event ElapsedEventHandler Elapsed;
    bool AutoReset { set; }
    bool Enabled { set; }
    void Dispose();
}
public interface ITimerFactory
{
    /// <summary></summary>
    ITimer CreateTimer(double interval);
}
internal class InternalTimer : Timer, ITimer
{
    internal class TimerFactory : ITimerFactory
    {
        public ITimer CreateTimer(double interval) => new InternalTimer(interval);
    }

    private InternalTimer(double interval) : base(interval) {}
}
```

There is a small, but important difference here - we have provided a factory class to create the timer, because it is common practice to create a timer on demand, not injecting it. By injecting a factory class, we are back to testable code again. Here is one example that shows how to create and initialize a timer,

```csharp
public UserSettingsWriteCache(ITimerFactory timerFactory /*, ... */)
{
    _checkForExpiredTimer = timerFactory.CreateTimer(MillisecondsBetweenCheckForExpired);
    _checkForExpiredTimer.Elapsed += FlushExpiredCommands;
    _checkForExpiredTimer.AutoReset = true;
    _checkForExpiredTimer.Enabled = true;
// ...
}
```

The timer mock is relatively simple, but there are a couple of important and useful twists to it,

```csharp
internal class MockTimer : ITimer
{
    private readonly TimeSpan _interval;
    private DateTime _dateTime;

    /// <summary>We inject the timer factory, not the timer, so the factory must implement IMockForData and pass data on.</summary>
    internal class TimerFactory : ITimerFactory, IMockForData<DateTime>
    {
        private MockTimer _timerMock; // We assume that only a single instance is created; change this if there are more than one in the production code!

        public ITimer CreateTimer(double interval) => _timerMock = new MockTimer(interval);
        public void WithData(DateTime data) => _timerMock.WithData(data);
        public void PreBuild() {}
        public void Build(Type type) {}
        public void PostBuild() {}
    }

    private MockTimer(double interval) => _interval = TimeSpan.FromMilliseconds(interval);

    public event ElapsedEventHandler Elapsed;
    public bool AutoReset { get; set; }
    public bool Enabled { get; set; }
    public void Dispose() {}

    private void WithData(DateTime data) // Calling this means "time has passed, the current time is now ..."
    {
        if (data - _dateTime > _interval)
            Elapsed?.Invoke(null, null);
        _dateTime = data;
    }
}
```

The first twist is that since we inject the factory, it is the factory which is known to the DI container not the timer itself, so we implement IMockForData on the factory and delegate to the timer. The timer implementation is instantiated and used by the factory, not the DI container.

The second twist is the fact that the timer gets its notion of time from IDateTime - this is *the same time as any other piece of production or test code* in our solution.

Occasionally, the above mock implementation will be too simplistic. As hinted in a comment, you will often need several timer instances. Also, if timer event handlers depend on time, through the IDateTime interface, you need to ensure that the time mock is updated with the current time before timer handlers are executed. There is no guarantee that mocks have their builder methods called in any specific order, so the trick is to implement both the ITimerFactory and the IDateTime interfaces in the same class. This is how the mock below implements these requirements,

```csharp
internal class MockTimer : ITimer
{
    private readonly TimeSpan _interval;
    private DateTime _invokedAt;
    private DateTime _currentTime;

    /// <summary>We inject the timer factory, not the timer, so the factory must implement IMockForData and pass data on.</summary>
    /// <remarks>We implement time (IClock) here in order to ensure that timer handlers and the timers perceive time consistently.</remarks>
    internal class TimerFactory : ITimerFactory, IMockForData<DateTime>, IDateTime
    {
        private readonly IList<MockTimer> _timerMocks = new List<MockTimer>();
        private DateTime _rawUtcNow;

        public ITimer CreateTimer(double interval)
        {
            var theTimer = new MockTimer(interval);
            _timerMocks.Add(theTimer);

            return theTimer;
        }

        public void WithData(DateTime data)
        {
            _rawUtcNow = data;
            foreach (MockTimer timerMock in _timerMocks)
                timerMock.WithData(data);
        }
        public void PreBuild() { }
        public void Build(Type type) { }
        public void PostBuild()
        {
            UtcNow = _rawUtcNow;
            foreach (MockTimer timerMock in _timerMocks)
                timerMock.PostBuild();
        }

        public DateTime UtcNow { get; private set; }
    }

    private MockTimer(double interval) => _interval = TimeSpan.FromMilliseconds(interval);

    public event ElapsedEventHandler Elapsed;
    public bool AutoReset { get; set; }
    public bool Enabled { get; set; }
    public void Dispose() { }
    public void Stop() { }

    /// <summary>Calling this means "time has passed, the current time is now ..." </summary>
    private void WithData(DateTime data) => _currentTime = data;

    private void PostBuild()
    {
        if (_currentTime == default)
            return;
        if (_invokedAt == default)
            _invokedAt = _currentTime;
        if (_currentTime - _invokedAt < _interval)
            return;

        _invokedAt = _currentTime;
        Elapsed?.Invoke(null, null);
    }
}
```

The meaning of this is that whenever we write test code like the following,

```csharp
[TestMethod]
public void PutMustUpdateWhenTimeBetweenCacheFlushesHasNotElapsedButDelayIsChangedToZero()
{
    DateTime startDateTime = new DateTime(2020, 5, 4);
    _contextBuilder
        .WithData(startDateTime)
        // ...  
        .Build();
    // ...  
}
```

... then it is both the `IDateTime` mock and the `ITimer` mock which gets the required time.

This is important when we want to test advanced timing related functionality, in which both the current time and elapsed time combined determine expected and required behaviour.

Let's say our test scenario is,

- Enqueue a change to the write cache.
- Before the default time between cache flushes elapse, check that the     write cache has not been flushed, i.e. the change has not been written.
- Let more than the default time between cache flushes elapse.
- Check that the change has been written.

It is straightforward to write such a test as well as similar tests. Here is an example of the above,

```csharp
[TestMethod]
public void PutMustUpdateWhenTimeBetweenCacheFlushesHasElapsed()
{
    DateTime startDateTime = DateTime.UtcNow;
    _contextBuilder
        .WithData<UserSettingsRow>(TestData.UserSettingsRows.User1InitialSettings)
        .WithData(startDateTime)
        .Build();
    string pre = _reader.Query(new UserSettingsQuery { UserId = 1, AppId = 42, Path = "text" });

    _target.PutUserSettings(new JValue("New value."), "text");

    string afterPutButBeforeExpiry = _reader.Query(new UserSettingsQuery { UserId = 1, AppId = 42, Path = "text" });

    _contextBuilder.WithClearDataStore()
        .WithData(startDateTime + TimeSpan.FromMilliseconds(UserSettingsWriteCache.DefaultMillisecondsBetweenCacheFlushes + 1))
        .Build();

    string actual = _reader.Query(new UserSettingsQuery {UserId = 1, AppId = 42, Path = "text" });
    MultiAssert.Aggregate(
        () => Assert.AreEqual(@"""This is a string.""", pre),
        () => Assert.AreEqual(@"""This is a string.""", afterPutButBeforeExpiry),
        () => Assert.AreEqual(@"""New value.""", actual));
}
```

Note the way we control time simply by passing it in `WithData` statements. First, we set the initial time, which will be passed to the mocks at the time of the first `Build`.

We execute the command, `PutUserSettings`, then check if the change has been written while *time effectively has stopped*. We are in the Matrix here.

The next `WithData` and `Build()` sets the time to 1 ms after the cache delay. Note the `WithClearDataStore` statement which instructs Lean Test not to pass all the initial with-data again.

This example has been boiled down from a slightly more advanced test which also changes the cache delay and asserts on log entries. Rest assured that the more advanced test is as simple and straightforward as the one described above.
