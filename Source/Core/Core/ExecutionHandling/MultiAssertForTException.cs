using System;
using System.Collections.Generic;
using System.Linq;

namespace LeanTest.Core.ExecutionHandling
{
    /// <summary>
    /// Allows support for multiple asserts.
    /// </summary>
    public static class MultiAssertForTException
    {
        /// <summary>Executes all actions regardless of failed assertions (a.k.a. <c>TException</c> thrown). If any assertions fail, a single 
        /// assertion failure with aggregated assertion message texts is generated (i.e. a <c>AggregatedMessagesException</c> is thrown).</summary>
        /// <param name="actions">A list of actions that represent a number of Assert method invocations (Assert.IsTrue etc.)</param>
        /// <remarks>Refer to the following blog posts for proper usage of MultiAssert,
        ///  - https://elgaard.blog/2011/02/06/multiple-asserts-in-a-single-unit-test-method/
        ///  - https://elgaard.blog/2013/05/26/even-more-asserts-in-a-single-unit-test-method/
        /// </remarks>
        /// <typeparam name="TException">The exception to catch and aggregate.</typeparam>
        public static void Aggregate<TException>(params Action[] actions) where TException: Exception
        {
            var exceptions = new List<TException>();

            foreach (Action action in actions)
            {
                try
                {
                    action();
                }
                catch (TException ex)
                {
                    exceptions.Add(ex);
                }
            }

            IEnumerable<string> assertionTexts = exceptions.Select(assertFailedException => assertFailedException.Message);
            IEnumerable<string> enumerable = assertionTexts as string[] ?? assertionTexts.ToArray();
            if (enumerable.Count() != 0)
                throw new
                    AggregatedMessagesException(
                        enumerable.Aggregate(
                            (aggregatedMessage, next) => aggregatedMessage + Environment.NewLine + next));
        }
    }
}