using Microsoft.Extensions.DependencyInjection;

namespace Template.L0Tests.Application
{
	internal static class CompositionRoot
	{
		internal static IServiceCollection Initialize(IServiceCollection services) => services.AddSingleton<MyApplicationService>();
	}
}