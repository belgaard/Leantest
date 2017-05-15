using System;
using System.Collections.Generic;
using System.Linq;

namespace LeanTest.Core.ExecutionHandling
{
    /// <summary>
    /// Allows support for multiple asserts.
    /// </summary>
    public static class MultiAssert
    {
        /// <summary>
        /// Executes all actions regardless of failed assertions. If any assertions fail, a single assertion failure with
        /// aggregated assertion message texts is generated.
        /// </summary>
        /// <param name="actions">
        /// A list of actions that represent a number of Assert method invocations (Assert.IsTrue() etc.)
        /// </param>
        /// <remarks>
        /// It is still Best Practice to have a single assert per unit test in most cases (but more often than not, developer tests will have several asserts per test).
        /// Refer to the following blog posts for proper usage of MultiAssert,
        ///  - https://elgaard.blog/2011/02/06/multiple-asserts-in-a-single-unit-test-method/
        ///  - https://elgaard.blog/2013/05/26/even-more-asserts-in-a-single-unit-test-method/
        /// </remarks>
        public static void Aggregate<TException>(params AssertAction<TException>[] actions) where TException: Exception, new()
        {
            var exceptions = new List<TException>();

            foreach (var action in actions)
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

            var assertionTexts = exceptions.Select(assertFailedException => assertFailedException.Message);
            IEnumerable<string> enumerable = assertionTexts as string[] ?? assertionTexts.ToArray();
            if (enumerable.Count() != 0)
            {
                throw new
                    TException();
                //{ 
                //        enumerable.Aggregate(
                //            (aggregatedMessage, next) => aggregatedMessage + Environment.NewLine + next))};
            }
        }
    }
}