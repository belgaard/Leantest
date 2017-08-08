using System;
using System.Collections.Generic;
using System.Linq;
using ExampleApp.Domain;
using LeanTest.Mock;
using Mock.Examples.MsTest.Application;

namespace Mock.Examples.MsTest.Mocks
{
    internal class MockMyExternalService : IMockForData<MyData>, IMockForData<MyOtherData>, IMyExternalService
    {
        private List<MyData> MyData { get; } = new List<MyData>();

        public int GetAge(string key) => MyData.First().Age;
        public void WithData(MyOtherData data) { }
        public void PreBuild() { }
        public void Build(Type type) { }
        public void PostBuild() {}
        public void WithData(MyData data) => MyData.Add(data);
    }
}
