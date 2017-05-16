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
}