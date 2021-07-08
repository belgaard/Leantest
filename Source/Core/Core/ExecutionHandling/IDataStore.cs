using System;
using System.Collections.Generic;

namespace LeanTest.Core.ExecutionHandling
{
    /// <summary>
    /// Interface for a data store that hold all data in the context.
    /// </summary>
    internal interface IDataStore
    {
        /// <summary>The data of the store, per <c>Type</c> of data.</summary>
        IDictionary<Type, List<object>> TypedData { get; }
        /// <summary>Add data of type <c>T</c> to the store.</summary>
        void WithData<T>(T data);
        /// <summary>Pre-declare the intent to handle data of type <c>T</c> in the store.</summary>
        void WithData<T>();
        /// <summary>Add an enumerable of data of type <c>T</c> to the store.</summary>
        void WithEnumerable<T>(IEnumerable<T> enumerableData);
    }
}