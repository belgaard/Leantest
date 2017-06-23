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
            var preAndPostBuiltHandlers = new List<object>();
            foreach (KeyValuePair<Type, Func<IEnumerable<object>>> stateKeyValuePair in _typedStateEnumsDelegates)
            {
                IEnumerable<object> handlers = stateKeyValuePair.Value().ToArray();
                
                foreach (object handler in handlers)
                {
                    bool mustPreAndPostBuild = !preAndPostBuiltHandlers.Contains(handler);
                    if (mustPreAndPostBuild)
                        preAndPostBuiltHandlers.Add(handler);

                    Type theClass = typeof(IStateHandler<>).MakeGenericType(stateKeyValuePair.Key);

                    if (mustPreAndPostBuild)
                    {
                        MethodInfo preBuildMethod = theClass.GetTypeInfo().GetDeclaredMethod(PreBuildMethod);
                        preBuildMethod.Invoke(handler, null);
                    }

                    MethodInfo withDataMethod = theClass.GetTypeInfo().GetDeclaredMethod(WithDataMethod);
                    foreach (object data in _dataStore.TypedData[stateKeyValuePair.Key])
                        withDataMethod.Invoke(handler, new[] { data });

                    MethodInfo buildMethod = theClass.GetTypeInfo().GetDeclaredMethod(BuildMethod);
                    buildMethod.Invoke(handler, new object[] { stateKeyValuePair.Key });

                    if (!mustPreAndPostBuild)
                        continue;

                    MethodInfo postBuildMethod = theClass.GetTypeInfo().GetDeclaredMethod(PostBuildMethod);
                    postBuildMethod.Invoke(handler, null);
                }
            }
        }

        public void WithBuilderForData<T>() =>
            _typedStateEnumsDelegates[typeof(T)] = () => from stateHandler in _container.TryResolveAll<IStateHandler<T>>() select stateHandler as object;
    }
}