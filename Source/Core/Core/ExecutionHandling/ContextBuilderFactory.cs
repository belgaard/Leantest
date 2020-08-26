using System;
using System.Collections.Generic;
using System.Linq;
using LeanTest.Mock;

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
        private static Lazy<IIocContainer> _lazyIocContainer;
        private static Func<IIocContainer> _iocContainerFactory;

        private static CleanContextMode _cleanContextMode;
        private static readonly ICollection<Func<IIocContainer, IDataStore, IBuilder>> BuilderFactories = new List<Func<IIocContainer, IDataStore, IBuilder>>();

        /// <summary>
        /// The lastly created context builder instance for the currently running AppDomain.
        /// Use it to reference context builders in any number of AppDomains across TestInitialize, TestCleanup and test methods.
        /// </summary>
        /// <remarks>
        /// You will create at least one context builder per test class, but never reuse across test classes.
        /// </remarks>
        [Obsolete("Don't reference a global instance")]
        public static ContextBuilder ContextBuilder { get; private set; }

        /// <summary>
        /// Creates a context builder with the IoC composition root from the current AppDomain.
        /// </summary>
        public static ContextBuilder CreateContextBuilder()
        {
            IIocContainer iocContainer;
            switch (_cleanContextMode)
            {
                case CleanContextMode.ReCreate:
                    // TODO: Dispose the old container!
                    iocContainer = _iocContainerFactory();
                    break;
                case CleanContextMode.ReUse:
                    iocContainer = _lazyIocContainer.Value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            return ContextBuilder = new ContextBuilder(iocContainer, BuilderFactories.ToArray());
        }

        public static ContextBuilder CreateContextBuilder(IIocContainer iocContainer)
        {
            return ContextBuilder = new ContextBuilder(iocContainer, BuilderFactories.ToArray());
        }

        /// <summary>Setup IoC and builders.</summary>
        /// <param name="mode">Always use 'ReCreate' to recreate the IoC container before each test. If you use 'ReUse' you will need to do any clean-up needed yourself.</param>
        /// <param name="iocContainerFactory">Creates the IoC container, such as e.g. 'new IocContainer(L0CompositionRootForTest.Initialize(CompositionRoot.Initialize(new ServiceCollection()))'.</param>
        public static void Initialize(CleanContextMode mode, Func<IIocContainer> iocContainerFactory)
        {
            _cleanContextMode = mode;
            _iocContainerFactory = iocContainerFactory;

            AddBuilderFactory((container, dataStore) => new GenericBuilder(container, dataStore, typeof(IStateHandler<>)));
            AddBuilderFactory((container, dataStore) => new GenericBuilder(container, dataStore, typeof(IMockForData<>)));

            _lazyIocContainer = new Lazy<IIocContainer>(_iocContainerFactory);
        }
        public static void Initialize(CleanContextMode mode)
        {
            _cleanContextMode = mode;
            _iocContainerFactory = default;

            AddBuilderFactory((container, dataStore) => new GenericBuilder(container, dataStore, typeof(IStateHandler<>)));
            AddBuilderFactory((container, dataStore) => new GenericBuilder(container, dataStore, typeof(IMockForData<>)));

            _lazyIocContainer = default;
        }

        /// <summary>Setup IoC and builders to create the IoC context before each test.</summary>
        /// <param name="iocContainerFactory">Creates the IoC container, such as e.g. 'new IocContainer(L0CompositionRootForTest.Initialize(CompositionRoot.Initialize(new ServiceCollection()))'.</param>
        public static void Initialize(Func<IIocContainer> iocContainerFactory) => Initialize(CleanContextMode.ReCreate, iocContainerFactory);

        /// <summary>
        /// Adds a builder factory for building e.g. 'mock' and 'state' builders.
        /// </summary>
        internal static void AddBuilderFactory(Func<IIocContainer, IDataStore, IBuilder> builderFactory) => BuilderFactories.Add(builderFactory);

        /// <summary>
        /// Eventually, this method will unload the separate app domains set up during <c>Initialize</c>.
        /// </summary>
        public static void Cleanup() {}
    }
}