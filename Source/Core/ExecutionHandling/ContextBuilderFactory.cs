using System;
using System.Collections.Generic;
using System.Linq;

namespace LeanTest.Core.ExecutionHandling
{
    /// <summary>
    /// Used to create instances of context builders.
    /// </summary>
    /// <remarks>
    /// Each context builder will reference an IoC container which is created by an IoC container factory method passed in during intialisation.
    /// Also during initialisation, it is specified if the IoC container must be re-used or re-created per request.
    /// Eventually, this class will support the ability to isolate tests so that services can run in separate AppDomains.
    /// </remarks>
    public static class ContextBuilderFactory
    {
        /// <summary>
        /// The container instance for the currently running AppDomain.
        /// </summary>
        /// <remarks>
        /// Depending on the IoC mode, we will either have a single container instance (per AppDomain) which is reused across all test classes 
        /// of the assembly, or a new instance will be created whenever a context builder is created.
        /// </remarks>
        private static Lazy<IIocContainer> lazyIocContainer;
        private static Func<IIocContainer> iocContainerFactory;

        private static CleanContextMode cleanContextMode;
        private static readonly ICollection<Func<IIocContainer, IDataStore, IBuilder>> builderFactories = new List<Func<IIocContainer, IDataStore, IBuilder>>();

        /// <summary>
        /// The lastly created context builder instance for the currently running AppDomain.
        /// Use it to reference context builders in any number of AppDomains across TestInitialize, TestCleanup and test methods.
        /// </summary>
        /// <remarks>
        /// You will create at least one context builder per test class, but never reuse across test classes.
        /// </remarks>
        public static ContextBuilder ContextBuilder { get; private set; }

        /// <summary>
        /// Creates a context builder with the IoC composition root from the current AppDomain.
        /// </summary>
        public static ContextBuilder CreateContextBuilder()
        {
            IIocContainer iocContainer;
            switch (cleanContextMode)
            {
                case CleanContextMode.ReCreate:
                    iocContainer = iocContainerFactory();
                    break;
                case CleanContextMode.ReUse:
                    iocContainer = lazyIocContainer.Value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return ContextBuilder = new ContextBuilder(iocContainer, Enumerable.ToArray<Func<IIocContainer, IDataStore, IBuilder>>(builderFactories));
        }

        /// <summary>
        /// Setup IoC and builders. Eventually, this will initialize app domains.
        /// </summary>
        public static void Initialize(CleanContextMode mode, Func<IIocContainer> iocfactory)
        {
            cleanContextMode = mode;
            iocContainerFactory = iocfactory;

            // TODO: This should not be part of Initialize - to be removed as soon as 'state' is a separate nuGet package!
            AddBuilderFactory((container, dataStore) => new StateBuilder(container, dataStore));

            lazyIocContainer = new Lazy<IIocContainer>(iocContainerFactory);
        }

        /// <summary>
        /// Adds a builder factory for building e.g. 'mock' and 'state' builders.
        /// </summary>
        internal static void AddBuilderFactory(Func<IIocContainer, IDataStore, IBuilder> builderFactory)
        {
            builderFactories.Add(builderFactory);
        }

        /// <summary>
        /// Eventually, this method will unload the separate app domains set up during <c>Initialize</c>.
        /// </summary>
        public static void Cleanup()
        {
        }
    }
}