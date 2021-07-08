using Examples.L0Tests.Application;
using Examples.L0Tests.TestSetup.IoC;
using LeanTest.Core.ExecutionHandling;
using Microsoft.Extensions.DependencyInjection;

namespace Examples.L0Tests.TestSetup
{
	/// <summary>Does the setup which must must be done consistently across all tests in the assembly.</summary>
	public class MyContextBuilderFactory
	{
		public ContextBuilder ContextBuilder {get;}
		public MyContextBuilderFactory() => 
			ContextBuilder = new ContextBuilder(new IocContainer(L0CompositionRootForTest.Initialize(CompositionRoot.Initialize(new ServiceCollection()))));
	}
}
