using System;
using System.Linq;
using System.Reflection;
using LeanTest.Core.ExecutionHandling;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LeanTest.MSTest
{
    /// <summary>
    /// Adds support for adding test data in Json format.
    /// </summary>
    public static class ContextBuilderExtensions
    {
		/// <summary>Registers an intend to use the <c>TestScenarioId</c> attribute on test methods.</summary>
		/// <remarks>This causes scenario IDs to be written to the test log (.trx-file).</remarks>
		public static ContextBuilder RegisterScenarioId(this ContextBuilder theContextBuilder, TestContext testContext)
	    {
		    Assembly assembly = Assembly.GetCallingAssembly();
		    MethodInfo[] methods = assembly.GetTypes()
			    .SelectMany(t => t.GetMethods())
			    .Where(m => m.GetCustomAttributes(typeof(TestScenarioIdAttribute), false).Length > 0)
			    .Where(m => m.Name == testContext.TestName)
			    .ToArray();

		    foreach (MethodInfo methodInfo in methods)
		    foreach (TestScenarioIdAttribute testScenarioIdAttribute in methodInfo.GetCustomAttributes(typeof(TestScenarioIdAttribute), false))
			    Console.WriteLine($@"{TestScenarioIdAttribute.Prefix}{testScenarioIdAttribute?.Value}{TestScenarioIdAttribute.Postfix}");

		    return theContextBuilder;
	    }
   }
}