using System.Diagnostics.CodeAnalysis;
using LeanTest.Core.ExecutionHandling;
using LeanTest.Mock;
using Microsoft.Extensions.DependencyInjection;

namespace LeanTest.L0Tests.TestSetup.IoC
{
	internal class My {}
	public static class L0CompositionRootForTest
	{
		private interface IMyExternalDependency {}
		[SuppressMessage("ReSharper", "ClassNeverInstantiated.Local", Justification = "Instantiated by the IoC container.")]
		private class MockForDataMyExternalDependency : TypedData<My>, IMyExternalDependency, IMockForData<My> {}

		public static IServiceCollection Initialize(IServiceCollection serviceCollection)
		{
			// Mock-for-data:
			serviceCollection.RegisterMockForData<IMyExternalDependency, MockForDataMyExternalDependency, My>();

			// State handlers:
			//serviceCollection.RegisterStateHandler<MyStateHandler, MyData, MyOtherData>();

			return serviceCollection;
		}

		private static void RegisterMockForData<TInterface, TImplementation, TData1>(this IServiceCollection container)
			where TImplementation: class, TInterface, IMockForData<TData1>
			where TInterface: class
		{
			container.AddSingleton<TImplementation>();
			container.AddSingleton<TInterface>(x => x.GetRequiredService<TImplementation>());
			container.AddSingleton<IMockForData<TData1>>(x => x.GetRequiredService<TImplementation>());
		}

		private static void RegisterStateHandler<TImplementation, TData1, TData2>(this IServiceCollection container)
			where TImplementation: class, IStateHandler<TData1>, IStateHandler<TData2>
		{
			container.AddSingleton<TImplementation>();
			container.AddSingleton<IStateHandler<TData1>>(x => x.GetRequiredService<TImplementation>());
			container.AddSingleton<IStateHandler<TData2>>(x => x.GetRequiredService<TImplementation>());
		}
	}
}