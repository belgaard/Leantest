using System.Collections.Generic;
using LeanTest.L0Tests.Mocks;
using LeanTest.L0Tests.StateHandlers;
using LeanTest.L0Tests.TestData;

namespace LeanTest.L0Tests.Readers
{
	internal class DataWithTwoStateHandlersReader
	{
		private readonly StateHandlerExternalDependency _stateHandler1;
		private readonly AnotherStateHandlerExternalDependency _stateHandler2;

		public DataWithTwoStateHandlersReader(StateHandlerExternalDependency stateHandler1, AnotherStateHandlerExternalDependency stateHandler2)
		{
			_stateHandler1 = stateHandler1;
			_stateHandler2 = stateHandler2;
		}

		public IEnumerable<DataWithTwoStateHandlers> Query() => 
			new List<DataWithTwoStateHandlers> { _stateHandler1.DataWithTwoStateHandlers, _stateHandler2.DataWithTwoStateHandlers};
	}
	internal class DataWithOneMockAndOneStateHandlerReader
	{
		private readonly StateHandlerExternalDependency _stateHandler;
		private readonly MockForDataExternalDependency _mock;

		public DataWithOneMockAndOneStateHandlerReader(StateHandlerExternalDependency stateHandler, MockForDataExternalDependency mock)
		{
			_stateHandler = stateHandler;
			_mock = mock;
		}

		public IEnumerable<DataWithOneMockAndOneStateHandler> Query() => 
			new List<DataWithOneMockAndOneStateHandler> { _stateHandler.DataWithOneMockAndOneStateHandler, _mock.DataWithOneMockAndOneStateHandler};
	}
}