using System.Linq;
using LeanTest.Mock;
using Mock.Examples.MsTest.Application;
using Mock.Examples.MsTest.Domain;

namespace Mock.Examples.MsTest.Mocks
{
    internal class MockMyExternalService : TypedData<MyData>, IMockForData<MyData>, IMyExternalService
    {
        public int GetAge(string key) => Data.First().Age;
    }
}
