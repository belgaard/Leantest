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
        private readonly IDictionary<Type, Func<IEnumerable<object>>> _typedMockEnumsDelegates = new Dictionary<Type, Func<IEnumerable<object>>>();

        public MockingBuilder(IIocContainer container, IDataStore dataStore)
        {
            _container = container ?? throw new ArgumentNullException(nameof(container));
            _dataStore = dataStore ?? throw new ArgumentNullException(nameof(dataStore));
        }

        public void Build()
        {
            var preAndPostBuiltMocks = new List<object>();

            foreach (KeyValuePair<Type, Func<IEnumerable<object>>> mockDelegatesForType in _typedMockEnumsDelegates)
            {
                IEnumerable<object> mocks = mockDelegatesForType.Value().ToArray();

                foreach (object mock in mocks)
                {
                    bool mustPreAndPostBuild = !preAndPostBuiltMocks.Contains(mock);
                    if (mustPreAndPostBuild)
                        preAndPostBuiltMocks.Add(mock);

                    Type theClass = typeof(IMockForData<>).MakeGenericType(mockDelegatesForType.Key);
                    if (mustPreAndPostBuild)
                    {
                        MethodInfo preBuildMethod = theClass.GetTypeInfo().GetDeclaredMethod(PreBuildMethod);
                        preBuildMethod.Invoke(mock, null);
                    }

                    MethodInfo withDataMethod = theClass.GetTypeInfo().GetDeclaredMethod(WithDataMethod);
                    foreach (object data in _dataStore.TypedData[mockDelegatesForType.Key])
                        withDataMethod.Invoke(mock, new[] { data });

                    MethodInfo buildMethod = theClass.GetTypeInfo().GetDeclaredMethod(BuildMethod);
                    buildMethod.Invoke(mock, new object[] { mockDelegatesForType.Key });

                    if (!mustPreAndPostBuild)
                        continue;

                    MethodInfo postBuildMethod = theClass.GetTypeInfo().GetDeclaredMethod(PostBuildMethod);
                    postBuildMethod.Invoke(mock, null);
                }
            }
        }

        public void WithBuilderForData<T>() => 
            _typedMockEnumsDelegates[typeof(T)] = () => from mock in _container.TryResolveAll<IMockForData<T>>() select mock as object;
    }
}