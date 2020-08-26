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
        [TestMethod]
        public void WithDataMustThrowArgumentExceptionWhenNoBuilderHasBeenRegistered()
        {
            ArgumentException actual = Assert.ThrowsException<ArgumentException>(() =>
                ContextBuilderFactory.CreateContextBuilder()
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
                ContextBuilderFactory.CreateContextBuilder()
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
            ContextBuilderFactory.CreateContextBuilder()
                .WithData(new DataWithOneMock())
                .Build();
        }

        [TestMethod]
        public void BuildMustPassTheDataToTheSingleMockWhenRegistered()
        {
            var contextBuilder = ContextBuilderFactory.CreateContextBuilder()
                .WithData(new DataWithOneMock {SomeData = "TheData"})
                .Build();

            Assert.AreEqual("TheData", contextBuilder.GetInstance<DataWithOneMockReader>().Query().SomeData, "Expected the data to be passed to the registered mock-for-data.");
        }

        [TestMethod]
        public void BuildMustPassTheDataToTwoMocksWhenRegistered()
        {
            var contextBuilder = ContextBuilderFactory.CreateContextBuilder()
                .WithData(new DataWithTwoMocks {SomeData = "TheData"})
                .Build();

            DataWithTwoMocks[] allDataInMocks = contextBuilder.GetInstance<DataWithTwoMocksReader>().Query().ToArray();
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
            var contextBuilder = ContextBuilderFactory.CreateContextBuilder()
                .WithData(new DataWithOneStateHandler {SomeData = "TheData"})
                .Build();

            Assert.AreEqual("TheData", contextBuilder.GetInstance<DataWithOneStateHandlerReader>().Query().SomeData, "Expected the data to be passed to the registered state handler.");
        }

        [TestMethod]
        public void BuildMustPassTheDataToTwoStateHandlersWhenRegistered()
        {
            var contextBuilder = ContextBuilderFactory.CreateContextBuilder()
                .WithData(new DataWithTwoStateHandlers {SomeData = "TheData"})
                .Build();

            DataWithTwoStateHandlers[] allDataInStateHandlers = contextBuilder.GetInstance<DataWithTwoStateHandlersReader>().Query().ToArray();
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
            var contextBuilder = ContextBuilderFactory.CreateContextBuilder()
                .WithEnumerableData(new List<DataWithOneMockAndOneStateHandler>
                    { new DataWithOneMockAndOneStateHandler {SomeData = "TheData"}})
                .Build();

            DataWithOneMockAndOneStateHandler[] allDataInStateHandlers = contextBuilder.GetInstance<DataWithOneMockAndOneStateHandlerReader>().Query().ToArray();
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
                ContextBuilderFactory.CreateContextBuilder()
                    .WithData(new DataForStateHandlerWhichThrowsInBuild())
                    .Build());
        }

        [TestMethod]
        public void WithClearDataStoreMustClearAllDataWhenCalled()
        {
            var contextBuilder = ContextBuilderFactory.CreateContextBuilder();
            contextBuilder
                .WithData(new DataWithOneMock {SomeData = "TheData"});
            DataWithOneMock firstPre = contextBuilder.First<DataWithOneMock>();
            DataWithOneMock lastPre = contextBuilder.Last<DataWithOneMock>();
            List<DataWithOneMock> allPre = contextBuilder.All<DataWithOneMock>().ToList();

            contextBuilder.WithClearDataStore();

            MultiAssertForTException.Aggregate<AssertFailedException>(
                () => Assert.IsTrue(allPre.Single() == firstPre, "Expected a single piece of data in the data store initially."),
                () => Assert.IsTrue(firstPre == lastPre, "Expected the first to be the last in the data store initially."),
                () => Assert.ThrowsException<KeyNotFoundException>(() => contextBuilder.All<DataWithOneMock>(), 
                    "Expected an empty data store after WithClearDataStore.")
            );
        }
    }
}
