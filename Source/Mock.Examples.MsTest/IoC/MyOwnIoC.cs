using System;
using System.Collections.Generic;
using System.Linq;
using LeanTest.Core.ExecutionHandling;
using LeanTest.Mock;
using Mock.Examples.MsTest.Application;
using Mock.Examples.MsTest.Domain;
using Mock.Examples.MsTest.Mocks;

namespace Mock.Examples.MsTest.IoC
{
    /// <summary>
    /// An IoC container made up for this example. In real-world examples you would use SimpleInjector, Unity or some 
    /// other popular container. Also, in a real-world example you would have a composition root for the system under 
    /// test - and modifications for that for the tests.
    /// </summary>
    public class MyOwnIoC : IIocContainer
    {
        private readonly MyApplicationService _myApplicationService;
        private readonly MockMyExternalService _myExternalService;

        public MyOwnIoC()
        {
            _myExternalService = new MockMyExternalService();
            _myApplicationService = new MyApplicationService(_myExternalService);
        }

        public T Resolve<T>() where T : class
        {
            if (typeof(T) == typeof(MyApplicationService))
                return _myApplicationService as T;
            if (typeof(T) == typeof(IMyExternalService))
                return _myExternalService as T;

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
            if (typeof(T) == typeof(IMockForData<MyData>))
                return from mock in new List<IMockForData<MyData>> { _myExternalService } select (T)mock;
            if (typeof(T) == typeof(IMockForData<MyOtherData>))
                return from mock in new List<IMockForData<MyOtherData>> { _myExternalService } select (T)mock;

            return new List<T>();
        }
    }
}
