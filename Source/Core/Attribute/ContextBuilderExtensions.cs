using System;
using System.Linq;
using System.Reflection;
using LeanTest.Core.ExecutionHandling;

namespace LeanTest.Attribute
{
	/// <summary>Adds support for adding information on tests to test run results.</summary>
	public static class ContextBuilderExtensions
    {
	    private static readonly IStdOut DefaultStdOut = new StdOutConsole();

        /// <summary>Registers an intent to use the <c>TestScenarioId</c> attribute on test methods.</summary>
        /// <remarks>This causes LeanTest scenario IDs to be written to the test log (.trx-file).</remarks>
        public static ContextBuilder RegisterScenarioId(this ContextBuilder theContextBuilder, string testName, Assembly assembly = null, Type extraType = null, IStdOut stdOut = null)
        {
            assembly ??= Assembly.GetCallingAssembly();
            stdOut ??= DefaultStdOut;
            Type[] types = BothTypes(extraType, typeof(TestScenarioIdAttribute));
            MethodInfo[] allMethodsForTest = GetMethodsForTest(testName, assembly);

            foreach (Type type in types)
                foreach (MethodInfo methodInfo in allMethodsForTest)
                    foreach (IAttributeValue attribute in methodInfo.GetCustomAttributes(type, false))
                        stdOut.WriteLine($@"{TestScenarioIdAttribute.Prefix}{attribute.Value}{TestScenarioIdAttribute.Postfix}");

            return theContextBuilder;
        }

        /// <summary>Registers an intent to use the <c>TestTag</c> attribute on test methods.</summary>
        /// <remarks>This causes LeanTest tags to be written to the test log (.trx-file).</remarks>
        public static ContextBuilder RegisterTags(this ContextBuilder theContextBuilder, string testName, Assembly assembly = null, Type extraType = null, IStdOut stdOut = null)
        {
            assembly ??= Assembly.GetCallingAssembly();
            stdOut ??= DefaultStdOut;
            Type[] types = BothTypes(extraType, typeof(TestTagAttribute));
            MethodInfo[] allMethodsForTest = GetMethodsForTest(testName, assembly);

            foreach (Type type in types)
                foreach (MethodInfo methodInfo in allMethodsForTest)
                    foreach (IAttributeValue attribute in methodInfo.GetCustomAttributes(type, false))
                        stdOut.WriteLine($@"{TestTagAttribute.Prefix}{attribute.Value}{TestTagAttribute.Postfix}");

            return theContextBuilder;
        }

        /// <summary>Registers an intent to use the <c>Description</c> attribute on test methods.</summary>
        /// <remarks>This causes MsTest descriptions to be written to the test log (.trx-file).</remarks>
        public static ContextBuilder RegisterDescription(this ContextBuilder theContextBuilder, string testName, Assembly assembly = null, Type extraType = null, IStdOut stdOut = null)
        {
            assembly ??= Assembly.GetCallingAssembly();
            stdOut ??= DefaultStdOut;
            Type[] types = BothTypes(extraType, typeof(TestDescriptionAttribute));
            MethodInfo[] allMethodsForTest = GetMethodsForTest(testName, assembly);

            foreach (Type type in types)
                foreach (MethodInfo methodInfo in allMethodsForTest)
                    foreach (IAttributeValue attribute in methodInfo.GetCustomAttributes(type, false))
                        stdOut.WriteLine($@"{TestDescriptionAttribute.Prefix}{attribute.Value}{TestDescriptionAttribute.Postfix}");

            return theContextBuilder;
        }

        /// <summary>Registers an intent to use the LeanTest attribute on test methods.</summary>
        /// <remarks>This causes LeanTest scenario IDs and tags as well as MsTest descriptions to be written to the test log (.trx-file).</remarks>
        public static ContextBuilder RegisterAttributes(this ContextBuilder theContextBuilder, string testName, Assembly assembly = null, IStdOut stdOut = null)
        {
            assembly ??= Assembly.GetCallingAssembly();
            return theContextBuilder
                .RegisterDescription(testName, assembly, null, stdOut)
                .RegisterScenarioId(testName, assembly, null, stdOut)
                .RegisterTags(testName, assembly, null, stdOut);
        }
 
        private static Type[] BothTypes(Type extraType, Type thisType)
        {		
            Type[] types = extraType == null
                ? new[] {thisType}
                : new[] {thisType, extraType};
            return types;
        }

        private static MethodInfo[] GetMethodsForTest(string testName, Assembly assembly)
        {
            return assembly.GetTypes()
                .SelectMany(t => t.GetMethods())
                .Where(m => m.Name == testName).ToArray();
        }
        private class StdOutConsole : IStdOut
        {
	        public void WriteLine(string value) => Console.WriteLine(value);
        }
    }
}