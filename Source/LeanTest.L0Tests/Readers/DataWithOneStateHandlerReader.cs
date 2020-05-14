using LeanTest.L0Tests.StateHandlers;
using LeanTest.L0Tests.TestData;

namespace LeanTest.L0Tests.Readers
{
	internal class DataWithOneStateHandlerReader
	{
		private readonly StateHandlerExternalDependency _stateHandler;

		public DataWithOneStateHandlerReader(StateHandlerExternalDependency stateHandler) => _stateHandler = stateHandler;
		public DataWithOneStateHandler Query() => _stateHandler.DataWithOneStateHandler;
	}
}