using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LeanTest.Core.ExecutionHandling;

namespace LeanTest.Mock
{
    internal class MockingBuilder : IBuilder
    {
        private const string WithDataMethod = nameof(IMockForData<MockingBuilder>.WithData);
        private const string PreBuildMethod = nameof(IMockForData<MockingBuilder>.PreBuild);
        private const string PostBuildMethod = nameof(IMockForData<MockingBuilder>.PostBuild);
        private const string BuildMethod = nameof(IStateHandler<StateBuilder>.Build);
        private readonly IIocContainer _container;
        private readonly IDataStore _dataStore;
        private readonly IDictionary<Type, Func<IEnumerable<object>>> _typedMockEnumsDelegates = 
			new Dictionary<Type, Func<IEnumerable<object>>>();

        public MockingBuilder(IIocContainer container, IDataStore dataStore)
        {
            _container = container ?? throw new ArgumentNullException(nameof(container));
            _dataStore = dataStore ?? throw new ArgumentNullException(nameof(dataStore));
        }

        public HashSet<Type> Build()
        {
            var preBuildMocks = new List<object>();
            var postBuildMethods = new List<Action>();
            var typesWithNoMock = new HashSet<Type>();

            foreach (KeyValuePair<Type, Func<IEnumerable<object>>> mockDelegatesForType in _typedMockEnumsDelegates)
            {
                IEnumerable<object> mocks = mockDelegatesForType.Value().ToArray();

                if (!mocks.Any())
	                typesWithNoMock.Add(mockDelegatesForType.Key);

                foreach (object mock in mocks)
                {
                    Type theClass = typeof(IMockForData<>).MakeGenericType(mockDelegatesForType.Key);

                    bool mustPreAndPostBuild = !preBuildMocks.Contains(mock);
                    if (mustPreAndPostBuild)
                    {
                        preBuildMocks.Add(mock);
                        theClass.GetTypeInfo().GetDeclaredMethod(PreBuildMethod).Invoke(mock, null);
                    }

                    if (_dataStore.TypedData.ContainsKey(mockDelegatesForType.Key))
                        foreach (object data in _dataStore.TypedData[mockDelegatesForType.Key])
                            theClass.GetTypeInfo().GetDeclaredMethod(WithDataMethod).Invoke(mock, new[] { data });

                    theClass.GetTypeInfo().GetDeclaredMethod(BuildMethod).Invoke(mock, new object[] { mockDelegatesForType.Key });

                    if (mustPreAndPostBuild)
                        postBuildMethods.Add(() => theClass.GetTypeInfo().GetDeclaredMethod(PostBuildMethod).Invoke(mock, null));
                }
            }

            foreach (Action postBuildMethod in postBuildMethods)
                postBuildMethod();

            return typesWithNoMock;
        }

        public Func<IEnumerable<object>> WithBuilderForData<T>() => 
            _typedMockEnumsDelegates[typeof(T)] = () => from mock in _container.TryResolveAll<IMockForData<T>>() select mock as object;
    }
}