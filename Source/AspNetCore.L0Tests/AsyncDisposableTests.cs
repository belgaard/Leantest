using AspNetCore.L0Tests.Helpers;
using LeanTest.Core.ExecutionHandling;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace AspNetCore.L0Tests
{
	[TestClass]
	public class AsyncDisposableTests
	{
		private ContextBuilder _contextBuilder;

		[TestInitialize]
		public void TestInitialize()
		{
			_contextBuilder = ContextBuilderFactory.CreateContextBuilder();
		}

		[TestMethod]
		[DataRow(1)]
		[DataRow(2)]
		public async Task AsyncDisposableServiceIsDisposedAfterTest(int _)
		{
			// We just need to resolve the service to have it tracked by the context builder and disposed at the end of the test
			_contextBuilder.GetInstance<DelayedAsyncDisposableService>();
		}
	}
}
