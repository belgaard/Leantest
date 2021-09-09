# Introduction

The need for a library like LeanTest.Net became obvious when we noticed that a set of test doubles, which we prefer to refer to as *mocks*, for a non-trivial *set of automated tests tended to not be designed as a set*. Also, there was no consistency in the way that test data was passed to mock instances. The consequence of this was more often than not that it was difficult to maintain the set of tests and it was very difficult to reason about functional coverage.

We found that the Builder pattern could serve as a suitable abstraction for passing data in a consistent manner to a set of consistently designed mocks.

On top of this Builder pattern implementation we added a number of guiding principles which gave us an even higher level of consistency. One of the guiding principles is that we want to *put as much code under test as possible*. As a consequence of that *we mock as little as possible*. Also, when we do mock, we try to do it in a way which mocks away as little production code as possible. We call these and a few more guiding principles the *Lean Testing methodology*.

Funnily enough, it turned out that using our Builder pattern and following our guiding principles, our tests not only became more easy to maintain, but it was also much easier to reason about functional coverage. What started as a *better way of unit testing* for developers, was now what testers call *real tests*. In fact, the gap between developers and (automation) testers has been reduced significantly, which is a very important outcome.

We get all of that, and still we have fast and 100% deterministic tests, as we would expect for unit tests.

At the time of writing, we write such tests at *all levels*, ranging from tests which are similar to traditional unit tests, middle-ground tests which cover e.g. database stored procedures, and all the way to full-fledged system integration tests. All of these with a consistent syntax and design of test code and mocks.

Covering all levels required that we expanded the concept of mocks. Essentially, to us a mock is something that substitutes, or *mocks away*, some production code, whereas a *state handler* is something that handles data for the test target without mocking away production code. Low levels of test, those that are similar to traditional unit tests, will do some mocking. High levels of tests, those that are similar to integration tests, will, at least ideally, not mock at all but will probably use state handlers. The middle ground tests will use a mix. State handlers need infrastructure support in order to interact with a test environment, but LeanTest.Net is not concerned with the details of that.

We have borrowed part of our nomenclature from the Microsofties since they have been on a quest to [*shift left to make testing fast and reliable*](https://docs.microsoft.com/en-us/devops/develop/shift-left-make-testing-fast-reliable), not at all unlike what lead to the concepts described here. For the definition of levels of tests, we are very much aligned,

- L0 tests are fast, in-memory tests. What most people would call unit tests, except that we tend to have very large units. You can include e.g. an in-memory Entity Framework database with an L0 test suite.
- L1 tests are as L0 tests, but may have one or more out-of-process dependencies. The purpose of L1 tests is to test "code" which resides in out-of-process dependencies, such as e.g. stored procedures in SQL Server. Since L1 tests run in-memory, you don't need an environment per se, but you could run your SQL Server instance with the relevant schema in a Docker container on your build server and local computers.
- L2 tests target deployed, out-of-process code. L2 tests require some kind of test environment. We prefer *owned, on-demand environments* because *shared, long-living environments* tend to cause non-deterministic (flaky) tests and *the value for money* ([confidence for stakeholders](https://dannorth.net/2021/07/26/we-need-to-talk-about-testing/)) tends to be very low.

## The Concept

The underlying thoughts behind the _Lean Testing methodology_ (formerly known as _developer testing_) are described in three articles on medium.com,

- [Should You Unit Test?](https://medium.com/codex/should-you-unit-test-fd801abf9d04)

- [Should You Unit-Test in ASP.NET Core?](https://medium.com/swlh/should-you-unit-test-in-asp-net-core-793de767ac68)

- [Why Don’t You Take ‘Given’ in BDD Seriously?](https://javascript.plainenglish.io/why-dont-you-take-given-in-bdd-seriously-f168da29f1c)  

Another angle of this topic was described at the IWCT 2021 conference in the paper,

- [A Practical Method for API Testing in the Context of Continuous Delivery and Behavior Driven Development](https://ieeexplore.ieee.org/abstract/document/9440154) ([you can see the presentation video here](https://zenodo.org/record/4661956#.YOv17Pkzabg))

The following is an intro to the subject. 

In short, this is about *developers writing tests while developing code*. And I mean tests that testers would call _real_ tests, not simply unit tests. Tests which are simple to write initially, then simple to maintain going forward. Tests which cover actual functionality which is recognizable by and valuable to the business. In fact, tests which are conceptually and syntactically identical to the tests that automation testers would write.

The way we achieve all this is by _maximizing code under test_ but _minimizing data_.

Before we dig into that, let's look at an even more fundamental concept, that of _initial context_.

### Initial context

When _testers write tests_ they often talk about _test data_. What they usually mean is that they use data, say a washed and minimized version of the full production database, which is sufficient for a number of tests to run. 

There are well known problems with this approach, for example that it can be difficult to keep the data and schema up-to-date with changes to the production system. Another problem is that each test will make assumptions about specific data in the set and these assumptions are usually not clear and documented, causing tests to fail mysteriously when the test data is updated.

Personally, I don't like such a scatter-gun approach to test data, but I will come back to that.

When _developers write unit tests_ they usually don't think much about test data as a concept, but naturally they do use it.

In a unit test, data is usually passed directly in each test, possibly via some kind of a mocking framework. Some of this data is similar to the test database in a _tester test_, while other data is input to the target under test. Usually, it is not clear which is which because unit tests traditionally work on such a low level that input to the target under test in a unit test would be data read from a database in a _tester test_.

The effect of this is that for a traditional unit test, it is not clear of what kind the data is, there is often many mocks and there is a separate mocking strategy per test. I don't like any of that.

We have a single concept for the equivalent of a test database in Lean Testing - we call it _initial context_. We simply insist that each test must declare what data it needs in order to succeed. For this we have a _test context_ to which we declare the data needed per test. Something like the following,

<<Example of initial context>>

In the above example, we have declared that our test must succeed if the only test data available is one specific instance of MyData. By the magic of dependency injection and a builder pattern (which will be described below), the data will be available to our test target.

Our test target can potentially be part of a huge and entangled code base, but by minimizing the data per test, we can handle that with very few and simple mocks. Which is what the next section is about.

### Maximizing code under test but minimizing data

Maximizing code under test means not mocking away logic unless we really have to. And we only really have to mock logic away if we cannot control it deterministically (or if it is really slow to execute). In practice, this usually means that truly external dependencies must be mocked and nothing more. And we have a single mocking strategy for an entire test suite, having slightly different mocking per test case is a no-no.

Minimizing data means ensuring that exactly the data needed for a given test to run (yes, we declare data _per-test_) is provided for the test. With naming we try to express exactly what characteristics of the data will make the test run.

### The builder pattern

In the above example, we declare the data, then call `Build`,

<<Example of using a builder pattern>>

We use a builder pattern for our data, as that layer of indirection from where the data is put allows us to write all levels of tests with the same syntax and the same concepts. The same test code will run in different levels of test,

- completely in-memory with all external dependencies mocked,
- in-memory, except for controllable external dependencies, such as e.g. in-memory databases (I use MongoDb no-SQL, Microsoft LocalDb SQL and Microsoft SQL Server in a Docker container), and
- in a fully integrated environment with (ideally) nothing mocked out.

The differences among these models of execution is handled behind the scenes, with simple test code which can be implemented per level of test. When we say that we *mock away logic*, we mean that we substitute production code with test code which implements an interface `IMockForData<>` with the type of data declared as a generic parameter. When we handle state in e.g. a database without mocking away the database code, we have test code which implements `IStateHandler<>`.

It is that simple - the LeanTest nuGet packages handle the rest.

And the good part is that experience shows that for large code bases only a few of these implementations are needed, and each of these are very simple with hardly any logic. When we made [tradingfloor.com](https://www.tradingfloor.com/) we had a handful of mocks/state handlers with cyclomatic complexity close to 1.

For tests which run in complex and shared test environments, it is not realistic to minimize data the way we can in in-memory tests. In such environments, state handler implementations will check if the required data is there and fail in a recognizable way if the required data is not there.

So in such tests, the `WithData<>` syntax means _check if the data is there, fail if not_, while in in-memory tests it means _ensure that the data is there by putting it wherever it belongs_.

The above is not the only advantage of the builder pattern in this context, but experience has shown that it is by far the most useful and most used.
