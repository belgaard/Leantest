using System;

namespace LeanTest.Core.ExecutionHandling
{
    /// <summary>
    /// Aggregates messages from <c>MultiAssertForTException</c>.
    /// </summary>
    public class AggregatedMessagesException : Exception
    {
        /// <summary></summary>
        public AggregatedMessagesException(string message) :base(message) {}
        /// <summary></summary>
        public AggregatedMessagesException(string message, Exception innerException) : base(message, innerException) { }
    }
}