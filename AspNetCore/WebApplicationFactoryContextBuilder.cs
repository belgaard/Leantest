using System.Linq;
using System.Net.Http;
using LeanTest.Core.ExecutionHandling;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;

namespace LeanTest
{
    /// <summary>A context builder which is initialized with a <c>WebApplicationFactory</c>.</summary>
    /// <typeparam name="TEntryPoint">The entry point for the <c>WebApplicationFactory</c>, typically the Startup class.</typeparam>
    /// <remarks>The web application factory and its test server and HTTP client can be accessed through the IFactoryAccess interface.</remarks>
    public class WebApplicationFactoryContextBuilder<TEntryPoint> : ContextBuilder, IFactoryAccess<TEntryPoint>  where TEntryPoint: class
    {
        private readonly WebApplicationFactory<TEntryPoint> _factory;

        /// <inheritdoc />
        public WebApplicationFactoryContextBuilder(WebApplicationFactory<TEntryPoint> factory, IIocContainer container)
            : base(container, ContextBuilderFactory.BuilderFactories.ToArray()) => _factory = factory;

        /// <inheritdoc />
        public WebApplicationFactory<TEntryPoint> GetFactory() => _factory;
        /// <inheritdoc />
        public TestServer Server => _factory?.Server;
        /// <inheritdoc />
        public HttpClient CreateClient() => _factory?.CreateClient();
    }
}