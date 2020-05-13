using LeanTest.L0Tests.Mocks;
using LeanTest.L0Tests.TestData;

namespace LeanTest.L0Tests.Readers
{
	internal class RegisteredDataReader
	{
		private readonly MockForDataExternalDependency _mock;

		public RegisteredDataReader(MockForDataExternalDependency mock) => _mock = mock;
		public RegisteredData Query() => _mock.RegisteredData;
	}
}