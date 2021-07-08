using System.Threading.Tasks;
using Examples.L0Tests.Application;
using Examples.L0Tests.Domain;
using Examples.L0Tests.TestSetup;
using LeanTest.Core.ExecutionHandling;
using LeanTest.MSTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TestScenarioIdAttribute = LeanTest.Attribute.TestScenarioIdAttribute;

namespace Examples.L0Tests
{
	[TestClass]
	public class TestMyApplicationService
	{
		private ContextBuilder _contextBuilder;
		private MyApplicationService _target;
		// Populated by MS Test:
		public TestContext TestContext { get; set; }

		[TestInitialize]
		public void TestInitialize()
		{
			_contextBuilder = new MyContextBuilderFactory().ContextBuilder
				.WithData<MyData>()
				.RegisterAttributes(TestContext);

			_target = _contextBuilder.GetInstance<MyApplicationService>();
		}

		[TestMethod, TestScenarioId("SimpleExamples - Obsolete"), LeanTest.Attribute.TestScenarioId("SimpleExamples")]
		public async Task GetAgeAsyncMustNotThrow()
		{
			_contextBuilder
				.WithData(new MyData { Age = 10, Key = "ac_32_576259321" })
				.WithData(new MyOtherData { OtherAge = 10, OtherKey = "ac_32_576259321" })
				.Build();

			await ExceptionAssert.DoesNotThrowAsync(() => _target.GetAgeAsync("FortyTwo"),
				$"Expected '{nameof(MyApplicationService.GetAgeAsync)}' not to throw").ConfigureAwait(false);
		}

		#region Example of existing state
		[TestMethod, TestScenarioId("SimpleExamples")]
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