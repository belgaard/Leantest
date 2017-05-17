using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LeanTest.Core.ExecutionHandling
{
    internal class StateBuilder : IBuilder
    {
        private const string WithDataMethod = nameof(IStateHandler<StateBuilder>.WithData);
        private const string ClearMethod = nameof(IStateHandler<StateBuilder>.Clear);
        private const string BuildMethod = nameof(IStateHandler<StateBuilder>.Build);
        private readonly IIocContainer _container;
        private readonly IDataStore _dataStore;
        private readonly IDictionary<Type, Func<IEnumerable<object>>> _typedStateEnumsDelegates = new Dictionary<Type, Func<IEnumerable<object>>>();

        public StateBuilder(IIocContainer container, IDataStore dataStore)
        {
            _container = container ?? throw new ArgumentNullException(nameof(container));
            _dataStore = dataStore ?? throw new ArgumentNullException(nameof(dataStore));
        }

        public void Build()
        {
            foreach (KeyValuePair<Type, Func<IEnumerable<object>>> stateKeyValuePair in _typedStateEnumsDelegates)
            {
                IEnumerable<object> states = stateKeyValuePair.Value().ToArray();
                foreach (object state in states)
                {
                    Type theClass = typeof(IStateHandler<>).MakeGenericType(stateKeyValuePair.Key);

                    MethodInfo clearMethod = theClass.GetTypeInfo().GetDeclaredMethod(ClearMethod);
                    clearMethod.Invoke(state, new object[] { stateKeyValuePair.Key });

                    if (_dataStore.TypedData.All(t => t.Key != stateKeyValuePair.Key))
                        continue;
                    MethodInfo withDataMethod = theClass.GetTypeInfo().GetDeclaredMethod(WithDataMethod);
                    foreach (object data in _dataStore.TypedData[stateKeyValuePair.Key])
                        withDataMethod.Invoke(state, new[] { data });

                    MethodInfo buildMethod = theClass.GetTypeInfo().GetDeclaredMethod(BuildMethod);
                    buildMethod.Invoke(state, null/*, new object[] { mockDelegatesForType.Key }*/); // TODO: Add a Type parameter to Build?
                }
            }
        }

        public void WithBuilderForData<T>() =>
            _typedStateEnumsDelegates[typeof(T)] = () => from stateHandler in _container.TryResolveAll<IStateHandler<T>>() select stateHandler as object;
    }
}