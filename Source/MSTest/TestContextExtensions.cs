using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace LeanTest.MSTest
{
	/// <summary>
	/// Extension methods for MS Tests TestContext.
	/// </summary>
	public static class TestContextExtensions
	{
		/// <summary>Registers an intend to use the <c>TestScenarioId</c> attribute on test methods.</summary>
		/// <remarks>This causes scenario IDs to be written to the test log (.trx-file).</remarks>
		public static TestContext RegisterScenarioId(this TestContext testContext, Assembly assembly = null)
		{
			if (testContext == null) throw new ArgumentNullException(nameof(testContext));

			assembly = assembly ?? Assembly.GetCallingAssembly();
			MethodInfo[] methods = assembly.GetTypes()
				.SelectMany(t => t.GetMethods())
				.Where(m => m.GetCustomAttributes(typeof(TestScenarioIdAttribute), false).Length > 0)
				.Where(m => m.Name == testContext.TestName)
				.ToArray();

			foreach (MethodInfo methodInfo in methods)
				foreach (TestScenarioIdAttribute testScenarioIdAttribute in methodInfo.GetCustomAttributes(typeof(TestScenarioIdAttribute), false))
					Console.WriteLine($@"{TestScenarioIdAttribute.Prefix}{testScenarioIdAttribute?.Value}{TestScenarioIdAttribute.Postfix}");

			return testContext;
		}

		/// <summary>Registers an intend to use the LeanTest attribute on test methods.</summary>
		/// <remarks>This causes scenario IDs and tags to be written to the test log (.trx-file).</remarks>
		public static TestContext RegisterTags(this TestContext testContext, Assembly assembly = null)
		{
			if (testContext == null) throw new ArgumentNullException(nameof(testContext));

			assembly = assembly ?? Assembly.GetCallingAssembly();
			MethodInfo[] methods = assembly.GetTypes()
				.SelectMany(t => t.GetMethods())
				.Where(m => m.GetCustomAttributes(typeof(TestTagAttribute), false).Length > 0)
				.Where(m => m.Name == testContext.TestName)
				.ToArray();

			foreach (MethodInfo methodInfo in methods)
				foreach (TestTagAttribute testTagAttribute in methodInfo.GetCustomAttributes(typeof(TestTagAttribute), false))
					Console.WriteLine($@"{TestTagAttribute.Prefix}{testTagAttribute?.Value}{TestTagAttribute.Postfix}");

			return testContext;
		}

		/// <summary>Registers an intend to use the LeanTest attribute on test methods.</summary>
		/// <remarks>This causes scenario IDs and tags to be written to the test log (.trx-file).</remarks>
		public static TestContext RegisterAttributes(this TestContext testContext, Assembly assembly = null)
		{
			if (testContext == null) throw new ArgumentNullException(nameof(testContext));

			assembly = assembly ?? Assembly.GetCallingAssembly();
			return testContext
				.RegisterScenarioId(assembly)
				.RegisterTags(assembly);
		}
	}
}
