using System;
using System.Net.Http;
using LeanTest.Core.ExecutionHandling;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace LeanTest
{
    /// <summary>This is used to integrate the .Net Core web host builder pattern with the Lean Test context builder pattern.</summary>
    public static class AspNetCoreContextBuilderFactory
    {
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
        [Obsolete("Use the overload without a CleanContextMode parameter")]
        public static void Initialize<T>(CleanContextMode mode, Func<WebApplicationFactory<T>> factoryFactory, Func<IServiceProvider, IIocContainer> iocContainerFactory) where T: class =>
            ContextBuilderFactory.Initialize(mode, () =>
            {
                WebApplicationFactory<T> factory = factoryFactory(); // TODO: How to dispose factory?
                factory = factory.WithWebHostBuilder(builder => builder
                    .ConfigureServices(services =>
                    {
                        services.AddSingleton<IWrapper>(x => new Wrapper(factory.Server, factory.CreateClient()));
                    }));

                IServiceProvider serviceProvider = factory?.Services;

                return iocContainerFactory(serviceProvider);
            });

        /// <summary>ASP.NET Core version of setting up IoC and builders.</summary>
        /// <param name="webHostBuilder">The ASP.NET Core web host builder from production code, overridden with mocks using .ConfigureTestServices().</param>
        /// <param name="iocContainerFactory">Creates the IoC container, such as e.g. 'provider => new IocContainer(provider)'.</param>
        /// <remarks>This is simple way of initialisation, it does not provide a <c>TestHost</c> or test HttpClient.</remarks>
        public static void Initialize(Func<IWebHostBuilder> webHostBuilder, Func<IServiceProvider, IIocContainer> iocContainerFactory) =>
            Initialize(CleanContextMode.ReCreate, webHostBuilder, iocContainerFactory);

        /// <summary>ASP.NET Core version of setting up IoC and builders.</summary>
        /// <param name="mode">Always use ReCreate to recreate the IoC container before each test.</param>
        /// <param name="webHostBuilder">The ASP.NET Core web host builder from production code, overridden with mocks using .ConfigureTestServices().</param>
        /// <param name="iocContainerFactory">Creates the IoC container, such as e.g. 'provider => new IocContainer(provider)'.</param>
        /// <remarks>This is simple way of initialisation, it does not provide a <c>TestHost</c> or test HttpClient.</remarks>
        [Obsolete("Use the overload without a CleanContextMode parameter")]
        public static void Initialize(CleanContextMode mode, Func<IWebHostBuilder> webHostBuilder, Func<IServiceProvider, IIocContainer> iocContainerFactory) =>
            ContextBuilderFactory.Initialize(mode, () =>
            {
                IServiceProvider serviceProvider = new TestServer(webHostBuilder()).Host.Services;

                return iocContainerFactory(serviceProvider);
            });

        /// <summary>ASP.NET Core version of setting up IoC and builders to create the IoC context before each test.</summary>
        /// <param name="hostBuilder">The ASP.NET Core host builder from production code, overridden with mocks using .ConfigureTestServices().</param>
        /// <param name="iocContainerFactory">Builds the IoC container, such as e.g. 'provider => new IocContainer(provider)'.</param>
        /// <remarks>This is simple way of initialisation, it does not provide a <c>TestHost</c> or test HttpClient.</remarks>
        public static void Initialize(Func<IHostBuilder> hostBuilder, Func<IServiceProvider, IIocContainer> iocContainerFactory) =>
            Initialize(CleanContextMode.ReCreate, hostBuilder, iocContainerFactory);

        /// <summary>ASP.NET Core version of setting up IoC and builders.</summary>
        /// <param name="mode">Always use ReCreate to recreate the IoC container before each test.</param>
        /// <param name="hostBuilder">The ASP.NET Core host builder from production code, overridden with mocks using .ConfigureTestServices().</param>
        /// <param name="iocContainerFactory">Creates the IoC container, such as e.g. 'provider => new IocContainer(provider)'.</param>
        /// <remarks>This is simple way of initialisation, it does not provide a <c>TestHost</c> or test HttpClient.</remarks>
        [Obsolete("Use the overload without a CleanContextMode parameter")]
        public static void Initialize(CleanContextMode mode, Func<IHostBuilder> hostBuilder, Func<IServiceProvider, IIocContainer> iocContainerFactory) =>
            ContextBuilderFactory.Initialize(mode, () =>
            {
                IHost host = hostBuilder().Build();
                host.Start();

                IServiceProvider serviceProvider = host.GetTestServer().Services;

                return iocContainerFactory(serviceProvider);
            });

        /// <summary>Get the ASP.NET Core <c>TestServer</c> created in <c>Initialize()</c>.</summary>
        public static TestServer GetTestServer(this ContextBuilder contextBuilder) => contextBuilder.GetInstance<IWrapper>().GetTestServer();
        /// <summary>Get the ASP.NET Core client from the <c>TestServer</c> created in <c>Initialize()</c>.</summary>
        public static HttpClient GetHttpClient(this ContextBuilder contextBuilder) => contextBuilder.GetInstance<IWrapper>().GetHttpClient();
        /// <summary>Get the ASP.NET Core client from the factory created in <c>Initialize()</c>.</summary>

        private interface IWrapper
        {
            TestServer GetTestServer();
            HttpClient GetHttpClient();
        }

        /// <summary>Get client and test server from ctor.</summary>
        private class Wrapper : IWrapper
        {
            private readonly TestServer _testServer;
            private readonly HttpClient _client;

            public Wrapper(TestServer testServer, HttpClient client) // We do not own these, we don't dispose.
            {
                _testServer = testServer;
                _client = client;
            }
            public TestServer GetTestServer() => _testServer;
            public HttpClient GetHttpClient() => _client;
        }
    }
}