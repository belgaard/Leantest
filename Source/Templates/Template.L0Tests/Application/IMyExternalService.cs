using System.Threading.Tasks;

namespace Template.L0Tests.Application
{
	public interface IMyExternalService
	{
		int GetAge(string key);
		Task<int> GetAgeAsync(string key);
	}
}