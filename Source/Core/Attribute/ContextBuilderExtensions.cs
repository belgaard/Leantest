using System;
using System.Linq;
using System.Reflection;
using LeanTest.Core.ExecutionHandling;

namespace LeanTest.Attribute
{
    /// <summary>Adds support for adding information on tests to test run results.</summary>
    public static class ContextBuilderExtensions
    {
        private static class EmptyArray<T>
        {
            internal static readonly T[] Value = new T[0];
        }
        /// <summary>Registers an intent to use the <c>TestScenarioId</c> attribute on test methods.</summary>
        /// <remarks>This causes LeanTest scenario IDs to be written to the test log (.trx-file).</remarks>
        public static ContextBuilder RegisterScenarioId(this ContextBuilder theContextBuilder, string testName, Assembly assembly = null, Type extraType = null)
        {
            assembly ??= Assembly.GetCallingAssembly();
            Type[] types = BothTypes(extraType, typeof(TestScenarioIdAttribute));
            MethodInfo[] allMethodsForTest = GetMethodsForTest(testName, assembly);

            foreach (Type type in types)
                foreach (MethodInfo methodInfo in allMethodsForTest)
                    foreach (IAttributeValue attribute in methodInfo.GetCustomAttributes(type, false))
                        Console.WriteLine($@"{TestScenarioIdAttribute.Prefix}{attribute.Value}{TestScenarioIdAttribute.Postfix}");

            return theContextBuilder;
        }

        /// <summary>Registers an intent to use the <c>TestTag</c> attribute on test methods.</summary>
        /// <remarks>This causes LeanTest tags to be written to the test log (.trx-file).</remarks>
        public static ContextBuilder RegisterTags(this ContextBuilder theContextBuilder, string testName, Assembly assembly = null, Type extraType = null)
        {
            assembly ??= Assembly.GetCallingAssembly();
            Type[] types = BothTypes(extraType, typeof(TestTagAttribute));
            MethodInfo[] allMethodsForTest = GetMethodsForTest(testName, assembly);

            foreach (Type type in types)
                foreach (MethodInfo methodInfo in allMethodsForTest)
                    foreach (IAttributeValue attribute in methodInfo.GetCustomAttributes(type, false))
                        Console.WriteLine($@"{TestTagAttribute.Prefix}{attribute.Value}{TestTagAttribute.Postfix}");

            return theContextBuilder;
        }

        /// <summary>Registers an intent to use the <c>Description</c> attribute on test methods.</summary>
        /// <remarks>This causes MsTest descriptions to be written to the test log (.trx-file).</remarks>
        public static ContextBuilder RegisterDescription(this ContextBuilder theContextBuilder, string testName, Assembly assembly = null, Type extraType = null)
        {
            assembly ??= Assembly.GetCallingAssembly();
            Type[] types = BothTypes(extraType, typeof(TestDescriptionAttribute));
            MethodInfo[] allMethodsForTest = GetMethodsForTest(testName, assembly);

            foreach (Type type in types)
                foreach (MethodInfo methodInfo in allMethodsForTest)
                    foreach (IAttributeValue attribute in methodInfo.GetCustomAttributes(type, false))
                        Console.WriteLine($@"{TestDescriptionAttribute.Prefix}{attribute.Value}{TestDescriptionAttribute.Postfix}");

            return theContextBuilder;
        }

        /// <summary>Registers an intent to use the LeanTest attribute on test methods.</summary>
        /// <remarks>This causes LeanTest scenario IDs and tags as well as MsTest descriptions to be written to the test log (.trx-file).</remarks>
        public static ContextBuilder RegisterAttributes(this ContextBuilder theContextBuilder, string testName, Assembly assembly = null)
        {
            assembly ??= Assembly.GetCallingAssembly();
            return theContextBuilder
                .RegisterDescription(testName, assembly)
                .RegisterScenarioId(testName, assembly)
                .RegisterTags(testName, assembly);
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
    }
}