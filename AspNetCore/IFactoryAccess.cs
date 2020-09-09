using System.Net.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;

namespace LeanTest
{
	/// <summary>Access to the test server and http client, typically through a <see cref="WebApplicationFactory{TEntryPoint}"/>.</summary>
	public interface IFactoryAccess
	{
		/// <summary>Gets the <see cref="TestServer"/> created by a <see cref="WebApplicationFactory{TEntryPoint}"/>.</summary>
		public TestServer Server { get; }
		/// <summary>Creates an instance of <see cref="HttpClient"/> that automatically follows redirects and handles cookies.</summary>
		/// <returns>The <see cref="HttpClient"/>.</returns>
		public HttpClient CreateClient();
	}
	/// <summary>Access to the <see cref="WebApplicationFactory{TEntryPoint}"/>.</summary>
	public interface IFactoryAccess<TEntryPoint> : IFactoryAccess  where TEntryPoint: class
	{
		/// <summary>Factory for bootstrapping an application in memory for functional end to end tests.</summary>
		public WebApplicationFactory<TEntryPoint> GetFactory();
	}
}