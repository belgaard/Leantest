namespace Examples.L0Tests.Application
{
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