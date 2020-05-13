using LeanTest.L0Tests.ExternalDependencies;
using LeanTest.L0Tests.Mocks;
using LeanTest.L0Tests.Readers;
using LeanTest.L0Tests.TestData;
using LeanTest.Mock;
using Microsoft.Extensions.DependencyInjection;

namespace LeanTest.L0Tests.TestSetup.IoC
{
	public static class L0CompositionRootForTest
	{
		public static IServiceCollection Initialize(IServiceCollection serviceCollection)
		{
			// Mock-for-data:
			serviceCollection.RegisterMockForData<IExternalDependency, MockForDataExternalDependency, RegisteredData>();

			// Readers:
			serviceCollection.AddSingleton<RegisteredDataReader>();

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
	}
}