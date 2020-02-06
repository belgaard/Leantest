namespace Core.Examples.L0Tests.Application
{
	public interface IMyExternalService
	{
		int GetAge(string key);
	}
	public class MyApplicationService
	{
		private readonly IMyExternalService _myExternalService;

		public MyApplicationService(IMyExternalService myExternalService)
		{
			_myExternalService = myExternalService;
		}
		public int GetAge(string key)
		{
			return _myExternalService.GetAge(key);
		}
	}
}