using LeanTest.L0Tests.Mocks;
using LeanTest.L0Tests.TestData;

namespace LeanTest.L0Tests.Readers
{
	internal class DataWithOneMockReader
	{
		private readonly MockForDataExternalDependency _mock;

		public DataWithOneMockReader(MockForDataExternalDependency mock) => _mock = mock;
		public DataWithOneMock Query() => _mock.DataWithOneMock;
	}
}