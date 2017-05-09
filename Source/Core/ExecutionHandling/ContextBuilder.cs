using System;
using System.Collections.Generic;
using System.Linq;

namespace LeanTest.Core.ExecutionHandling
{
    /// <summary>
    /// Encapsulates the IoC container and builds the data and execution context for a test, including 'state' and 'mocks'.
    /// </summary>
    public class ContextBuilder
    {
        private readonly IIocContainer _container;
        private readonly IBuilder[] _builders;
        internal IDataStore DataStore { get; }

        /// <summary>
        /// Initialize internal fields, including data store and builders (for e.g. 'mocks' and 'state') .
        /// </summary>
        /// <param name="container"></param>
        /// <param name="builderFactories"></param>
        internal ContextBuilder(IIocContainer container, params Func<IIocContainer, IDataStore, IBuilder>[] builderFactories)
        {
            _container = container ?? throw new ArgumentNullException(nameof(container));
            _builders = builderFactories?.Select(builderFactory => builderFactory(_container, DataStore)).ToArray() ?? throw new ArgumentNullException(nameof(builderFactories));

            DataStore = new DataStore();
        }

        /// <summary>
        /// Get an instance of type <c>T</c> from the IoC container.
        /// </summary>
        public T GetInstance<T>() where T : class
        {
            return _container.TryResolve<T>();
        }

        /// <summary>
        /// Declare data of type <c>T</c> to be stored, then used to fill in builders (e.g. 'mocks' and 'state') during <c>Build</c>.
        /// </summary>
        public ContextBuilder WithData<T>(T data)
        {
            DataStore.WithData(data);
            foreach (IBuilder builder in _builders)
                builder.WithBuilderForData<T>();

            return this;
        }

        /// <summary>
        /// Declare an enumeration of data of type <c>T</c> to be stored, then used to fill builders (e.g. 'mocks' and 'state') during <c>Build</c>.
        /// </summary>
        public ContextBuilder WithEnumerableData<T>(IEnumerable<T> ts)
        {
            DataStore.WithEnumerable(ts);
            foreach (IBuilder builder in _builders)
                builder.WithBuilderForData<T>();

            return this;
        }

        /// <summary>
        /// Clear all declared data from the data store.
        /// </summary>
        /// <returns></returns>
        public ContextBuilder WithClearDataStore()
        {
            DataStore.TypedData.Clear();

            return this;
        }

        /// <summary>
        /// Use the declared data to build builders (e.g. 'mocks' and 'state').
        /// </summary>
        /// <returns></returns>
        public ContextBuilder Build()
        {
            foreach (IBuilder builder in _builders)
                builder.Build();

            return this;
        }
    }
}
