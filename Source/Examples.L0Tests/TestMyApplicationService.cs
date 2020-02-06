using Examples.L0Tests.Application;
using Examples.L0Tests.Domain;
using LeanTest.Core.ExecutionHandling;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Examples.L0Tests
{
	[TestClass]
	public class TestMyApplicationService
	{
		private ContextBuilder _contextBuilder;
		private MyApplicationService _target;

		[TestInitialize]
		public void TestInitialize()
		{
			_contextBuilder = ContextBuilderFactory.CreateContextBuilder()
				.WithData<MyData>()
				.Build();

			_target = _contextBuilder.GetInstance<MyApplicationService>();
		}
		#region Example of existing state
		[TestMethod]
		public void GetAgeMustReturn10WhenKeyMatchesNewUpData()
		{
			#region Example of using a builder pattern
			_contextBuilder
				.WithData(new MyData { Age = 10, Key = "ac_32_576259321" })
				.WithData(new MyOtherData { OtherAge = 10, OtherKey = "ac_32_576259321" })
				.Build();
			#endregion

			int actual = _target.GetAge("FortyTwo");

			Assert.AreEqual(10, actual);
		}
		#endregion
	}
}
