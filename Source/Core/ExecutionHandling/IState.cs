using System;

namespace LeanTest.Core.ExecutionHandling
{
    /// <summary>
    /// Base interface for all state handlers.
    /// </summary>
    public interface IState
    {
        /// <summary>
        /// Declare data of type <c>Type</c>.
        /// </summary>
        void WithData(Type type, object data);
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