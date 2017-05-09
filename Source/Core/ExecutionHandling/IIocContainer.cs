using System;
using System.Collections.Generic;

namespace LeanTest.Core.ExecutionHandling
{
    /// <summary>
    /// Base interface for IoC containers.
    /// </summary>
    /// <remarks>
    /// A specific implementation for the actual IoC container used in a given project must be provided.
    /// </remarks>
    public interface IIocContainer
    {
        /// <summary>
        /// Resolve an instance of type <c>T</c>.
        /// </summary>
        /// <exception cref="Exception">Will be thrown if the container cannot resolve.</exception>
        /// <returns>The resolved instance.</returns>
        T Resolve<T>() where T : class;
        /// <summary>
        /// Resolve an instance of type <c>T</c>.
        /// </summary>
        /// <returns>The resolved instance, or null if the container cannot resolve.</returns>
        T TryResolve<T>() where T : class;
        /// <summary>
        /// Resolve all instances of type <c>T</c>.
        /// </summary>
        /// <returns>The resolved instances, or empty if the container cannot resolve.</returns>
        /// <remarks>Note that different IoC containers have different requirements fo rregistration in order to make such 
        /// an operation work as expected.
        /// It is common to implement this with a call to <c>TryResolve&lt;T&gt;</c>, thus allowing only a single mock or state handler per <c>T</c>.</remarks>
        IEnumerable<T> TryResolveAll<T>() where T : class;
    }
}