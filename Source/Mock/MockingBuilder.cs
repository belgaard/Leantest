using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using LeanTest.Core.ExecutionHandling;

namespace LeanTest.Mock
{
    internal class MockingBuilder : IBuilder
    {
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
            foreach (KeyValuePair<Type, Func<IEnumerable<object>>> mockDelegatesForType in _typedMockEnumsDelegates)
            {
                IEnumerable<object> mocks = mockDelegatesForType.Value().ToArray();
                foreach (object mock in mocks)
                {
                    Type theClass = typeof(IMockForData<>).MakeGenericType(mockDelegatesForType.Key);
                    
                    MethodInfo clearMethod = theClass.GetTypeInfo().GetDeclaredMethod(nameof(IMockForData<MockingBuilder>.Clear));
                    clearMethod.Invoke(mock, new object[] { mockDelegatesForType.Key });

                    if (_dataStore.TypedData.All(t => t.Key != mockDelegatesForType.Key))
                        continue;
                    MethodInfo withDataMethod = theClass.GetTypeInfo().GetDeclaredMethod(nameof(IMockForData<MockingBuilder>.WithData));
                    foreach (object data in _dataStore.TypedData[mockDelegatesForType.Key])
                        withDataMethod.Invoke(mock, new[] { data });

                    MethodInfo buildMethod = theClass.GetTypeInfo().GetDeclaredMethod(nameof(IMockForData<MockingBuilder>.Build));
                    buildMethod.Invoke(mock, null/*, new object[] { mockDelegatesForType.Key }*/); // TODO: Add a Type parameter to Build?
                }
            }
        }

        public void WithBuilderForData<T>() => 
            _typedMockEnumsDelegates[typeof(T)] = () => from mock in _container.TryResolveAll<IMockForData<T>>() select mock as object;
    }
}