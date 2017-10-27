using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LeanTest.Core.ExecutionHandling
{
    internal class StateBuilder : IBuilder
    {
        private const string WithDataMethod = nameof(IStateHandler<StateBuilder>.WithData);
        private const string PreBuildMethod = nameof(IStateHandler<StateBuilder>.PreBuild);
        private const string PostBuildMethod = nameof(IStateHandler<StateBuilder>.PostBuild);
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
            var preBuildHandlers = new List<object>();
            var postBuildMethods = new List<Action>();

            foreach (KeyValuePair<Type, Func<IEnumerable<object>>> stateKeyValuePair in _typedStateEnumsDelegates)
            {
                IEnumerable<object> handlers = stateKeyValuePair.Value().ToArray();
                
                foreach (object handler in handlers)
                {
                    Type theClass = typeof(IStateHandler<>).MakeGenericType(stateKeyValuePair.Key);

                    bool mustPreAndPostBuild = !preBuildHandlers.Contains(handler);
                    if (mustPreAndPostBuild)
                    {
                        preBuildHandlers.Add(handler);
                        theClass.GetTypeInfo().GetDeclaredMethod(PreBuildMethod).Invoke(handler, null);
                    }

                    if (_dataStore.TypedData.ContainsKey(stateKeyValuePair.Key))
                        foreach (object data in _dataStore.TypedData[stateKeyValuePair.Key])
                            theClass.GetTypeInfo().GetDeclaredMethod(WithDataMethod).Invoke(handler, new[] { data });

                    theClass.GetTypeInfo().GetDeclaredMethod(BuildMethod).Invoke(handler, new object[] { stateKeyValuePair.Key });

                    if (mustPreAndPostBuild)
                        postBuildMethods.Add(() => theClass.GetTypeInfo().GetDeclaredMethod(PostBuildMethod).Invoke(handler, null));
                }
            }

            foreach (Action postBuildMethod in postBuildMethods)
                postBuildMethod();
        }

        public void WithBuilderForData<T>() =>
            _typedStateEnumsDelegates[typeof(T)] = () => from stateHandler in _container.TryResolveAll<IStateHandler<T>>() select stateHandler as object;
    }
}