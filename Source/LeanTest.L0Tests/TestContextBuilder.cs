using System;
using System.Collections.Generic;
using System.Linq;
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
	    private DataWithOneMockReader _dataWithOneMockReader;
	    private DataWithTwoMocksReader _dataWithTwoMocksReader;

	    [TestInitialize]
	    public void TestInitialize()
	    {
		    _contextBuilder = ContextBuilderFactory.CreateContextBuilder();
		    _dataWithOneMockReader = _contextBuilder.GetInstance<DataWithOneMockReader>();
		    _dataWithTwoMocksReader = _contextBuilder.GetInstance<DataWithTwoMocksReader>();
	    }

	    [TestMethod]
	    public void WithDataMustThrowArgumentExceptionWhenNoBuilderHasBeenRegistered()
	    {
			ArgumentException actual = Assert.ThrowsException<ArgumentException>(() =>
			    _contextBuilder
				    .WithData(new DataWithNoHandler())
				    .WithData(new AlsoDataWithNoHandler())
				    .Build());

			MultiAssertForTException.Aggregate<AssertFailedException>(
				() => Assert.IsTrue(actual.Message.Contains(nameof(DataWithNoHandler)), 
					$"Expected the exception to mention the type '{nameof(DataWithNoHandler)}' as not registered."),
				() => Assert.IsTrue(actual.Message.Contains(nameof(AlsoDataWithNoHandler)), 
					$"Expected the exception to mention the type '{nameof(AlsoDataWithNoHandler)}' as not registered."));
	    }

	    [TestMethod]
	    public void WithDataMustNotThrowWhenABuilderHasBeenRegistered()
	    {
		    _contextBuilder
			    .WithData(new DataWithOneMock())
			    .Build();
	    }

	    [TestMethod]
	    public void BuildMustPassTheDataToTheSingleMockWhenRegistered()
	    {
		    _contextBuilder
			    .WithData(new DataWithOneMock {SomeData = "TheData"})
			    .Build();

		    Assert.AreEqual("TheData", _dataWithOneMockReader.Query().SomeData, "Expected the data to be passed to the registered mock-for-data.");
	    }

	    [TestMethod]
	    public void BuildMustPassTheDataToTwoMocksWhenRegistered()
	    {
		    _contextBuilder
			    .WithData(new DataWithTwoMocks {SomeData = "TheData"})
			    .Build();

		    DataWithTwoMocks[] allDataInMocks = _dataWithTwoMocksReader.Query().ToArray();
		    var actions = new List<Action> {() => 
			    Assert.AreEqual(2, allDataInMocks.Length)};
			actions.AddRange(allDataInMocks.Select<DataWithTwoMocks, Action>(data => () => 
				Assert.AreEqual("TheData", data.SomeData, "Expected the data to be passed to the registered mock-for-data.")));
		    MultiAssertForTException.Aggregate<AssertFailedException>(actions.ToArray());
	    }
    }
}
