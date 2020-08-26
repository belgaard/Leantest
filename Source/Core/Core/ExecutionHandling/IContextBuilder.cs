using System.Collections.Generic;

namespace LeanTest.Core.ExecutionHandling
{
	/// <summary>Encapsulates and represents the IoC container and builds the data and execution context for a test, including 'state' and 'mocks'.</summary>
	public interface IContextBuilder
	{
		/// <summary>Get an instance of type <c>T</c> from the IoC container.</summary>
		T GetInstance<T>() where T : class;
		/// <summary>Declare data of type <c>T</c> to be stored, then used to fill in builders (e.g. 'mocks' and 'state') during <c>Build</c>.</summary>
		IContextBuilder WithData<T>(T data);

		/// <summary>Pre-declare the intent to handle data of type <c>T</c>. The effect will be to have <c>PreBuild</c>, <c>Build</c> and <c>PostBuild</c> run for builders that
		/// support data of type <c>T</c>, even for tests which do not declare data of type <c>T</c>.</summary>
		IContextBuilder WithData<T>();

		/// <summary>Declare an enumeration of data of type <c>T</c> to be stored, then used to fill builders (e.g. 'mocks' and 'state') during <c>Build</c>.</summary>
		IContextBuilder WithEnumerableData<T>(IEnumerable<T> ts);

		/// <summary>Clear all declared data from the data store.</summary>
		IContextBuilder WithClearDataStore();

		/// <summary>Use the declared data to build builders (e.g. 'mocks' and 'state').</summary>
		IContextBuilder Build();
	}
}