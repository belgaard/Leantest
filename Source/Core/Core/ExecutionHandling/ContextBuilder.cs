using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LeanTest.Core.ExecutionHandling
{
	/// <inheritdoc />
	public class ContextBuilder : IContextBuilder
	{
		private readonly IIocContainer _container;
		private readonly IBuilder[] _builders;
		internal IDataStore DataStore { get; }

		/// <summary>Initialize internal fields, including data store and builders (for e.g. 'mocks' and 'state').</summary>
		internal ContextBuilder(IIocContainer container, params Func<IIocContainer, IDataStore, IBuilder>[] builderFactories)
		{
			_container = container ?? throw new ArgumentNullException(nameof(container));

			DataStore = new DataStore();
			_builders = builderFactories?.Select(builderFactory => builderFactory(_container, DataStore)).ToArray() ?? throw new ArgumentNullException(nameof(builderFactories));
		}

		/// <inheritdoc />
		public T GetInstance<T>() where T : class => _container.Resolve<T>();

		/// <inheritdoc />
		public IContextBuilder WithData<T>(T data)
	    {
		    DataStore.WithData(data);
		    foreach (IBuilder builder in _builders)
			    builder.WithBuilderForData<T>();

		    return this;
	    }

		/// <inheritdoc />
		public IContextBuilder WithData<T>()
		{
			DataStore.WithData<T>();
			foreach (IBuilder builder in _builders)
				builder.WithBuilderForData<T>();

			return this;
		}

		/// <inheritdoc />
		public IContextBuilder WithEnumerableData<T>(IEnumerable<T> ts)
		{
			DataStore.WithEnumerable(ts);
			foreach (IBuilder builder in _builders)
				builder.WithBuilderForData<T>();

			return this;
		}

		/// <inheritdoc />
		public IContextBuilder WithClearDataStore()
		{
			DataStore.TypedData.Clear();

			return this;
		}

		/// <inheritdoc />
		public IContextBuilder Build()
		{
			try
			{
				var typesWithNoHandler = new HashSet<Type>(DataStore.TypedData.Keys);
				foreach (IBuilder builder in _builders)
					typesWithNoHandler.IntersectWith(builder.Build());

				if (typesWithNoHandler.Any())
					throw new ArgumentException(
						$"Cannot handle declared data of type '{string.Join(", ", typesWithNoHandler)}'. No state-handler or mock-for-data was found.");
			}
			catch (TargetInvocationException e)
			{
				if (e.InnerException != null)
					throw e.InnerException;
				throw;
			}

			return this;
		}
	}
}
