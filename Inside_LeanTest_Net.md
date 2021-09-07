# Inside LeanTest.Net

The Lean Testing methodology is backed up by a simple library, a set of nuGet packages named [LeanTest.Net](https://github.com/SaxoBank/Leantest).

LeanTest.Net is little more than an implementation of a Builder pattern. In fact, the functionality of LeanTest.Net is so simple that it could almost serve as a morning coding kata for an experienced developer.

You can use LeanTest.Net if you understand that a test essentially *declares* data which is *passed* as initial context to external dependencies.

However, it can still be useful to understand what goes on under the hood, as it makes it more obvious how to write simple and elegant tests, and also to reason about what functionality existing tests cover.

This text is aimed at those who want to understand what is going on inside LeanTest.Net.

## The Builder Pattern

Let\'s assume that we want to write tests targeting a piece of software which has an external dependency represented by the interface `IMyExternalService`.

With Lean Testing, we mock as little as possible and when we do mock, we always mock at the boundary of external dependencies. We write tests in terms of the relevant state of such external dependencies.

Thus, in this case, it makes sense to have a mock implement `IMyExternalService`, then pass relevant state to it in each test.

In essence, all that our Builder pattern does is to pass data from a test to a mock via an internal *context data store,*

| Test *arrange* part  | Context data store   | Mock (the *receiver*) |
|----------------------|----------------------|-----------------------|
| _contextBuilder<br>.WithData(new<br>**My**())<br>.Build();      | **My**: \<one instance\> | internal class MockForDataMy : IMockForData<**My**>, IMyExternalService<br>{<br>public voidWithData(**My** data) { }<br>public void Build(Type type) {}<br>// ...<br>} |

> It is core to Lean testing that we pass data between a test and a mock this way.
>
> We do not simply instantiate an instance of a mock and feed it with context data, as that would tightly couple tests with mocks. By avoiding such a tight coupling, we can write tests which emphasize *what* is being tested, rather than *how* it is being tested. This not only means that test code is not coupled to a specific mock implementation, it also means that the test code does not depend on a mock being implemented at all. The test in the example above declares that it depends on having a My data in the context, but the test does not care whether the data is passed to a mock or a database.
>
> *Declaring* data needed for a test, without explicitly stating *how* that data is supposed to be used, is what allows us to write tests in one consistent way, with any kind of mocking strategy, running in-process or out-of-process, mocking a database or not, running with immediate external dependencies or a full environment.
>
> In short, there is one consistent way to write tests at any level and there is one consistent way to reason about functional test coverage.
>

Note that the test uses the *type* of the instance of data passed to the mock in order to decide the receiver, i.e. in order to identify the mock which will receive the data. We expect representations of external dependencies to use strong typing.

Also note how a mock declares what type of data it is prepared to receive simply by implementing the interface `IMockForData<>`. `Build()` on the context builder will pass data of type `T` from the internal data store by *asking the DI container* for implementations of `IMockForData<T>`. LeanTest.Net will never use reflection to discover receivers of data, ***it will rely on receivers to be registered with the DI container and tests to declare relevant data***.

### Multiple Receivers

In the example above we had a piece of data which was passed to a single mock. It is not always that simple; in fact, ***the possibility of having multiple receivers with a consistent view of test data is one major benefit of our flavour of the Builder pattern***.

One example usage of multiple receivers could be a test which declares that the financial Forex instrument EURUSD must be known and available. The external dependencies could be a number of backend services which each is responsible for handling a single aspect of that instrument (such as pricing, margin calculations etc.). We would expect that if one of these know about EURUSD, then the others will also.

Another example is when *time itself* is an external dependency. If the code under test has logic around the current time and also run code regularly on a thread as spun up by a timer, then you would expect such code to have the same notion of time. In other words, if a test declares that the current time is *Star Wars day 2020*, then you expect that both the current time and the timer logic would reflect that.

Treating time as an external dependency is a coding pattern which is documented TODO, the example functionality being a write-cache which flushes at regular times. From the details of the documented code pattern it follows that a single `DateTime` instance declared in a test will be passed to the two
relevant mocks,

| Test arrange part                                            | Context data store             | Mock                                                         |
| ------------------------------------------------------------ | ------------------------------ | ------------------------------------------------------------ |
| \_contextBuilder<br>.WithData(new **DateTime**(2020, 5, 4))<br>.Build(); | **DateTime**: \<one instance\> | internal class MockForDataDateTime : IDateTime, IMockForData\<**DateTime**\><br>\{<br>public void WithData(**DateTime** data) { }<br>public void Build(Type type) { }<br>// ...<br>}<br><br>internal class MockForDataTimer : ITimer<br>\{<br/>public void WithData(**DateTime** data) { }<br>public void Build(Type type) { }<br>// ...<br/>}<br/> |

### Multiple Builders

In the examples above we passed data to mocks. It is not always that simple; actually, ***not hardcoding that data is passed to any given mock is one major benefit of our flavour of the Builder pattern***.

LeanTest.Net\'s Builder pattern supports multiple builders. At the time of writing, a *mock-for-data builder* and a *state handler builder* are provided out-of-the-box.

The difference between these two builders is only conceptual. In
short,

- The *mock-for-data builder* passes data to mock implementations. Mock implementations *substitute production code* by implementing one or more interfaces, such as `IMyExternalDependency`, along with `IMockForData<T>` for all relevant data types `T`.
- The *state handler builder* passes data to state handler implementations. State handler implementations manage state of direct external dependencies by implementing  `IStateHandler<T>` for all relevant data types `T`. A state handler will never substitute production code.

Since mocks substitute production code, we will mock as little as possible, thereby putting as much code under test as possible. And when we mock, we will try to not mock away any production code which is relevant to put under test. Key to this is to define façade interfaces TODO: link representing external dependencies; these interfaces are defined such that a production code implementation contains no logic, only pass-through to the actual external dependency.

State handlers are typically used to handle data in database(s), but can also be used to handle other state, such as data in a distributed cache or a file system. In either case, a state handler must take full responsibility for handling the data. For a SQL database, that would involve deleting/inserting data in a way which respects referential integrity. Naturally, such destructive behaviour assumes ownership of the database.

A special use of state handlers is for shared environments, in which destructive actions are not *come-il-faut*. The best we can do in such an environment is to be explicit about our assumptions, checking and ensuring whenever possible. Tests may still fail for all the reasons which tests fail in shared environments, but at least we can more easily determine the assumption(s) that fail and do corrective actions immediately.

Naturally, the principle of having multiple receivers applies across multiple builders. Returning to an earlier example, a test could declare that EURUSD must be known and available; this instrument can then be passed to a mock implementation as well as to a table in the test target\'s own database.

## The Data Life-Cycle

In the examples above, we have declared data in tests using `WithData`, then sent data to mocks and state handlers, via the context store and the builders.

There is obviously a connection between `WithData`/`Build` in a test, and `WithData`/`Build` in mocks and state handlers, and it is quite possible
to write tests without understanding the finer details of that connection.

However, for those who want to dig a bit deeper in order to excel at writing tests, here are the finer details.

The interfaces for mocks and state handlers, `IMockForData<T>` and `IStateHandler<T>` implement `PreBuild` and `PostBuild` in addition to `WithData` and `Build`,

```csharp
/// <summary>Declare data of type <c>T</c>.</summary>
void WithData(T data);
/// <summary>Called before build only once for the instance, allows you to prepare to populate state.</summary>
void PreBuild();
/// <summary>Use the declared data to populate state, called after all data of type <c>type</c> has been put to the instance with <c>WithData</c>.</summary>
void Build(Type type);
/// <summary>Called after build, only once for the instance.</summary>
void PostBuild();
```

We implement either `IMockForData<T>` or `IStateHandler<T>`, but not both in a single class.

It is often a good idea to implement either of the interfaces for several types `T`~1~, `T`~2~, ... `T`~n~ in a single class. For example, a state handler which handles data in a SQL database, will be able to insert and delete records corresponding to `T`~1~, `T`~2~, \... `T`~n~ respecting relational constraints in the database.

In general, the data life-cycle is,

- Calling `WithData` in a test will add data to the internal context data store; no data is passed to any mock or state handler.
- Calling `Build` in a test will initiate a sequence of calls which will pass the data in the internal context data store to relevant mocks and state handlers. For each instance which implements either `IMockForData<T`~1~`>`, `IMockForData<T`~2~`>`, ... , `IMockForData<T`~n~`>` or `IStateHandler<T`~1~`>`, `IStateHandler<T`~2~`>`, ...,  `IStateHandler<T`~n~`>`,
  - `PreBuild` is called once, allowing you to prepare to use the data which is coming. This allows you to delete any `T`~1~, `T`~2~, ... `T`~n~ data stored in the instance, so that it is entirely clean. A state handler which handles data in a SQL database will delete all records, a mock will clear any internal data structures.
    - `WithData` is called once per piece of data in the internal context data store, for each type `T`~1~, `T`~2~, ... `T`~n~ of data. This is where you would store the data in your own internal data structures in the mock or state handler.
    - `Build` is called once per type `T`~1~, `T`~2~, ... `T`~n~ of data in the internal context data store. This is where you would commit the data from the `WithData` phase if you need to do that per data type. If on the other hand, you do not need per-data type processing, or perhaps you require cross-data type processing, then you will do your processing in `PostBuild` instead.
    - `PostBuild` is called once, allowing you to clean-up. This is where you would do cross-data type work, such as storing data in a relational database, assuming that there are relational constraints. This is also where you would dispose resources needed during the earlier phases.

Note that the above text describes the full flexibility of the data life-cycle, whereas most mock and state handler implementations will be much simpler.

Most implementations will simply receive one or more pieces of data which are returned by methods of a mocked interface. In such cases, `PreBuild` will be a no-op, `WithData` will be a single line (an assignment), and both `Build` and `PostBuild` will be no-ops. Here is the full implementation of the time mock mentioned above (and described in details TODO: link)

- `WithData` stores a `DateTime` instance and the two methods of the `IDateTime` mocked interface are implemented simply by returning that instance or calling `Ticks` on it respectively,

```csharp
public class MockForDataDateTime : IDateTime, IMockForData<DateTime>
{
    // The two methods of the IDateTime mocked interface:
    public DateTime UtcNow { get; private set; } = DateTime.UtcNow;
    public long Ticks => UtcNow.Ticks;

    public void WithData(DateTime data) => UtcNow = data;

    public void PreBuild() {} // Can be omitted entirely.
    public void Build(Type type) {} // Can be omitted entirely.
    public void PostBuild() {} // Can be omitted entirely.
}
```

## Multiple Build Calls

Most tests call `WithData` multiple times, then call `Build` once on a context builder. This is because most test cases are simple enough that a single call to Build is sufficient.

But the entire data life-cycle is traversed every time a test calls Build and this allows us to handle more advanced test cases.

Some examples of advanced cases involve testing of time based functionality. Such tests will typically not be simple AAA tests, with a single *arrange*, a single *act* and a single *assert*. Rather, it is common to set the state of the test target, perhaps do a bit of acting, change the current time, act a bit more etc.

Again, we can use our write-cache TODO: link as an example. We arrange data, *act* in a way that puts data in the cache, *assert* that the cache has not been flushed (as no time has passed yet), then *arrange* that time passes past the flush delay, then finally *assert* that the cache has flushed as expected.

Here is the test using the extended AAA,

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

Note that with the magic of `MultiAssert` we put all assert statements at the end of the test.

Also note the use of `WithClearDataStore`, which clears all data which was passed in `WithData` statements in the first arrange block. The effect of this is that only data from the following `WithData` statement will be passed in the data life-cycle, so the `UserSettingsRow` above will not be passed again. In other words, `WithData` in mocks and state handlers will not be called again for the type `UserSettingsRow`. However, `PreBuild` and `PostBuild` will still be called, since the data types are known to LeanTest.Net.

This is important to bear in mind when using multiple build calls, as it might influence the details of how mocks and state handlers must be implemented.

## Dependency Injection - IoC Container Integration

LeanTest.Net only knows about mocks and state handlers that you explicitly expose to it. This is the only way; LeanTest.Net will never magically find mocks and state handlers using reflection. This is what we call the *no magic* principle.

You expose your preferred IoC container by implementing a simple interface (as described TODO: link,

```csharp
public interface IIocContainer
{
    T Resolve<T>() where T : class;
    T TryResolve<T>() where T : class;
    IEnumerable<T> TryResolveAll<T>() where T : class;
}
```

Your implementation is passed to LeanTest.Net initialization *once per test suite* as described TODO: link. Note that it is wrapped in a factory which ensures that a new IoC container instance, along with new mock and state handler instances, are created *before each test is run*.

The consequence of this is that ***each test will start with an empty context data store, empty (i.e., no data passed yet) mocks and state handlers***.

It also means that the builders for mocks and state handlers are *empty* in the sense that they do not know of any types for which to call `PreBuild`/`Build`/`PostBuild` on mocks and state handlers yet.

This is a consequence of our *no magic* principle, which is exactly what is needed in most cases.

## Tweaking the Data Life-Cycle

At rare occasions you may wish to have slightly more low-level control of the steps in the data life-cycle for certain data types.

We have already mentioned `WithClearDataStore`, which you use when you want to avoid passing data again to mocks and state handlers in subsequent calls to `Build`. This way, you control how `WithData` and `Build` is called on mocks and state handlers.

The data life-cycle also controls when `PreBuild` and `PostBuild` are called; in short, these will be called on instances which handle a type for which data has been declared. For example, if data for the type `T`~1~ has been declared, then `PreBuild` and `PostBuild` will be called on implementations of `IMockForData<T`~1~`>` and `IStateHandler<T`~1~`>`. However, if you don't have data for `T`~2~ in a given test, but still want `PreBuild` and `PostBuild` to be called on implementations of `IMockForData<T`~2~`>` and `IStateHandler<T`~2~`>`, then you simply declare empty `T`~2~ data,

```csharp
 _contextBuilder
    .WithData<T2>()
// ... 
```

**Experimental, not implemented yet**: With the example above in mind, you may wish to go the opposite way; you may want the builders to forget about the type `T`~1~, effectively stopping them from calling `PreBuild` and `PostBuild` on implementations of `IMockForData<T`~1~`>` and `IStateHandler<T`~1~`>`. For this you use `WithClearBuilders`.

```csharp
 _contextBuilder
    .WithClearBuilders()
// ... 
```

Note that `WithClearBuilders` imply `WithClearDataStore`. Also note that `WithClearBuilders` and `WithClearDataStore` are never needed until the second call to `Build` on the context builder, as a test will always start with a clear data store and clear builders.
