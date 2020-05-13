using System;
using LeanTest.Core.ExecutionHandling;
using LeanTest.L0Tests.TestSetup.IoC;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LeanTest.L0Tests
{
    [TestClass]
    public class TestWithData
    {
	    private ContextBuilder _contextBuilder;

	    [TestInitialize]
	    public void TestInitialize()
	    {
		    _contextBuilder = ContextBuilderFactory.CreateContextBuilder();
	    }

	    [TestMethod]
	    public void WithDataMustThrowArgumentExceptionWhenNoBuilderHasBeenRegisteredForString()
	    {
			Assert.ThrowsException<ArgumentException>(() =>
			    _contextBuilder
				    .WithData("hey")
				    .Build());
	    }

	    [TestMethod]
	    public void WithDataMustNotThrowWhenABuilderHasBeenRegisteredForMy()
	    {
		    _contextBuilder
			    .WithData(new My())
			    .Build();
	    }
    }
}
