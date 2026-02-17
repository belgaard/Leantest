using AspNetCore.L0Tests.Helpers;
using Microsoft.Extensions.DependencyInjection;

namespace AspNetCore.L0Tests.TestSetup.IoC
{
	public static class L0CompositionRootForTest
	{
		public static void Initialize(IServiceCollection services)
		{
			services.AddSingleton<DelayedAsyncDisposableService>();
		}		
	}
}
