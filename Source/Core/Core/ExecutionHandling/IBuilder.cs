using System;
using System.Collections.Generic;

namespace LeanTest.Core.ExecutionHandling
{
    /// <summary>
    /// Interface for all builders (for building stuff like e.g. 'mock' and 'state').
    /// </summary>
    internal interface IBuilder
    {
        /// <summary>
        /// Register a builder for data of type <c>TData</c>
        /// </summary>
        /// <typeparam name="TData"></typeparam>
        Func<IEnumerable<object>> WithBuilderForData<TData>();
        /// <summary>
        /// Do the build.
        /// </summary>
        void Build();
    }
}