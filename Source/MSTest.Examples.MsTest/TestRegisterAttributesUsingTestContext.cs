using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
			this.TestContext.RegisterAttributes();
			Assert.IsTrue(true);
		}

		[TestMethod]
		public void RegisterAttributesMustWorkWhenUsingTextContextAndPassingAssembly()
		{
			this.TestContext.RegisterAttributes(System.Reflection.Assembly.GetExecutingAssembly());
			Assert.IsTrue(true);
		}
	}
}
