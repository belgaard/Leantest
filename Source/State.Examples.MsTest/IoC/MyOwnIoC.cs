using System;
using System.Collections.Generic;
using System.Linq;
using ExampleApp.Domain;
using LeanTest.Core.ExecutionHandling;
using State.Examples.MsTest.Application;
using State.Examples.MsTest.StateHandlers;

namespace State.Examples.MsTest.IoC
{
    /// <summary>
    /// An IoC container made up for this example. In real-world examples you would use SimpleInjector, Unity or some 
    /// other popular container. Also, in a real-world example you would have a composition root for the system under 
    /// test - and modifications for that for the tests.
    /// </summary>
    public class MyOwnIoC : IIocContainer
    {
        private readonly MyApplicationService _myApplicationService;
        private readonly MyDataLayer _myDataLayer;
        private readonly MyStateHandler _myStateHandler;

        public MyOwnIoC()
        {
            _myDataLayer = new MyDataLayer();
            _myApplicationService = new MyApplicationService(_myDataLayer);
            _myStateHandler = new MyStateHandler();
        }

        public T Resolve<T>() where T : class
        {
            if (typeof(T) == typeof(MyApplicationService))
                return _myApplicationService as T;
            if (typeof(T) == typeof(IMyDataLayer))
                return _myDataLayer as T;

            throw new ArgumentException();
        }

        public T TryResolve<T>() where T : class
        {
            try
            {
                return Resolve<T>();
            }
            catch (Exception)
            {
                return null;
            }
        }

        public IEnumerable<T> TryResolveAll<T>() where T : class
        {
            if (typeof(T) == typeof(IStateHandler<MyData>))
                return from stateHandler in new List<IStateHandler<MyData>> { _myStateHandler } select (T)stateHandler;
            if (typeof(T) == typeof(IStateHandler<MyOtherData>))
                return from stateHandler in new List<IStateHandler<MyOtherData>> { _myStateHandler } select (T)stateHandler;

            return new List<T>();
        }
    }
}
