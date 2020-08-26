using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LeanTest.Core.ExecutionHandling;

namespace LeanTest.Mock
{
    internal class GenericBuilder : IBuilder
    {
        private const string WithDataMethod = nameof(TypedData<GenericBuilder>.WithData);
        private const string PreBuildMethod = nameof(TypedData<GenericBuilder>.PreBuild);
        private const string PostBuildMethod = nameof(TypedData<GenericBuilder>.PostBuild);
        private const string BuildMethod = nameof(TypedData<GenericBuilder>.Build);
        private const string TryResolveAllMethod = nameof(IIocContainer.TryResolveAll);
        private readonly IIocContainer _container;
        private readonly IDataStore _dataStore;
        private readonly Type _specificInterface;

        private readonly IDictionary<Type, Func<IEnumerable<object>>> _typedMockEnumsDelegates = 
            new Dictionary<Type, Func<IEnumerable<object>>>();

        public GenericBuilder(IIocContainer container, IDataStore dataStore, Type specificInterface)
        {
            _container = container ?? throw new ArgumentNullException(nameof(container));
            _dataStore = dataStore ?? throw new ArgumentNullException(nameof(dataStore));
            _specificInterface = specificInterface;
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
                    Type theClass = _specificInterface.MakeGenericType(mockDelegatesForType.Key);

                    bool mustPreAndPostBuild = !preBuildMocks.Contains(mock);
                    if (mustPreAndPostBuild)
                    {
                        preBuildMocks.Add(mock);
                        theClass.GetTypeInfo().GetDeclaredMethod(PreBuildMethod)?.Invoke(mock, null);
                    }

                    if (_dataStore.TypedData.ContainsKey(mockDelegatesForType.Key))
                        foreach (object data in _dataStore.TypedData[mockDelegatesForType.Key])
                            theClass.GetTypeInfo().GetDeclaredMethod(WithDataMethod)?.Invoke(mock, new[] { data });

                    theClass.GetTypeInfo().GetDeclaredMethod(BuildMethod)?.Invoke(mock, new object[] { mockDelegatesForType.Key });

                    if (mustPreAndPostBuild)
                        postBuildMethods.Add(() => theClass.GetTypeInfo().GetDeclaredMethod(PostBuildMethod)?.Invoke(mock, null));
                }
            }

            foreach (Action postBuildMethod in postBuildMethods)
                postBuildMethod();

            return typesWithNoMock;
        }

        public void WithBuilderForData<T>() => 
            _typedMockEnumsDelegates[typeof(T)] = () => from mock in TryResolveAll<T>() select mock;

        private IEnumerable<object> TryResolveAll<T>()
        {
            MethodInfo mi = typeof(IIocContainer).GetMethod(TryResolveAllMethod);
            MethodInfo miConstructed = mi?.MakeGenericMethod(_specificInterface.MakeGenericType(typeof(T)));

            return miConstructed?.Invoke(_container, new object[] {}) as IEnumerable<object>;
        }
    }
}