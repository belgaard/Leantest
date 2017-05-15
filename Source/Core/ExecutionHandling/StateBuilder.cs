using System;
using System.Collections.Generic;
using System.Linq;

namespace LeanTest.Core.ExecutionHandling
{
    public delegate void AssertAction<T>();

    internal class StateBuilder : IBuilder
    {
        private readonly IIocContainer _container;
        private readonly IDataStore _dataStore;
        private readonly IDictionary<Type, Func<IEnumerable<IState>>> _typedStateEnumsDelegates = new Dictionary<Type, Func<IEnumerable<IState>>>();

        private readonly Action<IState, Type> _clearer;

        public StateBuilder(IIocContainer container, IDataStore dataStore)
        {
            _container = container ?? throw new ArgumentNullException(nameof(container));
            _dataStore = dataStore ?? throw new ArgumentNullException(nameof(dataStore));

            _clearer = (state, type) => state.Clear(type);
        }

        public void Build()
        {
            foreach (KeyValuePair<Type, Func<IEnumerable<IState>>> stateKeyValuePair in _typedStateEnumsDelegates)
            {
                IEnumerable<IState> states = stateKeyValuePair.Value().ToArray();
                foreach (IState state in states)
                {
                    _clearer(state, stateKeyValuePair.Key);

                    if (_dataStore.TypedData.All(t => t.Key != stateKeyValuePair.Key))
                        continue;
                    foreach (object data in _dataStore.TypedData[stateKeyValuePair.Key])
                        state.WithData(stateKeyValuePair.Key, data);

                    state.Build();
                }
            }
        }

        public void WithBuilderForData<T>()
        {
            _typedStateEnumsDelegates[typeof(T)] = () => from stateHandler in _container.TryResolveAll<IStateHandler<T>>() select stateHandler as IState;
        }
    }
}