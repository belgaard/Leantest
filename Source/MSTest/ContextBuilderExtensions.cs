using System;
using System.Linq;
using System.Reflection;
using LeanTest.Core.ExecutionHandling;
using LeanTest.Attribute;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LeanTest.MSTest
{
	/// <summary>Adds support for adding information on tests to test run results.</summary>
    public static class ContextBuilderExtensions
    {
        /// <summary>Registers an intent to use the <c>TestScenarioId</c> attribute on test methods.</summary>
        /// <remarks>This causes LeanTest scenario IDs to be written to the test log (.trx-file).</remarks>
        public static ContextBuilder RegisterScenarioId(this ContextBuilder theContextBuilder, TestContext testContext, Assembly assembly = null) => 
	        theContextBuilder.RegisterScenarioId(testContext.TestName, assembly ?? Assembly.GetCallingAssembly(), typeof(TestScenarioIdAttribute));

        /// <summary>Registers an intent to use the <c>TestTag</c> attribute on test methods.</summary>
        /// <remarks>This causes LeanTest tags to be written to the test log (.trx-file).</remarks>
        public static ContextBuilder RegisterTags(this ContextBuilder theContextBuilder, TestContext testContext, Assembly assembly = null) => 
	        theContextBuilder.RegisterTags(testContext.TestName, assembly ?? Assembly.GetCallingAssembly(), typeof(TestTagAttribute));

        /// <summary>Registers an intent to use the <c>Description</c> attribute on test methods.</summary>
        /// <remarks>This causes MsTest descriptions to be written to the test log (.trx-file).</remarks>
        public static ContextBuilder RegisterDescription(this ContextBuilder theContextBuilder, TestContext testContext, Assembly assembly = null)
        {
	        theContextBuilder.RegisterDescription(testContext.TestName, assembly ?? Assembly.GetCallingAssembly());

			// The following is for backwards compatibility with the MsTest Description attribute. We prefer to use the new LeanTest TestDescription
			// attribute:
            assembly ??= Assembly.GetCallingAssembly();
            MethodInfo[] methods = assembly.GetTypes()
                .SelectMany(t => t.GetMethods())
                .Where(m => m.GetCustomAttributes(typeof(DescriptionAttribute), false).Length > 0)
                .Where(m => m.Name == testContext.TestName)
                .ToArray();

            foreach (MethodInfo methodInfo in methods)
                foreach (DescriptionAttribute descriptionAttribute in methodInfo.GetCustomAttributes(typeof(DescriptionAttribute), false))
                    Console.WriteLine($@"{TestDescriptionAttribute.Prefix}{descriptionAttribute.Description}{TestDescriptionAttribute.Postfix}");

            return theContextBuilder;
        }
        /// <summary>Registers an intent to use the LeanTest attribute on test methods.</summary>
        /// <remarks>This causes LeanTest scenario IDs and tags as well as MsTest descriptions to be written to the test log (.trx-file).</remarks>
        public static ContextBuilder RegisterAttributes(this ContextBuilder theContextBuilder, TestContext testContext, Assembly assembly = null)
        {
            assembly ??= Assembly.GetCallingAssembly();
            return theContextBuilder
                .RegisterDescription(testContext, assembly)
                .RegisterScenarioId(testContext, assembly)
                .RegisterTags(testContext, assembly);
        }
    }
}