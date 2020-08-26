using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LeanTest.Core.ExecutionHandling
{
	public interface IContextBuilder : IDisposable // TODO: Introduce in LeanTest? Implement all extension methods on interface!
	{
		T GetInstance<T>() where T : class;
		ContextBuilder WithData<T>(T data);
	}

	/// <summary>
	/// Encapsulates the IoC container and builds the data and execution context for a test, including 'state' and 'mocks'.
	/// </summary>
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

		/// <summary>Get an instance of type <c>T</c> from the IoC container.</summary>
		public T GetInstance<T>() where T : class => _container.Resolve<T>();

	    /// <summary>Declare data of type <c>T</c> to be stored, then used to fill in builders (e.g. 'mocks' and 'state') during <c>Build</c>.</summary>
	    public ContextBuilder WithData<T>(T data)
	    {
		    DataStore.WithData(data);
		    foreach (IBuilder builder in _builders)
			    builder.WithBuilderForData<T>();

		    return this;
	    }

		/// <summary>Pre-declare the intent to handle data of type <c>T</c>. The effect will be to have <c>PreBuild</c>, <c>Build</c> and <c>PostBuild</c> run for builders that
		/// support data of type <c>T</c>, even for tests which do not declare data of type <c>T</c>.</summary>
		public ContextBuilder WithData<T>()
		{
			DataStore.WithData<T>();
			foreach (IBuilder builder in _builders)
				builder.WithBuilderForData<T>();

			return this;
		}

		/// <summary>Declare an enumeration of data of type <c>T</c> to be stored, then used to fill builders (e.g. 'mocks' and 'state') during <c>Build</c>.</summary>
		public ContextBuilder WithEnumerableData<T>(IEnumerable<T> ts)
		{
			DataStore.WithEnumerable(ts);
			foreach (IBuilder builder in _builders)
				builder.WithBuilderForData<T>();

			return this;
		}

		/// <summary>Clear all declared data from the data store.</summary>
		public ContextBuilder WithClearDataStore()
		{
			DataStore.TypedData.Clear();

			return this;
		}

		/// <summary>Use the declared data to build builders (e.g. 'mocks' and 'state').</summary>
		public ContextBuilder Build()
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

		protected virtual void Dispose(bool disposing)
		{
			if (disposing)
			{
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
	}
}
