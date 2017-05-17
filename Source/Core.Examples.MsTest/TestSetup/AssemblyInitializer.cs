using System;
using Core.Examples.MsTest.Domain;
using LeanTest.Core.ExecutionHandling;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Examples.MsTest.TestSetup
{
    [TestClass]
    public static class AssemblyInitializer
    {
        [AssemblyInitialize]
        public static void AssemblyInitialize(TestContext testContext) => ContextBuilderFactory.Initialize(() => new MyOwnIoC());
    }
}
