using System;
using LeanTest.Core.ExecutionHandling;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace LeanTest
{
	/// <summary>
	/// This web application factory encapsulates factories for initializing a composition root and an IoC container, respectively.
	/// These factories are used to create a context builder in the implementation of the <c>ICreateContextBuilder</c> interface.
	/// The .NET Core web application factory is documented here
	/// https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.mvc.testing.webapplicationfactory-1?view=aspnetcore-3.0&viewFallbackFrom=aspnetcore-3.1
	/// </summary>
	/// <typeparam name="TEntryPoint">The entry point, typically the <c>Startup</c> class.</typeparam>
	/// <remarks>Note that lean testing encourages that a context builder, and thus the IoC container, is created from scratch before each test.
	/// This class makes it easy to follow this principle.</remarks>
	public class LeanTestWebApplicationFactory<TEntryPoint> : WebApplicationFactory<TEntryPoint>, IFactoryAccess, ICreateContextBuilder where TEntryPoint : class
	{
		private readonly Action<IServiceCollection> _initializeCompositionRoot;
		private readonly Func<IServiceProvider, IIocContainer> _iocContainerFactory;

		/// <summary>ctor</summary>
		/// <param name="initializeCompositionRoot">Factory for initializing the composition root.</param>
		/// <param name="iocContainerFactory">Factory for creating an IoC container fro scratch.</param>
		public LeanTestWebApplicationFactory(Action<IServiceCollection> initializeCompositionRoot, Func<IServiceProvider, IIocContainer> iocContainerFactory)
		{
			_initializeCompositionRoot = initializeCompositionRoot;
			_iocContainerFactory = iocContainerFactory;
		}

		/// <inheritdoc />
		public ContextBuilder CreateContextBuilder => new WebApplicationFactoryContextBuilder<TEntryPoint>(this, _iocContainerFactory(Services));

		/// <inheritdoc />
		protected override void ConfigureWebHost(IWebHostBuilder builder) => builder.ConfigureTestServices(_initializeCompositionRoot);
	}
}