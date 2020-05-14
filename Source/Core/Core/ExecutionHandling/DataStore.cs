using System;
using System.Collections.Generic;

namespace LeanTest.Core.ExecutionHandling
{
    internal class DataStore : IDataStore
    {
        public IDictionary<Type, List<object>> TypedData { get; } = new Dictionary<Type, List<object>>();

        public void WithData<T>()
        {
            if (!TypedData.ContainsKey(typeof(T)))
                TypedData[typeof(T)] = new List<object>();
        }

        public void WithData<T>(T data)
        {
            if (!TypedData.ContainsKey(typeof(T)))
                TypedData[typeof(T)] = new List<object>();
            TypedData[typeof(T)].Add(data);
        }

        public void WithEnumerable<T>(IEnumerable<T> enumerableData)
        {
            if (!TypedData.ContainsKey(typeof(T)))
                TypedData[typeof(T)] = new List<object>();
            foreach (T t in enumerableData)
                TypedData[typeof(T)].Add(t);
        }
    }
}