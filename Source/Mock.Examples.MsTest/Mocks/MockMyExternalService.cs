using System.Linq;
using ExampleApp.Application;
using ExampleApp.Domain;
using LeanTest.Mock;

namespace Mock.Examples.MsTest.Mocks
{
    internal class MockMyExternalService : TypedData<MyData>, IMockForData<MyData>, IMyExternalService
    {
        public int GetAge(string key) => Data.First().Age;
    }
}
