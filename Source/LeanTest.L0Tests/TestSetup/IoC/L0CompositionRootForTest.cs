using LeanTest.Core.ExecutionHandling;
using LeanTest.L0Tests.ExternalDependencies;
using LeanTest.L0Tests.Mocks;
using LeanTest.L0Tests.Readers;
using LeanTest.L0Tests.StateHandlers;
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
			serviceCollection.RegisterMockForData<IExternalDependency, MockForDataExternalDependency, 
				DataWithOneMock, DataWithTwoMocks, DataWithOneMockAndOneStateHandler>();
			serviceCollection.RegisterMockForData<IExternalDependency, AnotherMockForDataExternalDependency, 
				DataWithTwoMocks>();

			// State handlers:
			serviceCollection.RegisterStateHandler<StateHandlerExternalDependency, 
				DataWithOneStateHandler, DataWithTwoStateHandlers, DataWithOneMockAndOneStateHandler>();
			serviceCollection.RegisterStateHandler<AnotherStateHandlerExternalDependency, 
				DataWithTwoStateHandlers>();

			// Readers:
			serviceCollection.AddSingleton<DataWithOneMockReader>();
			serviceCollection.AddSingleton<DataWithTwoMocksReader>();
			serviceCollection.AddSingleton<DataWithOneStateHandlerReader>();
			serviceCollection.AddSingleton<DataWithTwoStateHandlersReader>();
			serviceCollection.AddSingleton<DataWithOneMockAndOneStateHandlerReader>();

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

		private static void RegisterMockForData<TInterface, TImplementation, TData1, TData2, TData3>(this IServiceCollection container)
			where TImplementation: class, TInterface, IMockForData<TData1>, IMockForData<TData2>, IMockForData<TData3>
			where TInterface: class
		{
			container.AddSingleton<TImplementation>();
			container.AddSingleton<TInterface>(x => x.GetRequiredService<TImplementation>());
			container.AddSingleton<IMockForData<TData1>>(x => x.GetRequiredService<TImplementation>());
			container.AddSingleton<IMockForData<TData2>>(x => x.GetRequiredService<TImplementation>());
			container.AddSingleton<IMockForData<TData3>>(x => x.GetRequiredService<TImplementation>());
		}

		private static void RegisterStateHandler<TImplementation, TData1>(this IServiceCollection container)
			where TImplementation: class, IStateHandler<TData1>
		{
			container.AddSingleton<TImplementation>();
			container.AddSingleton<IStateHandler<TData1>>(x => x.GetRequiredService<TImplementation>());
		}

		private static void RegisterStateHandler<TImplementation, TData1, TData2, TData3>(this IServiceCollection container)
			where TImplementation: class, IStateHandler<TData1>, IStateHandler<TData2>, IStateHandler<TData3>
		{
			container.AddSingleton<TImplementation>();
			container.AddSingleton<IStateHandler<TData1>>(x => x.GetRequiredService<TImplementation>());
			container.AddSingleton<IStateHandler<TData2>>(x => x.GetRequiredService<TImplementation>());
			container.AddSingleton<IStateHandler<TData3>>(x => x.GetRequiredService<TImplementation>());
		}
	}
}