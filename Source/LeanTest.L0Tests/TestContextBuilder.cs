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
	    private RegisteredDataReader _registeredDataReader;

	    [TestInitialize]
	    public void TestInitialize()
	    {
		    _contextBuilder = ContextBuilderFactory.CreateContextBuilder();
		    _registeredDataReader = _contextBuilder.GetInstance<RegisteredDataReader>();
	    }

	    [TestMethod]
	    public void WithDataMustThrowArgumentExceptionWhenNoBuilderHasBeenRegistered()
	    {
			ArgumentException actual = Assert.ThrowsException<ArgumentException>(() =>
			    _contextBuilder
				    .WithData(new NonRegisteredData())
				    .WithData(new AlsoNonRegisteredData())
				    .Build());

			MultiAssertForTException.Aggregate<AssertFailedException>(
				() => Assert.IsTrue(actual.Message.Contains("NonRegisteredData"), "Expected the exception to mention the type 'NonRegisteredData' as not registered."),
				() => Assert.IsTrue(actual.Message.Contains("AlsoNonRegisteredData"), "Expected the exception to mention the type 'AlsoNonRegisteredData' as not registered."));
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

		    Assert.AreEqual("TheData", _registeredDataReader.Query().SomeData, "Expected the data to be passed to the registered mock-for-data.");
	    }
    }
}
