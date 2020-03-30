using System;
using System.Net.Http;
using LeanTest.Core.ExecutionHandling;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;

namespace LeanTest
{
	/// <summary>This is used to integrate the .Net Core web host builder pattern with the Lean Test context builder pattern.</summary>
	public static class AspNetCoreContextBuilderFactory
	{
		private static TestServer _testServer;
		private static HttpClient _client;

		/// <summary>ASP.NET Core version of setting up IoC and builders.</summary>
		/// <param name="mode">Always use ReCreate to recreate the IoC container before each test.</param>
		/// <param name="webHostBuilder">The ASP.NET Core web host builder from production code, overridden with mocks using .ConfigureTestServices().</param>
		/// <param name="iocContainerFactory">Creates the IoC container, such as e.g. 'provider => new IocContainer(provider)'.</param>
		/// <remarks>The passed in web host builder is used to create a <c>TestHost</c>.
		/// A client is created and can be accessed through <c>GetClient()</c>.
		/// The test host instance is disposed as the first step of initialisation.</remarks>
		public static void Initialize(CleanContextMode mode, Func<IWebHostBuilder> webHostBuilder, Func<IServiceProvider, IIocContainer> iocContainerFactory) =>
			ContextBuilderFactory.Initialize(mode, () =>
			{
				_testServer?.Dispose();
				_testServer = null;
				_client = null;

				_testServer = new TestServer(webHostBuilder());
				IServiceProvider serviceProvider = _testServer.Host.Services;
				_client = _testServer.CreateClient();

				return iocContainerFactory(serviceProvider);
			});

		/// <summary>ASP.NET Core version of setting up IoC and builders.</summary>
		/// <param name="mode">Always use ReCreate to recreate the IoC container before each test.</param>
		/// <param name="hostBuilder">The ASP.NET Core host builder from production code, overridden with mocks using .ConfigureTestServices().</param>
		/// <param name="iocContainerFactory">Creates the IoC container, such as e.g. 'provider => new IocContainer(provider)'.</param>
		/// <remarks>The passed in web host builder is used to create a <c>TestHost</c>.
		/// A client is created and can be accessed through <c>GetClient()</c>.
		/// The test host instance is disposed as the first step of initialisation.</remarks>
		public static void Initialize(CleanContextMode mode, Func<IHostBuilder> hostBuilder, Func<IServiceProvider, IIocContainer> iocContainerFactory) =>
			ContextBuilderFactory.Initialize(mode, () =>
			{
				_testServer?.Dispose();
				_testServer = null;
				_client = null;

				var host = hostBuilder().Build();
				host.Start();

				_testServer = host.GetTestServer();
				IServiceProvider serviceProvider = _testServer.Services;
				_client = _testServer.CreateClient();

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
		public static TestServer GetTestServer(this ContextBuilder _) => _testServer;

		/// <summary>Get the ASP.NET Core client from the <c>TestServer</c> created in <c>Initialize()</c>.</summary>
		public static HttpClient GetClient(this ContextBuilder _) => _client;
	}
}