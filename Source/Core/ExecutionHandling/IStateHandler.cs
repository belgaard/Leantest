namespace LeanTest.Core.ExecutionHandling
{
    /// <summary>
    /// Interface for state handlers that implement state of type <c>T</c>.
    /// </summary>
    public interface IStateHandler<in T> : IState
    {
        /// <summary>
        /// Declare data of type <c>T</c>.
        /// </summary>
        void WithData(T data);
    }
}