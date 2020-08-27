using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using LeanTest.Core.ExecutionHandling;
using LeanTest.L0Tests.Readers;
using LeanTest.L0Tests.TestData;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LeanTest.L0Tests
{
    [TestClass]
    public class TestContextBuilder
    {
        private ContextBuilder ContextBuilder { get => _contextBuilder.Value; set => _contextBuilder.Value = value; }
        private DataWithOneMockReader WithOneMockReader { get => _dataWithOneMockReader.Value; set => _dataWithOneMockReader.Value = value; }
        private DataWithOneStateHandlerReader WithOneStateHandlerReader { get => _dataWithOneStateHandlerReader.Value; set => _dataWithOneStateHandlerReader.Value = value; }
        private DataWithTwoMocksReader DataWithTwoMocksReader { get => _dataWithTwoMocksReader.Value; set => _dataWithTwoMocksReader.Value = value; }
        private DataWithTwoStateHandlersReader WithTwoStateHandlersReader { get => _dataWithTwoStateHandlersReader.Value; set => _dataWithTwoStateHandlersReader.Value = value; }
        private DataWithOneMockAndOneStateHandlerReader DataWithOneMockAndStateHandlersReader { get => _dataWithOneMockAndStateHandlersReader.Value; set => _dataWithOneMockAndStateHandlersReader.Value = value; }

        private readonly ThreadLocal<DataWithOneMockReader> _dataWithOneMockReader = new ThreadLocal<DataWithOneMockReader>();
        private readonly ThreadLocal<DataWithTwoMocksReader> _dataWithTwoMocksReader = new ThreadLocal<DataWithTwoMocksReader>();
        private readonly ThreadLocal<DataWithOneStateHandlerReader> _dataWithOneStateHandlerReader = new ThreadLocal<DataWithOneStateHandlerReader>();
        private readonly ThreadLocal<DataWithTwoStateHandlersReader> _dataWithTwoStateHandlersReader = new ThreadLocal<DataWithTwoStateHandlersReader>();
        private readonly ThreadLocal<DataWithOneMockAndOneStateHandlerReader> _dataWithOneMockAndStateHandlersReader = new ThreadLocal<DataWithOneMockAndOneStateHandlerReader>();
        private readonly ThreadLocal<ContextBuilder> _contextBuilder = new ThreadLocal<ContextBuilder>();

        [TestInitialize]
        public void TestInitialize()
        {
            ContextBuilder = ContextBuilderFactory.CreateContextBuilder();
            WithOneMockReader = ContextBuilder.GetInstance<DataWithOneMockReader>();
            DataWithTwoMocksReader = ContextBuilder.GetInstance<DataWithTwoMocksReader>();
            WithOneStateHandlerReader = ContextBuilder.GetInstance<DataWithOneStateHandlerReader>();
            WithTwoStateHandlersReader = ContextBuilder.GetInstance<DataWithTwoStateHandlersReader>();
            DataWithOneMockAndStateHandlersReader = ContextBuilder.GetInstance<DataWithOneMockAndOneStateHandlerReader>();
        }

        [TestMethod]
        public void WithDataMustThrowArgumentExceptionWhenNoBuilderHasBeenRegistered()
        {
            ArgumentException actual = Assert.ThrowsException<ArgumentException>(() =>
                ContextBuilder
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
                ContextBuilder
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
            ContextBuilder
                .WithData(new DataWithOneMock())
                .Build();
        }

        [TestMethod]
        public void BuildMustPassTheDataToTheSingleMockWhenRegistered()
        {
            ContextBuilder
                .WithData(new DataWithOneMock {SomeData = "TheData"})
                .Build();

            Assert.AreEqual("TheData", WithOneMockReader.Query().SomeData, "Expected the data to be passed to the registered mock-for-data.");
        }

        [TestMethod]
        public void BuildMustPassTheDataToTwoMocksWhenRegistered()
        {
            ContextBuilder
                .WithData(new DataWithTwoMocks {SomeData = "TheData"})
                .Build();

            DataWithTwoMocks[] allDataInMocks = DataWithTwoMocksReader.Query().ToArray();
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
            ContextBuilder
                .WithData(new DataWithOneStateHandler {SomeData = "TheData"})
                .Build();

            Assert.AreEqual("TheData", WithOneStateHandlerReader.Query().SomeData, "Expected the data to be passed to the registered state handler.");
        }

        [TestMethod]
        public void BuildMustPassTheDataToTwoStateHandlersWhenRegistered()
        {
            ContextBuilder
                .WithData(new DataWithTwoStateHandlers {SomeData = "TheData"})
                .Build();

            DataWithTwoStateHandlers[] allDataInStateHandlers = WithTwoStateHandlersReader.Query().ToArray();
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
            ContextBuilder
                .WithEnumerableData(new List<DataWithOneMockAndOneStateHandler>
                    { new DataWithOneMockAndOneStateHandler {SomeData = "TheData"}})
                .Build();

            DataWithOneMockAndOneStateHandler[] allDataInStateHandlers = DataWithOneMockAndStateHandlersReader.Query().ToArray();
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
                ContextBuilder
                    .WithData(new DataForStateHandlerWhichThrowsInBuild())
                    .Build());
        }

        [TestMethod]
        public void WithClearDataStoreMustClearAllDataWhenCalled()
        {
            ContextBuilder
                .WithData(new DataWithOneMock {SomeData = "TheData"});
            DataWithOneMock firstPre = ContextBuilder.First<DataWithOneMock>();
            DataWithOneMock lastPre = ContextBuilder.Last<DataWithOneMock>();
            List<DataWithOneMock> allPre = ContextBuilder.All<DataWithOneMock>().ToList();

            ContextBuilder.WithClearDataStore();

            MultiAssertForTException.Aggregate<AssertFailedException>(
                () => Assert.IsTrue(allPre.Single() == firstPre, "Expected a single piece of data in the data store initially."),
                () => Assert.IsTrue(firstPre == lastPre, "Expected the first to be the last in the data store initially."),
                () => Assert.ThrowsException<KeyNotFoundException>(() => ContextBuilder.All<DataWithOneMock>(), 
                    "Expected an empty data store after WithClearDataStore.")
            );
        }
    }
}
