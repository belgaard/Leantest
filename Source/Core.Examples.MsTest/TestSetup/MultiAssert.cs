using System;
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
        public static TException Throws<TException>(Action action, string message = "")
            where TException : Exception
        {
            try
            {
                return ExceptionAssertTException.Throws<TException>(action, message);
            }
            // Turn aggregated messages into a proper MS Test assert failed exception fall through:
            catch (AggregatedMessagesException e)
            {
                throw new AssertFailedException(e.Message);
            }
        }
    }
}