using System;
using System.Collections.Generic;
using LeanTest.Core.ExecutionHandling;
using Microsoft.Extensions.DependencyInjection;

namespace LeanTest.L0Tests.TestSetup.IoC
{
	public class IocContainer : IIocContainer
	{
		private readonly IServiceProvider _serviceProvider;
		public IocContainer(IServiceCollection serviceCollection) => _serviceProvider = serviceCollection.BuildServiceProvider();
 
		public T Resolve<T>() where T : class => _serviceProvider.GetRequiredService<T>();
		public T TryResolve<T>() where T : class => _serviceProvider.GetService<T>();
		public IEnumerable<T> TryResolveAll<T>() where T : class => _serviceProvider.GetServices<T>();
	}
}