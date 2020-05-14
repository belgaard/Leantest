using System.Diagnostics.CodeAnalysis;
using System.Linq;
using LeanTest.Core.ExecutionHandling;
using LeanTest.L0Tests.TestData;
using LeanTest.Mock;

namespace LeanTest.L0Tests.StateHandlers
{
	[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "Instantiated by the IoC container.")]
	internal class StateHandlerExternalDependency : TypedData<DataWithOneStateHandler>,
		IStateHandler<DataWithOneStateHandler>, IStateHandler<DataWithTwoStateHandlers>, IStateHandler<DataWithOneMockAndOneStateHandler>
	{
		internal DataWithOneStateHandler DataWithOneStateHandler => Data.First();
		internal DataWithTwoStateHandlers DataWithTwoStateHandlers { get; private set; }
		internal DataWithOneMockAndOneStateHandler DataWithOneMockAndOneStateHandler { get; private set; }
		public void WithData(DataWithTwoStateHandlers data) => DataWithTwoStateHandlers = data;
		public void WithData(DataWithOneMockAndOneStateHandler data) => DataWithOneMockAndOneStateHandler = data;
	}
}