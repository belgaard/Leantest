# LeanTest
A shared library used to simplify the process of creating and maintaining simple and consistent developer tests across projects.

## Deployment
The latest AppVeyor build [![Build status](https://ci.appveyor.com/api/projects/status/gd05aw9aslc3kgbq/branch/master?svg=true)](https://ci.appveyor.com/project/belgaard/leantest/branch/master) (from the latest push to master on GitHub) is available on myGet.org,
 - https://www.myget.org/F/belgaard-ci/api/v3/index.json

Note that v0.13.* LeanTest is built on .Net Standard 1.0. By using the very lowest common denominator it is possible to use LeanTest on all platforms dupported by .Net Standard.

The latest version will be built on at least .Net Standard 2.0, in order to avoid the many supporting dependencies needed by 1.0.

Note that the versions built so far start with _0._, which means that I consider the code to be in a pre-release state. The consequence of this is that there may be small breaking changes even though only the minor part of the version is bumped up. As soon as the major part of the version is bumped to _1._ I will start using semantic versioning, so you can count on minor version upgrades being backwards compatible.

Officially released builds are available on nuGet.org,
 - Core: [![nuGet Core](https://img.shields.io/nuget/v/LeanTest.Core.svg?style=plastic)](https://www.nuget.org/Packages/LeanTest.Core)
 - Mock: [![nuGet Mock](https://img.shields.io/nuget/v/LeanTest.Mock.svg?style=plastic)](https://www.nuget.org/Packages/LeanTest.Mock)
 - JSon: [![nuGet JSon](https://img.shields.io/nuget/v/LeanTest.JSon.svg?style=plastic)](https://www.nuget.org/Packages/LeanTest.JSon) 
 
 ## Support
 [![Join the chat at https://gitter.im/Leantestnet/Lobby](https://badges.gitter.im/Leantestnet/Lobby.svg)](https://gitter.im/Leantestnet/Lobby?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge&utm_content=badge)

## The Concept
The concept of _Lean Testing_ (formerly _developer testing_) is not very well understood. I hope in the following to shed a little light on the subject. Also, watch [my blog](https://blog.elgaard.com), I may eventually explain the concept in great detail there.

In short, this is about developers writing tests while developing code. And I mean tests that testers would call _real_ tests, not simply unit tests. Tests which are simple to write initially, then simple to maintain going forward. Tests which cover actual functionality which is recognisable by and valuable to the business.

The way we achieve all this is by _maximizing code under test_ but _minimizing data_.

But before we dig into that, let's look at an even more fundamental concept, that of _existing state_.

### Existing state

When _testers write tests_ they often talk about _test data_. What they usually mean is that they use a set of test data, say a washed and minimized version of the full production database, which is sufficient for a number of tests to run. 

There are well known problems with this approach, for example that it can be difficult to keep the data and schema up-to-date with changes to the production system.

Personally, I don't like such a scatter-gun approach to test data, but I will come back to that.

When _developers write unit tests_ they usually don't think much about test data as a concept, but naturally they do use it.

In a unit test, data is usually passed directly in each test, possibly via some kind of a mocking framework. Some of this data is similar to the test database in a _tester test_, while other data is input to the target under test. Usually, it is not clear which is which because unit tests traditionally work on such a low level that input to the target under test in a unit test would be data read from a database in a _tester test_.

The effect of this is that for a traditional unit tests, it is not clear of what kind the data is, there is often many mocks and there is a separate mocking strategy per test. I don't like any of that.

We have a single concept for the equivalent of a test database in Lean Testing - we call it _existing state_. We simply insist that each test must declare what data it needs in order to succeed. For this we have a _test context_ to which we declare the data needed per test. Something like the following,

````csharp
        [TestMethod]
        public void GetAgeMustReturn10WhenKeyMatchesNewedUpData()
        {
            _contextBuilder
                .WithData(new MyData { Age = 10, Key = "ac_32_576259321" })
                .Build();

            int actual = _target.GetAge("FourtyTwo");

            Assert.AreEqual(10, actual);
        }
````
In the above example, we have declared that our test must succeed if the only test data available is one specific instance of MyData. By the magic of dependency injection and a builder pattern (which will be described below), the data will be available to our test target.

Our test target can potentially be part of a huge and tangled code base, but by minimizing the data per test, we can handle that with very few and simple mocks. Which is what the next section is about.

### Maximizing code under test but minimizing data

Maximizing code under test means not mocking away logic unless we really have to. And we only really have to mock logic away if we cannot control it deterministically (or if it is really slow to execute). In practice, this usually means that truly external dependencies must be mocked and nothing more. And we have a single mocking strategy for an entire test suite, having slightly different mocking per test case is a no-no.

Minimizing data means ensuring that exactly the data needed for a given test to run (yes, we do this _per-test_ unlike the way we do mocking) is provided for the test. With naming we try to express exactly what characteristica of the data will make the test run.

### The builder pattern

In the above example, we declare the data, then call `Build`,

````csharp
            _contextBuilder
                .WithData(new MyData { Age = 10, Key = "ac_32_576259321" })
                .Build();
````

We use a builder pattern for our data, as that layer of indirection from where the data is put allows us to write all levels of tests with the same syntax and the same concepts. The same test code will run in different levels of test,

- completely in-memory with all external dependencies mocked,
- in-memory, except for controllable external dependencies, such as e.g. in-memory databases (I use MongoDb no-SQL and Microsoft LocalDb SQL), and
- in a fully integrated environmnet with (ideally) nothing mocked out.

The differences among these models of execution is handled behind the scenes, with simple supporting test code which can be implemented per level of test. When we mock away logic, we substitute it my test code that implements an interface `IMockForData<>` with the type of data declared as a generic parameter. When we handle state in e.g. a database without mocking away the database code, we have test code which implements `IStateHandler<>`.

It is that simple - the LeanTest nuGet packages handles the rest.

And the good part is that experience shows that for large code bases only a few of these implementations are needed, and each of these are very simple with hardly any logic. When we made [tradingfloor.com](https://www.tradingfloor.com/) we had a handful of mocks/state handlers with cyclomatic complexity close to 1.

For tests which run in complex and shared test environments, it is not realistic to minimize data the way we can in in-memory tests. In such environments, state handler implementations will check if the required data is there and fail in a recognizable way if the required data is not there.

So in such tests, the `WithData<>` syntax means _check if the data is there, fail if not_, while in in-memory tests it means _ensure that the data is there by putting it whereever it belongs_.

The above is not the only advantage of the builder pattern in this context, but experience ha sshown that it is by far the most useful and most used.

## An Example
Note that the test below declares all _data_ that it depends on in a `WithData` method. This is quite central to lean tests - the target under test is always _empty_ with respect to data before each test, while any data that each individual test depends on is explicitly declared. In this case, the data is fed into a simple mock of an external service. In other cases the data could be fed into an (initially empty) database or other means for keeping state. The same data can be fed to multiple places. This is not a concern of the test method as it is handled by the contect builder.

````csharp
        [TestMethod]
        public void GetAgeMustReturn10WhenKeyMatchesNewedUpData()
        {
            _contextBuilder
                .WithData(new MyData { Age = 10, Key = "ac_32_576259321" })
                .Build();

            int actual = _target.GetAge("FourtyTwo");

            Assert.AreEqual(10, actual);
        }
````

## References
An early version of LeanTest, as well as the concept of developer testing, was used extensively while developing Saxo Bank's social web site, [TradingFloor.com](https://www.tradingfloor.com/). Around 850+ developer tests (today 1000+ tests) covered most business functionality, running on a typical developer PC in around one minute. Major refactorings of the code were done with these tests as a safety net.

LeanTest is currently used internally in Saxo Bank's REST based [Open API](https://developer.saxo). I intend to also use LeanTest in my experimental [C# Open API](https://github.com/belgaard/TopOA) which wraps the official REST Open API.
