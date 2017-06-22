namespace State.Examples.MsTest.Application
{
	internal class MyDataLayer : IMyDataLayer
	{
		public int ReadAgeRecord(string key)
		{
			// TODO: Read from e.g. a SQL database!
			return 10;
		}
	}
}