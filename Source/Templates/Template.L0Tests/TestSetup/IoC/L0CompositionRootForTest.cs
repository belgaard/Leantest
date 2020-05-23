using LeanTest.Core.ExecutionHandling;
using LeanTest.Mock;
using Microsoft.Extensions.DependencyInjection;
using Template.L0Tests.Application;
using Template.L0Tests.Domain;
using Template.L0Tests.Mocks;
using Template.L0Tests.StateHandlers;

namespace Template.L0Tests.TestSetup.IoC
{
	public static class L0CompositionRootForTest
	{
		public static IServiceCollection Initialize(IServiceCollection serviceCollection)
		{
			// Mock-for-data:
			serviceCollection.RegisterMockForData<IMyExternalService, MockMyExternalService, MyData, MyOtherData>();
 
			// State handlers:
			serviceCollection.RegisterStateHandler<MyStateHandler, MyData, MyOtherData>();
 
			return serviceCollection;
		}

		private static void RegisterMockForData<TInterface, TImplementation, TData1, TData2>(this IServiceCollection container)
			where TImplementation: class, TInterface, IMockForData<TData1>, IMockForData<TData2>
			where TInterface: class
		{
			container.AddSingleton<TImplementation>();
			container.AddSingleton<TInterface>(x => x.GetRequiredService<TImplementation>());
			container.AddSingleton<IMockForData<TData1>>(x => x.GetRequiredService<TImplementation>());
			container.AddSingleton<IMockForData<TData2>>(x => x.GetRequiredService<TImplementation>());
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