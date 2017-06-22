using LeanTest.Core.ExecutionHandling;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using State.Examples.MsTest.IoC;

namespace State.Examples.MsTest.TestSetup
{
    [TestClass]
    public static class AssemblyInitializer
    {
        [AssemblyInitialize]
        public static void AssemblyInitialize(TestContext testContext)
        {
            ContextBuilderFactory.Initialize(() => new MyOwnIoC());
        }
    }
}
