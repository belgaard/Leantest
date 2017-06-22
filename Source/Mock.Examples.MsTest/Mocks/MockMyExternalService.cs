using System.Linq;
using ExampleApp.Domain;
using LeanTest.Mock;
using Mock.Examples.MsTest.Application;

namespace Mock.Examples.MsTest.Mocks
{
    internal class MockMyExternalService : TypedData<MyData>, IMockForData<MyData>, IMyExternalService
    {
        public int GetAge(string key) => Data.First().Age;
    }
}
