using Core.Examples.MsTest.IoC;

namespace Core.Examples.MsTest.Application
{
    public class MyApplicationService
    {
        public int Sum(MyData myData)
        {
            return myData.First + myData.Second;
        }
    }
}