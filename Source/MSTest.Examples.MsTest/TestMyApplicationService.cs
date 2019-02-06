using System.Diagnostics.CodeAnalysis;
using Core.Examples.MsTest.Application;
using Core.Examples.MsTest.IoC;
using LeanTest.Core.ExecutionHandling;
using LeanTest.JSon;
using LeanTest.MSTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MSTest.Examples.MsTest
{
    [TestClass]
    public class TestMyApplicationService
    {
        private ContextBuilder _contextBuilder;
        private MyApplicationService _target;
	    [SuppressMessage("ReSharper", "MemberCanBePrivate.Global", Justification = "MS Test requires it to be public")]
	    [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global", Justification = "Initialized by MsTest")]
	    public TestContext TestContext { get; set; }

		[TestInitialize]
        public void TestInitialize()
        {
            _contextBuilder = ContextBuilderFactory.CreateContextBuilder()
	            .RegisterScenarioId(TestContext)
                .Build();

            _target = _contextBuilder.GetInstance<MyApplicationService>();
        }

        [TestMethod]
        [TestScenarioId("{439E173C-D2A8-4E1B-9D6D-B3EF9A72A3F9}")]
        public void SumMustReturn42When10And32ArePassedFromJson()
        {
            _contextBuilder
                .WithData<MyData>(TestData.MyData.TenAndThirtyTwo)
                .Build();

            int actual = _target.Sum(_contextBuilder.First<MyData>());

            Assert.AreEqual(42, actual);
        }
    }
}
