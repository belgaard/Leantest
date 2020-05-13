using System;
using System.Collections.Generic;

namespace LeanTest.Mock
{
    /// <summary>If you find it convenient, you can use this as a base class for classes which implement <c>IMockForData</c> type <c>T</c>.</summary>
    public class TypedData<T>
    {
        /// <summary>Stores data for type <c>T</c>.</summary>
        protected List<T> Data { get; } = new List<T>();
        /// <summary>Store data of type <c>T</c>.</summary>
        public void WithData(T data) => Data.Add(data);
        /// <summary>Override this in order to execute pre build actions.</summary>
        public virtual void PreBuild() { }
        /// <summary>Override this in order to execute build actions per type.</summary>
        public virtual void Build(Type type) { }
        /// <summary>Override this in order to execute post build actions.</summary>
        public virtual void PostBuild() { }
    }
}