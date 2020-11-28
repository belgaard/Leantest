using LeanTest.Core.ExecutionHandling;
using Microsoft.Extensions.Hosting;
using System;

namespace LeanTest
{
	/// <summary>This is used to integrate the .Net Core IHost builder pattern with the Lean Test context builder pattern.</summary>
	public static class HostedServiceContextBuilderFactory
	{
		private static ITestServiceRunner _wrapped;

		/// <summary>
		/// .NET Core version of setting up IoC and builders.		
		/// </summary>
		/// <param name="mode">Always use ReCreate to recreate the IoC container before each test.</param>
		/// <param name="hostBuilder">The .NET Core host builder from production code, e.g '() => Program.CreateHostBuilder()' overridden with mocks using .ConfigureTestServices(). </param>
		/// <param name="iocContainerFactory">Creates the IoC container, such as e.g. 'provider => new IocContainer(provider)'.</param>
		public static void Initialize(CleanContextMode mode, Func<IHostBuilder> hostBuilder, Func<IServiceProvider, IIocContainer> iocContainerFactory) =>
			ContextBuilderFactory.Initialize(mode, () =>
			{
				_wrapped?.Dispose();

				var host = hostBuilder().Build();
				_wrapped = new TestServiceRunner(host);

				IServiceProvider serviceProvider = host.Services;

				return iocContainerFactory(serviceProvider);
			});

		/// <summary>
		/// Cleans the resources used by the HostedServiceContext.
		/// Also invokes the default ContextBuilderFactory.Cleanup
		/// </summary>
		public static void Cleanup()
		{
			_wrapped.Dispose();
			ContextBuilderFactory.Cleanup();
		}
		
		/// <summary>
		/// Returns the ITestServiceRunner which runs your hosted services under test
		/// </summary>
		/// <param name="contextBuilder"></param>
		/// <returns></returns>
		public static ITestServiceRunner GetTestServiceRunner(this ContextBuilder contextBuilder) => _wrapped;
	}

}