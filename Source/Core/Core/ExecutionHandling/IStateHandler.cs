using System;

namespace LeanTest.Core.ExecutionHandling
{
    /// <summary>Interface for state handlers that implement state of type <c>T</c>.</summary>
    public interface IStateHandler<in T>
    {
        /// <summary>Declare data of type <c>T</c>.</summary>
        void WithData(T data);
        /// <summary>Called before build only once for the instance, allows you to prepare to populate state.</summary>
#if NET472
        void PreBuild();
#else
        void PreBuild() {}
#endif
        /// <summary>Use the declared data to populate state, called after all data of type <c>type</c> has been put to the instance with <c>WithData</c>.</summary>
#if NET472
        void Build(Type type);
#else
        void Build(Type type) {}
#endif
        /// <summary>Called after build, only once for the instance.</summary>
#if NET472
        void PostBuild();
#else
        void PostBuild() {}
#endif
    }
}