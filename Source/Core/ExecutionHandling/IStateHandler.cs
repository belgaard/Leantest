using System;

namespace LeanTest.Core.ExecutionHandling
{
    /// <summary>
    /// Interface for state handlers that implement state of type <c>T</c>.
    /// </summary>
    public interface IStateHandler<in T>
    {
        /// <summary>
        /// Declare data of type <c>T</c>.
        /// </summary>
        void WithData(T data);
        /// <summary>
        /// Clear data of type <c>Type</c>.
        /// </summary>
        void Clear(Type type);
        /// <summary>
        /// Use the declared data to populate state.
        /// </summary>
        void Build();
    }
}