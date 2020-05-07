using System;

namespace LeanTest.Mock
{
    /// <summary>Interface for mocks for data of type <c>T</c>.</summary>
    public interface IMockForData<in T>
    {
        /// <summary>Declare data of type <c>T</c>.</summary>
        void WithData(T data);
        /// <summary>Called before build only once for the instance, allows you to prepare to populate state.</summary>
        void PreBuild();
        /// <summary>Use the declared data to populate state, called after all data of type <c>type</c> has been put to the instance with <c>WithData</c>.</summary>
        void Build(Type type);
        /// <summary>Called after build, only once for the instance.</summary>
        void PostBuild();
    }
}
