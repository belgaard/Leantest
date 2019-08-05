using System.Threading.Tasks;
using Core.Examples.MsTest.IoC;

namespace Core.Examples.MsTest.Application
{
    public class MyApplicationService
    {
#region Add function
		public int Sum(MyData myData)
        {
            return myData.First + myData.Second;
        }
#endregion
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