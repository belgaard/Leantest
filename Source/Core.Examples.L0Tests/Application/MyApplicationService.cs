using System.Threading.Tasks;
using Core.Examples.L0Tests.Domain;

namespace Core.Examples.L0Tests.Application
{
    public class MyApplicationService
    {
		public int Sum(MyData myData)
        {
            return myData.First + myData.Second;
        }

		public void DivideByZero()
        {
            throw new System.DivideByZeroException();
        }

        public Task<MyData> DivideByZeroAsync(int theInt)
        {
            throw new System.DivideByZeroException();
        }
    }
}