using Core.Examples.L0Tests.Application;
using Core.Examples.L0Tests.TestSetup.IoC;
using LeanTest.Core.ExecutionHandling;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Examples.L0Tests.TestSetup
{
	[TestClass]
	public static class AssemblyInitializer
	{
		[AssemblyInitialize]
		public static void AssemblyInitialize(TestContext _)
		{
			IIocContainer IocFactory() => new IocContainer(L0CompositionRootForTest.Initialize(CompositionRoot.Initialize(new ServiceCollection())));
			ContextBuilderFactory.Initialize(CleanContextMode.ReCreate, IocFactory);
		}
 
		[AssemblyCleanup]
		public static void AssemblyCleanup() => ContextBuilderFactory.Cleanup();
	}
}
