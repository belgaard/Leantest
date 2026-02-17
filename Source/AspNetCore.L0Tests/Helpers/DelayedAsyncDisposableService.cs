using System;
using System.Threading.Tasks;

namespace AspNetCore.L0Tests.Helpers
{
	public sealed class DelayedAsyncDisposableService : IAsyncDisposable
	{
		public async ValueTask DisposeAsync()
		{
			//forces ValueTask to be asynchronous and not be immediately completed
			await Task.Yield();
		}
	}
}
