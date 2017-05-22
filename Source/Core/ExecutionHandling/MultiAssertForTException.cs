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
        /// <summary>
        /// Executes all actions regardless of failed assertions (a.k.a. <c>TException</c> thrown). If any assertions fail, a single 
        /// assertion failure with aggregated assertion message texts is generated (i.e. a <c>AggregatedMessagesException</c> is thrown).
        /// </summary>
        /// <param name="actions">
        /// A list of actions that represent a number of Assert method invocations (Assert.IsTrue etc.)
        /// </param>
        /// <remarks>
        /// It is still Best Practice to have a single assert per unit test in most cases (but more often than not, developer tests will have several asserts per test).
        /// Refer to the following blog posts for proper usage of MultiAssert,
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
    /// <summary>
    /// Exception helper methods for tests.
    /// </summary>
    public static class ExceptionAssertTException
    {
        /// <exception cref="AggregatedMessagesException">Thrown when the action did not throw the required exception.</exception>
        public static TException Throws<TException>(Action action, string message = "")
            where TException : Exception
        {
            Exception exception = null;

            try
            {
                action.Invoke();
            }
            catch (TException ex)
            {
                return ex;
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            if (exception == null)
                throw new AggregatedMessagesException("ExceptionAssert.Throws failed. No exception was thrown. Expected: " + typeof(TException) + ". " + message);

            throw new AggregatedMessagesException("ExceptionAssert.Throws failed. Expected exception type: " + typeof(TException).Name + ". Thrown: " + exception.GetType() + ". " + message);
        }

        /// <exception cref="AggregatedMessagesException">Thrown when the action threw and exception. </exception>
        public static void DoesNotThrow(Action action, string message = "")
        {
            try
            {
                action.Invoke();
            }
            catch (Exception ex)
            {
                throw new AggregatedMessagesException("ExceptionAssert.DoesNotThrow failed. " /*+ action.Method + " threw unexpected exception: "*/ + ex.GetType() + ". " + message, ex);
            }
        }
    }
}