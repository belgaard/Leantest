using System;
using System.Collections.Generic;
using LeanTest.Core.ExecutionHandling;
using Microsoft.Extensions.DependencyInjection;

namespace LeanTest.DI.DotNetCore
{
	/// <inheritdoc/>
	public class IocContainer : IIocContainer
	{
		private readonly IServiceProvider _serviceProvider;

		/// <summary>The ctor requires a service provider, you can generate it from a service collection using <c>.BuildServiceProvider()</c></summary>
		public IocContainer(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

		/// <inheritdoc/>
		/// <remarks>Gets a required service from the service provider.</remarks>
		public T Resolve<T>() where T : class => _serviceProvider.GetRequiredService<T>();
		/// <inheritdoc/>
		public T TryResolve<T>() where T : class => _serviceProvider.GetService<T>();
		/// <inheritdoc/>
		/// <remarks>The full set of mock-for-data and state handlers is returned.</remarks>
		public IEnumerable<T> TryResolveAll<T>() where T : class => _serviceProvider.GetServices<T>();
	}
}
