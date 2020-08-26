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
        private DataWithOneStateHandlerReader _dataWithOneStateHandlerReader;
        private DataWithTwoStateHandlersReader _dataWithTwoStateHandlersReader;
        private DataWithOneMockAndOneStateHandlerReader _dataWithOneMockAndStateHandlersReader;

        [TestInitialize]
        public void TestInitialize()
        {
            _contextBuilder = ContextBuilderFactory.CreateContextBuilder();
            _dataWithOneMockReader = _contextBuilder.GetInstance<DataWithOneMockReader>();
            _dataWithTwoMocksReader = _contextBuilder.GetInstance<DataWithTwoMocksReader>();
            _dataWithOneStateHandlerReader = _contextBuilder.GetInstance<DataWithOneStateHandlerReader>();
            _dataWithTwoStateHandlersReader = _contextBuilder.GetInstance<DataWithTwoStateHandlersReader>();
            _dataWithOneMockAndStateHandlersReader = _contextBuilder.GetInstance<DataWithOneMockAndOneStateHandlerReader>();
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
        public void WithDataPreRegistrationMustThrowArgumentExceptionWhenNoBuilderHasBeenRegistered()
        {
            ArgumentException actual = Assert.ThrowsException<ArgumentException>(() =>
                _contextBuilder
                    .WithData<DataWithNoHandler>()
                    .WithData<AlsoDataWithNoHandler>()
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
            var actions = new List<Action> 
            {
                () => Assert.AreEqual(2, allDataInMocks.Length, "Expected exactly two mocks."),
                () => Assert.AreEqual("TheData", allDataInMocks[0].SomeData, "Expected the data to be passed to the first registered mock-for-data."),
                () => Assert.AreEqual("TheData", allDataInMocks[1].SomeData, "Expected the data to be passed to the second registered mock-for-data.")
            };
            MultiAssertForTException.Aggregate<AssertFailedException>(actions.ToArray());
        }

        [TestMethod]
        public void BuildMustPassTheDataToTheSingleStateHandlerWhenRegistered()
        {
            _contextBuilder
                .WithData(new DataWithOneStateHandler {SomeData = "TheData"})
                .Build();

            Assert.AreEqual("TheData", _dataWithOneStateHandlerReader.Query().SomeData, "Expected the data to be passed to the registered state handler.");
        }

        [TestMethod]
        public void BuildMustPassTheDataToTwoStateHandlersWhenRegistered()
        {
            _contextBuilder
                .WithData(new DataWithTwoStateHandlers {SomeData = "TheData"})
                .Build();

            DataWithTwoStateHandlers[] allDataInStateHandlers = _dataWithTwoStateHandlersReader.Query().ToArray();
            var actions = new List<Action> 
            {
                () => Assert.AreEqual(2, allDataInStateHandlers.Length, "Expected exactly two state handlers."),
                () => Assert.AreEqual("TheData", allDataInStateHandlers[0].SomeData, "Expected the data to be passed to the first registered state handler."),
                () => Assert.AreEqual("TheData", allDataInStateHandlers[1].SomeData, "Expected the data to be passed to the second registered state handler.")
            };
            MultiAssertForTException.Aggregate<AssertFailedException>(actions.ToArray());
        }

        [TestMethod]
        public void BuildMustPassTheDataToBothStateHandlerAndMockForDataWhenRegistered()
        {
            _contextBuilder
                .WithEnumerableData(new List<DataWithOneMockAndOneStateHandler>
                    { new DataWithOneMockAndOneStateHandler {SomeData = "TheData"}})
                .Build();

            DataWithOneMockAndOneStateHandler[] allDataInStateHandlers = _dataWithOneMockAndStateHandlersReader.Query().ToArray();
            var actions = new List<Action> 
            {
                () => Assert.AreEqual(2, allDataInStateHandlers.Length, "Expected exactly two state handlers."),
                () => Assert.AreEqual("TheData", allDataInStateHandlers[0].SomeData, "Expected the data to be passed to the first registered state handler."),
                () => Assert.AreEqual("TheData", allDataInStateHandlers[1].SomeData, "Expected the data to be passed to the second registered state handler.")
            };
            MultiAssertForTException.Aggregate<AssertFailedException>(actions.ToArray());
        }

        [TestMethod]
        public void BuildMustThrowExceptionFromStateHandlerWhenItThrowsInBuild()
        {
            Assert.ThrowsException<Exception>(() =>
                _contextBuilder
                    .WithData(new DataForStateHandlerWhichThrowsInBuild())
                    .Build());
        }

        [TestMethod]
        public void WithClearDataStoreMustClearAllDataWhenCalled()
        {
            _contextBuilder
                .WithData(new DataWithOneMock {SomeData = "TheData"});
            DataWithOneMock firstPre = _contextBuilder.First<DataWithOneMock>();
            DataWithOneMock lastPre = _contextBuilder.Last<DataWithOneMock>();
            List<DataWithOneMock> allPre = _contextBuilder.All<DataWithOneMock>().ToList();

            _contextBuilder.WithClearDataStore();

            MultiAssertForTException.Aggregate<AssertFailedException>(
                () => Assert.IsTrue(allPre.Single() == firstPre, "Expected a single piece of data in the data store initially."),
                () => Assert.IsTrue(firstPre == lastPre, "Expected the first to be the last in the data store initially."),
                () => Assert.ThrowsException<KeyNotFoundException>(() => _contextBuilder.All<DataWithOneMock>(), 
                    "Expected an empty data store after WithClearDataStore.")
            );
        }
    }
}
