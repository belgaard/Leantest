using System;
using System.Threading.Tasks;
using LeanTest.Core.ExecutionHandling;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Core.Examples.MsTest.TestSetup
{
    /// <summary>
    /// The MS Test adaption to <c>MultiAssertForTException</c>.
    /// </summary>
    public static class MultiAssert
    {
        public static void Aggregate(params Action[] actions)
        {
            try
            {
                MultiAssertForTException.Aggregate<AssertFailedException>(actions);
            }
            // Turn aggregated messages into a proper MS Test assert failed exception - but let other exceptions, including 'inconclusive' fall through:
            catch (AggregatedMessagesException e)
            {
                throw new AssertFailedException(e.Message);
            }
        }
    }
    /// <summary>
    /// The MS Test adaption to <c>ExceptionAssertTException</c>.
    /// </summary>
    public static class ExceptionAssert
    {
        public static TException Throws<TException>(Func<Task> func, string message = "") where TException : Exception =>
            ExceptionAssertTException.Adapter(func, message, ExceptionAssertTException.Throws<TException>, MsTestAssertFailedException);
        public static TException Throws<TException>(Action action, string message = "") where TException : Exception =>
            ExceptionAssertTException.Adapter(action, message, ExceptionAssertTException.Throws<TException>, MsTestAssertFailedException);

        private static Exception MsTestAssertFailedException(string message) => 
            new AssertFailedException(message);
    }
}