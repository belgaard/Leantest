using System;

namespace LeanTest.Mock
{
    /// <summary>
    /// Interface for mocks for data of type <c>T</c>.
    /// </summary>
    public interface IMockForData<in T>
    {
        /// <summary>
        /// Declare data of type <c>T</c>.
        /// </summary>
        void WithData(T data);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        void Clear(Type type);

        /// <summary>
        /// 
        /// </summary>
        void Build(); // TODO: Add Type parameter?
    }
}
