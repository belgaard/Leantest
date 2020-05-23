using System.Threading.Tasks;

namespace Template.L0Tests.Application
{
	public class MyApplicationService
	{
		private readonly IMyExternalService _myExternalService;

		public MyApplicationService(IMyExternalService myExternalService) => _myExternalService = myExternalService;
		public int GetAge(string key) => _myExternalService.GetAge(key);
		public async Task<int> GetAgeAsync(string key) => await _myExternalService.GetAgeAsync(key).ConfigureAwait(false);
	}
}