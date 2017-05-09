using System;
using System.Collections.Generic;
using Core.Examples.MsTest.Application;
using LeanTest.Core.ExecutionHandling;

namespace Core.Examples.MsTest.Domain
{
    /// <summary>
    /// An IoC container made up for this example. In real-world examples you would use SimpleInjector, Unity or some 
    /// other popular container. Also, in a real-world example you would have a composition root for the system under 
    /// test - and modifications for that for the tests.
    /// </summary>
    public class MyOwnIoC : IIocContainer
    {
        private readonly MyApplicationService _myApplicationService;

        public MyOwnIoC()
        {
            _myApplicationService = new MyApplicationService();
        }

        public T Resolve<T>() where T : class
        {
            if (typeof(T) == typeof(MyApplicationService))
                return _myApplicationService as T;

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
            return new List<T>();
        }
    }
}