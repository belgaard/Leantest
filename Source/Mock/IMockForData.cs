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
        /// Called before build, allows you to prepare to populate state.
        /// </summary>
        void PreBuild();
        /// <summary>
        /// Use the declared data to populate state.
        /// </summary>
        void Build(Type type);
        /// <summary>
        /// Called after build.
        /// </summary>
        void PostBuild();
    }
}
