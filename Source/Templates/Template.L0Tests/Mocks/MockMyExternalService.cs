using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using LeanTest.Mock;
using Template.L0Tests.Application;
using Template.L0Tests.Domain;

namespace Template.L0Tests.Mocks
{
	[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "Instantiated by the IoC container")]
	internal class MockMyExternalService : IMockForData<MyData>, IMockForData<MyOtherData>, IMyExternalService
	{
		private List<MyData> MyData { get; } = new List<MyData>();

		public int GetAge(string key) => MyData.First().Age;
		public async Task<int> GetAgeAsync(string key) => await Task.FromResult(MyData.First().Age);
		public void WithData(MyOtherData data) { }
		public void PreBuild() { }
		public void Build(Type type) { }
		public void PostBuild() {}
		public void WithData(MyData data) => MyData.Add(data);
	}
}
