# Introduction

## The Concept
The concept of _Lean Testing_ (formerly _developer testing_) is not very well understood. I hope in the following to shed a little light on the subject. Also, watch [my blog](https://blog.elgaard.com), I may eventually explain the concept in great detail there.

In short, this is about developers writing tests while developing code. And I mean tests that testers would call _real_ tests, not simply unit tests. Tests which are simple to write initially, then simple to maintain going forward. Tests which cover actual functionality which is recognisable by and valuable to the business. In fact, tests which are conceptually and syntactically identical to the tests that automation testers write.

The way we achieve all this is by _maximizing code under test_ but _minimizing data_.

Before we dig into that, let's look at an even more fundamental concept, that of _existing state_.

### Existing state

When _testers write tests_ they often talk about _test data_. What they usually mean is that they use a set of test data, say a washed and minimized version of the full production database, which is sufficient for a number of tests to run. 

There are well known problems with this approach, for example that it can be difficult to keep the data and schema up-to-date with changes to the production system. Another problem is that each test will make assumptions about specific data in the set and these assumptions are usually not clear and documented, causing tests to fail mysteriously when the test data is updated.

Personally, I don't like such a scatter-gun approach to test data, but I will come back to that.

When _developers write unit tests_ they usually don't think much about test data as a concept, but naturally they do use it.

In a unit test, data is usually passed directly in each test, possibly via some kind of a mocking framework. Some of this data is similar to the test database in a _tester test_, while other data is input to the target under test. Usually, it is not clear which is which because unit tests traditionally work on such a low level that input to the target under test in a unit test would be data read from a database in a _tester test_.

The effect of this is that for a traditional unit tests, it is not clear of what kind the data is, there is often many mocks and there is a separate mocking strategy per test. I don't like any of that.

We have a single concept for the equivalent of a test database in Lean Testing - we call it _existing state_. We simply insist that each test must declare what data it needs in order to succeed. For this we have a _test context_ to which we declare the data needed per test. Something like the following,

<<Example of existing state>>

In the above example, we have declared that our test must succeed if the only test data available is one specific instance of MyData. By the magic of dependency injection and a builder pattern (which will be described below), the data will be available to our test target.

Our test target can potentially be part of a huge and tangled code base, but by minimizing the data per test, we can handle that with very few and simple mocks. Which is what the next section is about.

### Maximizing code under test but minimizing data

Maximizing code under test means not mocking away logic unless we really have to. And we only really have to mock logic away if we cannot control it deterministically (or if it is really slow to execute). In practice, this usually means that truly external dependencies must be mocked and nothing more. And we have a single mocking strategy for an entire test suite, having slightly different mocking per test case is a no-no.

Minimizing data means ensuring that exactly the data needed for a given test to run (yes, we do this _per-test_ unlike the way we do mocking) is provided for the test. With naming we try to express exactly what characteristica of the data will make the test run.

### The builder pattern

In the above example, we declare the data, then call `Build`,

<<Example of using a builder pattern>>

We use a builder pattern for our data, as that layer of indirection from where the data is put allows us to write all levels of tests with the same syntax and the same concepts. The same test code will run in different levels of test,

- completely in-memory with all external dependencies mocked,
- in-memory, except for controllable external dependencies, such as e.g. in-memory databases (I use MongoDb no-SQL and Microsoft LocalDb SQL), and
- in a fully integrated environmnet with (ideally) nothing mocked out.

The differences among these models of execution is handled behind the scenes, with simple supporting test code which can be implemented per level of test. When we mock away logic, we substitute it with test code that implements an interface `IMockForData<>` with the type of data declared as a generic parameter. When we handle state in e.g. a database without mocking away the database code, we have test code which implements `IStateHandler<>`.

It is that simple - the LeanTest nuGet packages handle the rest.

And the good part is that experience shows that for large code bases only a few of these implementations are needed, and each of these are very simple with hardly any logic. When we made [tradingfloor.com](https://www.tradingfloor.com/) we had a handful of mocks/state handlers with cyclomatic complexity close to 1.

For tests which run in complex and shared test environments, it is not realistic to minimize data the way we can in in-memory tests. In such environments, state handler implementations will check if the required data is there and fail in a recognizable way if the required data is not there.

So in such tests, the `WithData<>` syntax means _check if the data is there, fail if not_, while in in-memory tests it means _ensure that the data is there by putting it wherever it belongs_.

The above is not the only advantage of the builder pattern in this context, but experience has shown that it is by far the most useful and most used.