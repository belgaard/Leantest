namespace State.Examples.MsTest.Application
{
    public class MyApplicationService
    {
        private readonly IMyDataLayer _myDataLayer;

        public MyApplicationService(IMyDataLayer myDataLayer)
        {
            _myDataLayer = myDataLayer;
        }
        public int GetAge(string key)
        {
            return _myDataLayer.ReadAgeRecord(key);
        }
    }
}
