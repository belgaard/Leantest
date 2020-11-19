using System;
using System.Net.Http;
using LeanTest.Core.ExecutionHandling;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;

namespace LeanTest
{
    /// <summary>This is used to integrate the .Net Core web host builder pattern with the Lean Test context builder pattern.</summary>
    public static class AspNetCoreContextBuilderFactory
    {
        private static IWrapper _wrapped;

        /// <summary>ASP.NET Core version of setting up IoC and builders.</summary>
        /// <param name="factoryFactory">The factory used to create a WebApplicationFactory&lt;T&gt; instance.</param>
        /// <param name="iocContainerFactory">Creates the IoC container, such as e.g. 'provider => new IocContainer(provider)'.</param>
        /// <typeparam name="T">The Startup class of the unit under test.</typeparam>
        public static void Initialize<T>(Func<WebApplicationFactory<T>> factoryFactory, Func<IServiceProvider, IIocContainer> iocContainerFactory) where T: class =>
            Initialize(CleanContextMode.ReCreate, factoryFactory, iocContainerFactory);

        /// <summary>ASP.NET Core version of setting up IoC and builders.</summary>
        /// <param name="mode">Always use ReCreate to recreate the IoC container before each test.</param>
        /// <param name="factoryFactory">The factory used to create a WebApplicationFactory&lt;T&gt; instance.</param>
        /// <param name="iocContainerFactory">Creates the IoC container, such as e.g. 'provider => new IocContainer(provider)'.</param>
        /// <typeparam name="T">The Startup class of the unit under test.</typeparam>
        public static void Initialize<T>(CleanContextMode mode, Func<WebApplicationFactory<T>> factoryFactory, Func<IServiceProvider, IIocContainer> iocContainerFactory) where T: class =>
            ContextBuilderFactory.Initialize(mode, () =>
            {
                _wrapped?.DisposeAndRemove();

                var factory = new FactoryWrapper<T>(factoryFactory);
                _wrapped = factory;
                IServiceProvider serviceProvider = factory.GetFactory()?.Services;

                return iocContainerFactory(serviceProvider);
            });

        /// <summary>ASP.NET Core version of setting up IoC and builders.</summary>
        /// <param name="mode">Always use ReCreate to recreate the IoC container before each test.</param>
        /// <param name="webHostBuilder">The ASP.NET Core web host builder from production code, overridden with mocks using .ConfigureTestServices().</param>
        /// <param name="iocContainerFactory">Creates the IoC container, such as e.g. 'provider => new IocContainer(provider)'.</param>
        /// <remarks>The passed in web host builder is used to create a <c>TestHost</c>.
        /// A client is created and can be accessed through <c>GetHttpClient()</c>.
        /// The test host instance is disposed as the first step of initialisation.</remarks>
        public static void Initialize(CleanContextMode mode, Func<IWebHostBuilder> webHostBuilder, Func<IServiceProvider, IIocContainer> iocContainerFactory) =>
            ContextBuilderFactory.Initialize(mode, () =>
            {
                _wrapped?.DisposeAndRemove();

                var testServer = new TestServer(webHostBuilder());
                _wrapped = new Wrapper(testServer, testServer.CreateClient());
                IServiceProvider serviceProvider = testServer.Host.Services;

                return iocContainerFactory(serviceProvider);
            });

        /// <summary>ASP.NET Core version of setting up IoC and builders.</summary>
        /// <param name="mode">Always use ReCreate to recreate the IoC container before each test.</param>
        /// <param name="hostBuilder">The ASP.NET Core host builder from production code, overridden with mocks using .ConfigureTestServices().</param>
        /// <param name="iocContainerFactory">Creates the IoC container, such as e.g. 'provider => new IocContainer(provider)'.</param>
        /// <remarks>The passed in web host builder is used to create a <c>TestHost</c>.
        /// A client is created and can be accessed through <c>GetHttpClient()</c>.
        /// The test host instance is disposed as the first step of initialisation.</remarks>
        public static void Initialize(CleanContextMode mode, Func<IHostBuilder> hostBuilder, Func<IServiceProvider, IIocContainer> iocContainerFactory) =>
            ContextBuilderFactory.Initialize(mode, () =>
            {
                _wrapped?.DisposeAndRemove();

                var host = hostBuilder().Build();
                host.Start();

                var testServer = host.GetTestServer();
                var client = testServer.CreateClient();
                _wrapped = new Wrapper(testServer, client);
                IServiceProvider serviceProvider = testServer.Services;

                return iocContainerFactory(serviceProvider);
            });

        /// <summary>ASP.NET Core version of setting up IoC and builders to create the IoC context before each test.</summary>
        /// <param name="webHostBuilder">The ASP.NET Core web host builder from production code, overridden with mocks using .ConfigureTestServices().</param>
        /// <param name="iocContainerFactory">Builds the IoC container, such as e.g. 'provider => new IocContainer(provider)'.</param>
        public static void Initialize(Func<IWebHostBuilder> webHostBuilder, Func<IServiceProvider, IIocContainer> iocContainerFactory) =>
            Initialize(CleanContextMode.ReCreate, webHostBuilder, iocContainerFactory);

        /// <summary>ASP.NET Core version of setting up IoC and builders to create the IoC context before each test.</summary>
        /// <param name="hostBuilder">The ASP.NET Core host builder from production code, overridden with mocks using .ConfigureTestServices().</param>
        /// <param name="iocContainerFactory">Builds the IoC container, such as e.g. 'provider => new IocContainer(provider)'.</param>
        public static void Initialize(Func<IHostBuilder> hostBuilder, Func<IServiceProvider, IIocContainer> iocContainerFactory) =>
            Initialize(CleanContextMode.ReCreate, hostBuilder, iocContainerFactory);

        /// <summary>Get the ASP.NET Core <c>TestServer</c> created in <c>Initialize()</c>.</summary>
        public static TestServer GetTestServer(this ContextBuilder contextBuilder) => (contextBuilder as IFactoryAccess)?.Server ?? _wrapped?.GetTestServer();
        /// <summary>Get the ASP.NET Core client from the <c>TestServer</c> created in <c>Initialize()</c>.</summary>
        public static HttpClient GetHttpClient(this ContextBuilder contextBuilder) => (contextBuilder as IFactoryAccess)?.CreateClient() ?? _wrapped?.GetHttpClient();
        /// <summary>Get the ASP.NET Core client from the factory created in <c>Initialize()</c>.</summary>
        /// <remarks>Returns null if a Func&lt;WebApplicationFactory&lt;T&gt;&gt; was not passed in <c>Initialize()</c></remarks>
        public static WebApplicationFactory<T> GetFactory<T>(this ContextBuilder contextBuilder) where T: class => 
            WebApplicationFactoryFromContextBuilder<T>(contextBuilder) ?? (_wrapped as FactoryWrapper<T>)?.GetFactory();
        private static WebApplicationFactory<T> WebApplicationFactoryFromContextBuilder<T>(ContextBuilder contextBuilder) where T: class => 
            (contextBuilder as WebApplicationFactoryContextBuilder<T>)?.GetFactory();

        private interface IWrapper : IDisposable
        {
            TestServer GetTestServer();
            HttpClient GetHttpClient();
            void DisposeAndRemove()
            {
	            Dispose();
	            ContextBuilderFactory.RemoveFromCleanup(this);
            }
        }

        /// <summary>Get client and test server from ctor.</summary>
        private class Wrapper : IWrapper
        {
            private TestServer _testServer;
            private HttpClient _client;

            public Wrapper(TestServer testServer, HttpClient client)
            {
                _testServer = testServer;
                _client = client;
                ContextBuilderFactory.AddForCleanup(this);
            }
            public TestServer GetTestServer() => _testServer;
            public HttpClient GetHttpClient() => _client;

            public void Dispose()
            {
                (_testServer?.Host?.Services as IDisposable)?.Dispose();
                _testServer?.Dispose();
                _testServer = null;
                _client?.Dispose();
                _client = null;
            }
        }

        /// <summary>Get client and test server from factory.</summary>
        /// <typeparam name="T">The Startup class of the unit under test.</typeparam>
        private class FactoryWrapper<T> : IWrapper where T: class
        {
            private WebApplicationFactory<T> _factory;
            private TestServer _testServer;
            private HttpClient _client;

            public FactoryWrapper(Func<WebApplicationFactory<T>> factoryFactory)
            {
	            _factory = factoryFactory();
	            ContextBuilderFactory.AddForCleanup(this);
            }

            public TestServer GetTestServer() => _testServer ??= _factory?.Server;
            public HttpClient GetHttpClient() => _client ??= _factory?.CreateClient();
            public WebApplicationFactory<T> GetFactory() => _factory;

            public void Dispose()
            {
                (_factory?.Services as IDisposable)?.Dispose();
                _factory?.Dispose();
                _factory = null;
            }
        }
    }
}