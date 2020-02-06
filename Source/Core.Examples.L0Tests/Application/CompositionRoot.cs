using Microsoft.Extensions.DependencyInjection;

namespace Core.Examples.L0Tests.Application
{
	internal static class CompositionRoot
	{
		internal static IServiceCollection Initialize(IServiceCollection services)
		{
			services.AddSingleton<MyApplicationService>();

			return services;
		}
	}
}