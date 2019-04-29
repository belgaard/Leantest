using Microsoft.VisualStudio.TestTools.UnitTesting;
using LeanTest.MSTest;

namespace MSTest.Examples.MsTest
{
	[TestClass]
	public class TestRegisterAttributesUsingTestContext
	{
		// Populated by MS Test
		public TestContext TestContext { get; set; }

		[TestMethod]
		public void RegisterAttributesMustWorkWhenUsingTextContext()
		{
			TestContext.RegisterAttributes();
			Assert.IsTrue(true);
		}

		[TestMethod]
		public void RegisterAttributesMustWorkWhenUsingTextContextAndPassingAssembly()
		{
			TestContext.RegisterAttributes(System.Reflection.Assembly.GetExecutingAssembly());
			Assert.IsTrue(true);
		}
	}
}
