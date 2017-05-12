using System;
using LeanTest.Core.ExecutionHandling;
using LeanTest.Mock;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mock.Examples.MsTest.IoC;

namespace Mock.Examples.MsTest.TestSetup
{
    [TestClass]
    public static class AssemblyInitializer
    {
        [AssemblyInitialize]
        public static void AssemblyInitialize(TestContext testContext)
        {
            var iocFactory = new Func<IIocContainer>(() => new MyOwnIoC());

            ContextBuilderFactory.Initialize(CleanContextMode.ReCreate, iocFactory);
            MockingBuilderFactory.Initialize();
        }
    }
}
