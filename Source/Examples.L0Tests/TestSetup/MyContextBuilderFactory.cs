using LeanTest.DI.DotNetCore;
using Examples.L0Tests.Application;
using Examples.L0Tests.TestSetup.IoC;
using LeanTest.Core.ExecutionHandling;
using Microsoft.Extensions.DependencyInjection;

namespace Examples.L0Tests.TestSetup
{
	/// <summary>Does the setup which must be done consistently across all tests in the assembly.</summary>
	public class MyContextBuilderFactory
	{
		public ContextBuilder ContextBuilder {get;}
		/// <summary>Use the production code composition root, let the test composition root override what must be mocked, wrap the chosen DI container, 
		/// then create the context.</summary>
		public MyContextBuilderFactory()
		{
			// Production code composition root with test overrides:
			IServiceCollection serviceCollection = L0CompositionRootForTest.Initialize(CompositionRoot.Initialize(new ServiceCollection()));
			// Wrap the .NET Core/.NET 8 DI container to be used by LeanTest:
			IocContainer container = new IocContainer(serviceCollection.BuildServiceProvider());
			// Create the context:
			ContextBuilder = new ContextBuilder(container);
		}
	}
}
