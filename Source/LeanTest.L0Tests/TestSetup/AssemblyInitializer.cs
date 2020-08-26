using Microsoft.VisualStudio.TestTools.UnitTesting;
using LeanTest.Core.ExecutionHandling;
using LeanTest.L0Tests.TestSetup.IoC;
using Microsoft.Extensions.DependencyInjection;

[assembly: Parallelize(Workers = 0, Scope = ExecutionScope.MethodLevel)]

namespace LeanTest.L0Tests.TestSetup
{
	[TestClass]
	public static class AssemblyInitializer
	{
		[AssemblyInitialize]
		public static void AssemblyInitialize(TestContext _)
		{
			static IIocContainer IocFactory() => new IocContainer(L0CompositionRootForTest.Initialize(new ServiceCollection()));
			ContextBuilderFactory.Initialize(IocFactory);
		}
 
		[AssemblyCleanup]
		public static void AssemblyCleanup() => ContextBuilderFactory.Cleanup();
	}
}
