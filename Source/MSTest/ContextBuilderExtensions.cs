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
		/// <remarks>This causes LeanTest scenario IDs to be written to the test log (.trx-file).</remarks>
		public static IContextBuilder RegisterScenarioId(this IContextBuilder theContextBuilder, TestContext testContext, Assembly assembly = null)
	    {
	        assembly ??= Assembly.GetCallingAssembly();
		    MethodInfo[] methods = assembly.GetTypes()
			    .SelectMany(t => t.GetMethods())
			    .Where(m => m.GetCustomAttributes(typeof(TestScenarioIdAttribute), false).Length > 0)
			    .Where(m => m.Name == testContext.TestName)
			    .ToArray();

		    foreach (MethodInfo methodInfo in methods)
		        foreach (TestScenarioIdAttribute testScenarioIdAttribute in methodInfo.GetCustomAttributes(typeof(TestScenarioIdAttribute), false))
			        Console.WriteLine($@"{TestScenarioIdAttribute.Prefix}{testScenarioIdAttribute.Value}{TestScenarioIdAttribute.Postfix}");

		    return theContextBuilder;
	    }
		/// <summary>Registers an intend to use the <c>TestTag</c> attribute on test methods.</summary>
		/// <remarks>This causes LeanTest tags to be written to the test log (.trx-file).</remarks>
		public static IContextBuilder RegisterTags(this IContextBuilder theContextBuilder, TestContext testContext, Assembly assembly = null)
	    {
		    assembly ??= Assembly.GetCallingAssembly();
		    MethodInfo[] methods = assembly.GetTypes()
			    .SelectMany(t => t.GetMethods())
			    .Where(m => m.GetCustomAttributes(typeof(TestTagAttribute), false).Length > 0)
			    .Where(m => m.Name == testContext.TestName)
			    .ToArray();

		    foreach (MethodInfo methodInfo in methods)
		        foreach (TestTagAttribute testTagAttribute in methodInfo.GetCustomAttributes(typeof(TestTagAttribute), false))
			        Console.WriteLine($@"{TestTagAttribute.Prefix}{testTagAttribute.Value}{TestTagAttribute.Postfix}");

		    return theContextBuilder;
	    }
		/// <summary>Registers an intend to use the <c>Description</c> attribute on test methods.</summary>
		/// <remarks>This causes MsTest descriptions to be written to the test log (.trx-file).</remarks>
		public static IContextBuilder RegisterDescription(this IContextBuilder theContextBuilder, TestContext testContext, Assembly assembly = null)
	    {
		    assembly ??= Assembly.GetCallingAssembly();
		    MethodInfo[] methods = assembly.GetTypes()
			    .SelectMany(t => t.GetMethods())
			    .Where(m => m.GetCustomAttributes(typeof(DescriptionAttribute), false).Length > 0)
			    .Where(m => m.Name == testContext.TestName)
			    .ToArray();

		    foreach (MethodInfo methodInfo in methods)
		        foreach (DescriptionAttribute descriptionAttribute in methodInfo.GetCustomAttributes(typeof(DescriptionAttribute), false))
			        Console.WriteLine($@"{DescriptionAttributeFix.Prefix}{descriptionAttribute.Description}{DescriptionAttributeFix.Postfix}");

		    return theContextBuilder;
	    }
		/// <summary>Registers an intend to use the LeanTest attribute on test methods.</summary>
		/// <remarks>This causes LeanTest scenario IDs and tags as well as MsTest descriptions to be written to the test log (.trx-file).</remarks>
		public static IContextBuilder RegisterAttributes(this IContextBuilder theContextBuilder, TestContext testContext, Assembly assembly = null)
        {
            assembly ??= Assembly.GetCallingAssembly();
            return theContextBuilder
                .RegisterDescription(testContext, assembly)
                .RegisterScenarioId(testContext, assembly)
                .RegisterTags(testContext, assembly);
        }
    }
}