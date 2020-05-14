using System.Collections;
using System.Collections.Generic;
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
	internal class DataWithTwoMocksReader
	{
		private readonly MockForDataExternalDependency _mock1;
		private readonly AnotherMockForDataExternalDependency _mock2;

		public DataWithTwoMocksReader(MockForDataExternalDependency mock1, AnotherMockForDataExternalDependency mock2)
		{
			_mock1 = mock1;
			_mock2 = mock2;
		}

		public IEnumerable<DataWithTwoMocks> Query() => 
			new List<DataWithTwoMocks> { _mock1.DataWithTwoMocks, _mock2.DataWithTwoMocks};
	}
}