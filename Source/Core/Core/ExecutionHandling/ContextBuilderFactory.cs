using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LeanTest.Mock;

namespace LeanTest.Core.ExecutionHandling
{
    /// <summary>Used to create instances of context builders.</summary>
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
        internal static readonly ICollection<Func<IIocContainer, IDataStore, IBuilder>> BuilderFactories = new List<Func<IIocContainer, IDataStore, IBuilder>>();
        private static Func<ICreateContextBuilder> _createContextBuilderFactory;

		private static readonly LockedDisposeList DisposablesForCleanup = new LockedDisposeList();

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
            if (_createContextBuilderFactory != null)
                return _createContextBuilderFactory().CreateContextBuilder;

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

        /// <summary>Setup IoC and builders.</summary>
        /// <param name="mode">Always use 'ReCreate' to recreate the IoC container before each test. If you use 'ReUse' you will need to do any clean-up needed yourself.</param>
        /// <param name="iocContainerFactory">Creates the IoC container, such as e.g. 'new IocContainer(L0CompositionRootForTest.Initialize(CompositionRoot.Initialize(new ServiceCollection()))'.</param>
        public static void Initialize(CleanContextMode mode, Func<IIocContainer> iocContainerFactory)
        {
            _cleanContextMode = mode;
            _iocContainerFactory = iocContainerFactory;

            InitializeBuilders();

            _lazyIocContainer = new Lazy<IIocContainer>(_iocContainerFactory);
        }

        /// <summary>Setup via a context builder factory.</summary>
        /// <param name="createContextBuilder">The context builder factory.</param>
        /// <remarks>Note that it is the responsibility of the caller to dispose of any objects that may be created along with the context builder.</remarks>
        public static void Initialize(Func<ICreateContextBuilder> createContextBuilder)
        {
            _createContextBuilderFactory = createContextBuilder;

            InitializeBuilders();
        }

        /// <summary>Setup builders only.</summary>
        public static void InitializeBuilders()
        {
            AddBuilderFactory((container, dataStore) => new GenericBuilder(container, dataStore, typeof(IStateHandler<>)));
            AddBuilderFactory((container, dataStore) => new GenericBuilder(container, dataStore, typeof(IMockForData<>)));
        }

        /// <summary>Setup IoC and builders to create the IoC context before each test.</summary>
        /// <param name="iocContainerFactory">Creates the IoC container, such as e.g. 'new IocContainer(L0CompositionRootForTest.Initialize(CompositionRoot.Initialize(new ServiceCollection()))'.</param>
        public static void Initialize(Func<IIocContainer> iocContainerFactory) => Initialize(CleanContextMode.ReCreate, iocContainerFactory);

        /// <summary>
        /// Adds a builder factory for building e.g. 'mock' and 'state' builders.
        /// </summary>
        internal static void AddBuilderFactory(Func<IIocContainer, IDataStore, IBuilder> builderFactory) => BuilderFactories.Add(builderFactory);

        /// <summary>Cleanup by disposing registered disposables.</summary>
        public static void Cleanup() => DisposablesForCleanup.Clear();

        /// <summary>Register to be disposed at cleanup.</summary>
        /// <remarks>Don't call from Dispose methods, that would cause a deadlock.</remarks>
        public static void AddForCleanup(IDisposable disposable) => DisposablesForCleanup.Add(disposable);
        /// <summary>Remove from the list of registered to be disposed at cleanup.</summary>
        /// <remarks>Don't call from Dispose methods, that would cause a deadlock.</remarks>
        public static void RemoveFromCleanup(IDisposable disposable) => DisposablesForCleanup.Remove(disposable);

        private class LockedDisposeList : ICollection<IDisposable>
        {
			private List<IDisposable> _disposables = new List<IDisposable>();
			private readonly object _theLock = new object(); // Lock for any access, not only the current list instance.

			public void Clear()
	        {
		        lock (_theLock)
		        {
			        foreach (var disposable in _disposables)
				        try { disposable?.Dispose(); }
				        catch { /* ignored */ }

			        _disposables = new List<IDisposable>();
		        }
	        }
			public void Add(IDisposable item)
			{
				lock (_theLock) _disposables.Add(item);
			}
			public bool Remove(IDisposable item)
			{
				lock (_theLock) return _disposables.Remove(item);
			}

	        IEnumerator<IDisposable> IEnumerable<IDisposable>.GetEnumerator() { throw new NotImplementedException(); }
	        IEnumerator IEnumerable.GetEnumerator() { throw new NotImplementedException(); }
	        bool ICollection<IDisposable>.Contains(IDisposable item) { throw new NotImplementedException(); }
	        void ICollection<IDisposable>.CopyTo(IDisposable[] array, int arrayIndex) { throw new NotImplementedException(); }
			int ICollection<IDisposable>.Count => throw new NotImplementedException();
			bool ICollection<IDisposable>.IsReadOnly => throw new NotImplementedException();
        }
    }
}