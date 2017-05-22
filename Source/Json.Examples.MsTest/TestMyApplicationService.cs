using Core.Examples.MsTest.Application;
using Core.Examples.MsTest.IoC;
using LeanTest.Core.ExecutionHandling;
using LeanTest.JSon;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Json.Examples.MsTest
{
    /// <summary>
    /// Work-in-progress. 
    /// Note that in a real-worl example we must choose an IoC container - here we have implemented our own for
    /// this example. See the ValueAdd solution for examples of using Unity. 
    /// This container must be initialized in an AssemblyInitializer class - refer to this class to see how it is
    /// done with our example IoC container.
    /// For the example to be more complete, there should be two projects, one for test one for what is being tested.
    /// </summary>
    [TestClass]
    public class TestMyApplicationService
    {
        private ContextBuilder _contextBuilder;
        private MyApplicationService _target;

        [TestInitialize]
        public void TestInitialize()
        {
            _contextBuilder = ContextBuilderFactory.CreateContextBuilder()
                .Build();

            _target = _contextBuilder.GetInstance<MyApplicationService>();
        }

        [TestMethod]
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
