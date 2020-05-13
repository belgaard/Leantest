using System;
using LeanTest.Core.ExecutionHandling;
using LeanTest.L0Tests.Readers;
using LeanTest.L0Tests.TestData;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LeanTest.L0Tests
{
    [TestClass]
    public class TestContextBuilder
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
	    public void WithDataMustNotThrowWhenABuilderHasBeenRegistered()
	    {
		    _contextBuilder
			    .WithData(new RegisteredData())
			    .Build();
	    }

	    [TestMethod]
	    public void BuildMustPassTheDataToTheMockWhenRegistered()
	    {
		    _contextBuilder
			    .WithData(new RegisteredData {SomeData = "TheData"})
			    .Build();

		    Assert.AreEqual("TheData", _contextBuilder.GetInstance<RegisteredDataReader>().Query().SomeData);
	    }
    }
}
