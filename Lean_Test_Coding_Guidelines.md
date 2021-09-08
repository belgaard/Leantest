# Coding Guidelines

Note that the following guidelines assume that the test projects have [the latest LeanTest.Net packages from nuGet](https://www.nuget.org/packages?q=leantest) installed.

The *LeanTest.Core* package contains the core builder pattern implementation, initialization etc.

The *LeanTest.JSon* package adds extension methods for `WithData<T>(string)` for which a string parameter is deserialized to `T`.

The *LeanTest.DI.DotNetCore* package implements a wrapper for .NET Core/.NET 5 dependency injection. Use this package rather than implementing `IIocContainer` if you use .NET Core/.NET 5.

The *LeanTest.AspNetCore* package contains helper methods for setting up LeanTest as well as the .NET Core test host. LeanTest can handle all disposing, or you can chose to do that yourself. **You do not need this package** if you use the recommended method of initialization described below.

The *LeanTest.MsTest* package adds MsTest specific functionality, e.g. attributes initialized via an MsTest context. You only need this package if you use the MsTest test runner.

The *LeanTest.Xunit* package adds Xunit specific functionality, e.g. attributes initialized via an Xunit `ITestOutputHelper`. You only need this package if you use the xUnit test runner.

## Introduction

Lean Testing requires little or no refactoring of production code in order to put the code under test.

The only hard requirement which Lean Testing imposes is that external dependencies must be well-defined. Furthermore, if these external dependencies are represented by interfaces implemented as pass-through to the actual external dependency then it is quite possible to put all relevant logic under test. Finally, if at least these external dependency interfaces and implementations are handled with dependency injection, then life as a person implementing automated tests is easy.

However, the job is a lot easier if everybody consistently follows the following coding guidelines.

## Tests

Each test class is initialized by creating a context builder and then getting an instance of the test target. There are a couple of ways to do that and your choice of test framework will influence other details, but it will generally look something like the following,

```csharp
[TestClass]
public class TestMyService
{
    private ContextBuilder _contextBuilder;
    private HttpClient _target;

    public TestContext TestContext { get; set; } // use a TestContext if you use MS Test

    [TestInitialize]
    public void TestInitialize()
    {
        _contextBuilder = // there are a couple of ways to create a context builder
            .RegisterAttributes(TestContext)
        _target = _contextBuilder.GetHttpClient();
    }
    // ...
}
```

Tests largely follow the Arrange, Act, Assert (AAA) pattern. When *arranging*, the initial context is setup using `WithData` statements on the context builder, the *act* part will exercise the test target, and the *assert* part will, well, assert on the outcome. Here is an example,

```csharp
[TestMethod, TestScenarioId("UserAssumptions")]
public async Task AssumeUsersAsyncMustReturnIbWithActiveChildWhenAssumingIbWithActiveChild()
{
    _contextBuilder
        .WithData<UsersWithEnvironment>(TestData.UsersWithEnvironment.IbWithWithActiveChild)
        .Build();

    User actual = await _target.AssumeUsersAsync(myAssumption).ConfigureAwait(false);

    Assert.IsTrue(actual.ChildrenOfIb.Any(c => c.IsActive == true));
}
```

Note that it is quite possible that you need [multiple asserts in a single unit test method](https://elgaard.blog/2011/02/06/multiple-asserts-in-a-single-unit-test-method/), and that's OK as long as you use the magic of `MultiAssert` so that all relevant assertions are always executed.

Since the main focus for Lean Testing is usually *functional coverage*, you will often need to combine multiple A's in a single test. That is often also OK, as long as the test code is as simple and maintainable as possible, and as long as all relevant assertions are always executed.

LeanTest itself must be initialized once per test suite. As a matter of principle we prefer that each test case starts with a clean (or *empty*, if you will) environment.

There are a number of ways to ensure a clean environment, including the use of an assembly initializer class and static factory classes.

A better way which does not require an assembly initializer class is described in the next section.

## Recommended LeanTest initialization

The recommended method of initialization can be seen as a method with less *hidden magic* than the *`AssemblyInitializer` based method* described below. Without the magic, you need to handle any clean-up needed, such as e.g. disposing instances.

Below are examples of how to implement it for the .Net Core built-in IoC container. The first example does not use the ASP.NET Core test server, so you can use this approach for non-ASP.NET Core projects. Note that an assembly initializer is not needed, we simply new up a context builder for each test.

### Initialization without a test server

```csharp
[TestInitialize]
public void TestInitialize()
{
    _contextBuilder = new MyContextBuilderFactory().ContextBuilder
        .WithData<MyData>()
        .RegisterAttributes(TestContext);

    _target = _contextBuilder.GetInstance<MyApplicationService>();
}

/// <summary>
/// Does the setup which must must be done consistently across all tests in the assembly.
/// </summary>
public class MyContextBuilderFactory
{
    public ContextBuilder ContextBuilder {get;}
    public MyContextBuilderFactory() =>     
        ContextBuilder = 
            new ContextBuilder(
                new IocContainer(L0CompositionRootForTest.Initialize(
                    CompositionRoot.Initialize(new ServiceCollection()))));
}
```

### Initialization with a test server

```csharp
[TestClass]
public class TestAuthController
{
    private ContextBuilder _contextBuilder;
    private HttpClient _target;
    private WebApplicationFactory<Startup> _factory;

    public TestContext TestContext { get; set; }
        
    [TestInitialize]
    public void TestInitialize()
    {
        _factory = new ExampleWebApplication().Factory;
        _contextBuilder = new ContextBuilder(new LeanTestContainer(_factory.Services))
            .RegisterAttributes(TestContext);
        _target = _factory.CreateClient();
    }

    /// <summary>
    /// The factory is not automatically disposed.
    /// In this example, the test fail if we don't dispose manually.
    /// </summary>
    [TestCleanup]
    public void TestCleanup() => _factory.Dispose();
// ...
}
/// <summary>
/// Does the setup which must must be done consistently across all tests in the assembly.
/// </summary>
public class ExampleWebApplication
{
    public WebApplicationFactory<Startup> Factory {get;}

    public ExampleWebApplication()
    {
        Factory = new WebApplicationFactory<Startup>();
        Factory = Factory.WithWebHostBuilder(builder =>
            builder
                .ConfigureTestServices(L0CompositionRootForTest.Initialize);
    }
}
```

Using a test server is preferred for ASP.NET Core, as this approach will exercise the end-point through the http stack, which means that even routing, model binding etc. will be under test. For a good description of the benefits of testing through the .NET Core test server see this [blog post](https://andrewlock.net/should-you-unit-test-controllers-in-aspnetcore/) or this [Pluralsight course](https://app.pluralsight.com/library/courses/integration-testing-asp-dot-net-core-applications-best-practices/table-of-contents). While it is not recommended to test through the controller C# class in .NET Core, it is sometimes necessary to test through other objects registered in the IoC container. Note that even in these cases *the principle of maximizing the code under test* must be adhered to.

## Using an assembly initializer class and static factory classes

The initialization can be done in an assembly initializer class so that initialization is done exactly once per test suite (a test project is considered to be a test suite). We ensure a clean environment per test case by passing a factory method to LeanTest's `ContextBuilderFactory` initializer. Below are examples of how to implement it for the .Net Core built-in IoC container. The first example does not use the ASP.NET Core TestServer, so you can use this approach for non-ASP.NET Core projects,

### Initialization without a test server

```csharp
/// <summary>
/// Does the setup which must must be done consistently across all tests in the assembly.
/// </summary>
[TestClass]
public static class AssemblyInitializer
{
    [AssemblyInitialize]
    public static void AssemblyInitialize(TestContext _)
    {
        IIocContainer IocFactory() => 
            new IocContainer(L0CompositionRootForTest.Initialize(
                CompositionRoot.Initialize(new ServiceCollection())));
        ContextBuilderFactory.Initialize(IocFactory);
    }

    [AssemblyCleanup]
    public static void AssemblyCleanup() => ContextBuilderFactory.Cleanup();
}
```

### Initialization with a test server

```csharp
[TestClass]
public static class AssemblyInitializer
{
    [AssemblyInitialize]
    public static void AssemblyInitialize(TestContext _)
    {
        static WebApplicationFactory<Startup> FactoryFactory()
        {
            var factory = new WebApplicationFactory<Startup>();
            factory = factory.WithWebHostBuilder(builder =>
                builder
                    .ConfigureTestServices(L0CompositionRootForTest.Initialize));

            return factory;
        }
        AspNetCoreContextBuilderFactory.Initialize(
            FactoryFactory, provider => new IocContainer(provider));
    }

    [AssemblyCleanup]
    public static void AssemblyCleanup() => ContextBuilderFactory.Cleanup();
}
```

The above examples of initialization is for L0 test projects, but L1 test projects will be very similar, the difference usually being code needed in order to reference a database running in a Docker container. Initialization in L2 test projects will be different because L2 tests depend on the run-time environment and environment dependencies make life difficult

## Using the Xunit Test Framework

The examples have until now implicitly used the MS Test test framework. The Lean Testing approach and the LeanTest.NET core library do not depend on any specific test environment, but separate nuGet packages provide support for MS Test and Xunit.

An Xunit test project must include the packages

- *xunit* and *xunit.runner.visualstudio* (not *MSTest.\**), and
- *LeanTest.xUnit* (not *LeanTest.MsTest*)

### Recommended LeanTest initialization with Xunit

With Xunit, you new up a context builder similar to how it is done for MS Test..

### Using an assembly initializer class and static factory classes with Xunit

Initialization of LeanTest.NET itself in an `AssemblyInitializer` class is similar to MS Test based test projects, except that the following attributes must be added,

```csharp
[assembly: AssemblyFixture(typeof(AssemblyInitializer))]
[assembly: Xunit.TestFramework("LeanTest.Xunit.XunitExtensions.XunitTestFrameworkWithAssemblyFixture", "LeanTest.Xunit")]

public sealed class AssemblyInitializer
{
    public AssemblyInitializer()
    {
        static ICreateContextBuilder FactoryFactory() => 
            new CustomFactory<Startup>(L0CompositionRootForTest.Initialize, provider => new IocContainer(provider));

        ContextBuilderFactory.Initialize(FactoryFactory);
    }
}
```

### Writing Xunit tests

Each Xunit test class will be very similar to MS Test test classes. Xunit does not have a `TestInitialize` attribute, but rather use the constructor for that purpose,

```csharp
public class TestMyController
{
    private readonly ContextBuilder _contextBuilder;
    private readonly HttpClient _target;

    public TestMyController(ITestOutputHelper output)
    {
        var testContext = new TestContext(output);
        _contextBuilder = ContextBuilderFactory.CreateContextBuilder()
            .RegisterAttributes(testContext)
            .Build();
        _target = _contextBuilder.GetHttpClient();
    }
// ...
}
```

The difference here, when compared to an MS Test based test, is the Xunit specific `ITestOutputHelper` which is used to craft our own `TestContext`. The reason for this is that Xunit does not support a test context (it is planned for version 3.x) as MS Test does.

Each Xunit test will differ from similar MS Test dittos only for test framework specific differences, such as Xunit using `Fact` or `Theory` attributes rather than `TestMethod`. Also, assertion syntax is different.

As always, use the test attributes from the *LeanTest.Attribute* namespace.

## Dependency Injection

It is not compulsory to follow the Dependency Inversion Principle (aka. DIP, one of [the SOLID principles](https://elgaard.blog/2014/05/31/coding-for-continuous-delivery-the-solid-principles/)) and to use an Inversion of Control (IoC) container, but doing so is considered best practice and it tends to make the life of a developer easier. At least when applied to truly external dependencies.

### Integrating LeanTest.Net with your Preferred IoC Container

LeanTest will resolve instances using your preferred IoC container by calling your implementation of the `IIocContainer` interface,

```csharp
public interface IIocContainer
{
    T Resolve<T>() where T : class;
    T TryResolve<T>() where T : class;
    IEnumerable<T> TryResolveAll<T>() where T : class;
}
```

LeanTest will resolve single instances using `Resolve<T>` and it is expected that failure to resolve an instance of a given type will throw an exception.

LeanTest will use `TryResolveAll<T>` to resolve instances of mock-for-data and state-handlers, i.e. implementations of `IMockForData<T>` and `IStateHandler<T>`. This way, it is possible to implement multiple mocks and handlers for a single `T`.

Here is how the *LeanTest.DI.DotNetCore* package implements `IIocContainer` for the .Net Core built-in IoC container,

```csharp
public class IocContainer : IIocContainer
{
    private readonly IServiceProvider _serviceProvider;
    public IocContainer(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

    public T Resolve<T>() where T : class => _serviceProvider.GetRequiredService<T>();
    public T TryResolve<T>() where T : class => _serviceProvider.GetService<T>();
    public IEnumerable<T> TryResolveAll<T>() where T : class => _serviceProvider.GetServices<T>();
}
```

## Composition Roots

It is recommended that all dependencies are injected into constructors and that dependencies (i.e. the object graph) is composed as close as possible to an application's entry point.

This recommendation goes for production code as well as for test code - see more details [here](https://blog.ploeh.dk/2011/07/28/CompositionRoot/).

For LeanTest, our recommendation goes one step further - we recommend that test code *reuse the production code composition root, only replacing the few mocks needed.* This makes it possible to catch tricky bugs that stem from life-time mismatches, such as injecting an object with transient life-time into a singleton. If the production code is following this best practice, then it will also simplify setup of the test project. And the simple test composition root will clearly document what is mocked.

In .Net Core it is customary to compose the object graph with the built-in IoC container in `Startup.ConfigureServices`, so this method *is* the composition root. If you wish, you can choose to delegate initialization of dependencies to a class called `CompositionRoot`,

```csharp
internal static class CompositionRoot
{
    internal static IServiceCollection Initialize(IServiceCollection services)
    {
        services.AddSingleton<IPortfolioService, PortfolioService>();
        services.AddSingleton<ITimeFacade, TimeFacade>();
        return services;
    }
}
```

In the above, we declare an external dependency, a *portfolio service*. Note that the interface `IPortfolioService` can be found in the production code in a folder named `ExternalDependencies` and that its implementation is simple and has cyclomatic complexity of 1. That is why we can mock this interface in a test project and still claim that we have tested all significant code.

The time façade wraps time, i.e. `DateTime.Now`. This can be necessary when time itself is a relevant external dependency (see [Lean Test Coding Patterns](Lean_Test_Coding_Patterns.html)).

Here is one way to implement the L0 test composition root for the .Net Core built-in IoC container,

```csharp
public static class L0CompositionRootForTest
{
    public static IServiceCollection Initialize(IServiceCollection services)
    {
        // Mocks (not mock-for-data):
        services.Replace(ServiceDescriptor.Singleton<ITimeFacade, MockTimeFacade>());

        // Mock-for-data:
        services.RegisterMockForData<IPortfolioService, MockForDataPortfolioService, AccountsList>();

        return services;
    }
}
```

In the above, we define a simple mock, not a mock-for-data, for the time façade, since we assume simple requirements for which hard-coded time is good-enough. This may change, so the mock may later be turned into a proper mock-for-data.

For the portfolio service, the tests require that we control initial context, so we use a utility method, `RegisterMockForData`, in order to declare that the mock-for-data implementation is `MockForDataPortfolioService` which implements `IPortfolioService`
and `IMockForData<AccountList>`.

Here is an example of how to implement `RegisterMockForData` for the .Net Core built-in IoC container,

```csharp
private static void RegisterMockForData<TInterface, TImplementation, TData>(this IServiceCollection container) 
    where TImplementation: class, TInterface, IMockForData<TData>
    where TInterface: class
{
    container.AddSingleton<TImplementation>();
    container.AddSingleton<TInterface>(x => x.GetRequiredService<TImplementation>());
    container.AddSingleton<IMockForData<TData>>(x => x.GetRequiredService<TImplementation>());
}
```

The benefits of consistent use of composition roots can be seen when nuGet packages are consumed. If nuGet packages expose their composition roots, then it is possible for the consuming service to provide test doubles for the nuGet packages' external dependencies, thereby putting more code under test.

The implementation of the above mentioned mock-for-data is as simple as usual, it receives the initial context data in `WithData`, in this case it receives an `AccountList`, which it hands out in its single `IPortfolioService` method, `GetMyAccountsAsync`,

```csharp
public class MockForDataPortfolioService : IPortfolioService, IMockForData<AccountsList>
{
    private AccountsList _accountsList;

    public Task<AccountsList> GetMyAccountsAsync(OpenApiHttpClient openApiClient) => 
        Task.FromResult(_accountsList);

    public void WithData(AccountsList data) => _accountsList = data;
}
```
