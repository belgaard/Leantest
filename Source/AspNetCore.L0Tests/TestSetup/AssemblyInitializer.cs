using AspNetCore.L0Tests.TestSetup.IoC;
using LeanTest;
using LeanTest.Core.ExecutionHandling;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Hosting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Reflection;

namespace AspNetCore.L0Tests.TestSetup
{
	[TestClass]
	public static class AssemblyInitializer
	{
		[AssemblyInitialize]
		public static void AssemblyInitialize(TestContext _)
		{
			static WebApplicationFactory<EntryPoint> FactoryFactory()
			{
				WebApplicationFactory<EntryPoint> factory = new WebApplicationFactory<EntryPoint>();
				factory = factory.WithWebHostBuilder(
					builder =>
					{
						// Set the content root to the test assembly's directory
						var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
						builder.UseContentRoot(assemblyPath ?? Directory.GetCurrentDirectory());
						builder.ConfigureTestServices(L0CompositionRootForTest.Initialize);
					});

				return factory;
			}
			AspNetCoreContextBuilderFactory.Initialize(FactoryFactory, provider => new IoCContainer(provider));
		}

		[AssemblyCleanup]
		public static void AssemblyCleanup()
		{
			ContextBuilderFactory.Cleanup();
		}

		public class EntryPoint
		{
			public static void Main(string[] args)
			{
				CreateHostBuilder(args).Build().Run();
			}

			public static IHostBuilder CreateHostBuilder(string[] args) =>
				Host.CreateDefaultBuilder(args)
					.ConfigureWebHostDefaults(webBuilder =>
					{
						webBuilder.Configure(app =>
						{
							app.Map("/test", builder => builder.Run(async context =>
							{
								await context.Response.WriteAsync("Hello world");
							}));
						});
					});
		}
	}
}
