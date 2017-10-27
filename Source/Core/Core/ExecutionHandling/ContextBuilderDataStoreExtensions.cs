using System.Collections.Generic;
using System.Linq;

namespace LeanTest.Core.ExecutionHandling
{
    /// <summary>
    /// Typed access to the data store. Simple First/Last/All operations.
    /// </summary>
    public static class ContextBuilderDataStoreExtensions
    {

        /// <summary>
        /// Get the first declared piece of data of type <c>T</c>.
        /// </summary>
        public static T First<T>(this ContextBuilder contextBuilder) where T : class
        {
            return contextBuilder.DataStore.TypedData[typeof(T)].First() as T;
        }

        /// <summary>
        /// Get the last declared piece of data of type <c>T</c>.
        /// </summary>
        public static T Last<T>(this ContextBuilder contextBuilder) where T : class
        {
            return contextBuilder.DataStore.TypedData[typeof(T)].Last() as T;
        }

        /// <summary>
        /// Get all declared data of type <c>T</c>.
        /// </summary>
        public static IEnumerable<T> All<T>(this ContextBuilder contextBuilder) where T : class
        {
            return contextBuilder.DataStore.TypedData[typeof(T)].Select(o => o as T);
        }
    }
}