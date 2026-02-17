using LeanTest.Core.ExecutionHandling;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;

namespace AspNetCore.L0Tests.TestSetup.IoC
{
	internal class IoCContainer : IIocContainer
	{
		private readonly IServiceProvider _serviceProvider;

		public IoCContainer(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;

		public T Resolve<T>() where T : class => _serviceProvider.GetRequiredService<T>();

		public T TryResolve<T>() where T : class => _serviceProvider.GetService<T>();

		public IEnumerable<T> TryResolveAll<T>() where T : class => _serviceProvider.GetServices<T>();
	}
}
