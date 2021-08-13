using System.Reflection;
using LeanTest.Attribute;
using LeanTest.Core.ExecutionHandling;
using Xunit.Abstractions;

namespace LeanTest.Xunit
{
    /// <summary>Adds support for adding information on tests to test run results.</summary>
    public static class ContextBuilderExtensions
    {
        /// <summary>Registers an intent to use the <c>TestScenarioId</c> attribute on test methods.</summary>
        /// <remarks>This causes LeanTest scenario IDs to be written to the test log (.trx-file).</remarks>
        public static ContextBuilder RegisterScenarioId(this ContextBuilder theContextBuilder, TestContext testContext, Assembly assembly = null, IStdOut stdOut = null) => 
            theContextBuilder.RegisterScenarioId(testContext.MethodName, assembly ?? Assembly.GetCallingAssembly(), typeof(TestScenarioIdAttribute), stdOut ?? new StdOutTestOutputHelper(testContext));

        /// <summary>Registers an intent to use the <c>TestTag</c> attribute on test methods.</summary>
        /// <remarks>This causes LeanTest tags to be written to the test log (.trx-file).</remarks>
        public static ContextBuilder RegisterTags(this ContextBuilder theContextBuilder, TestContext testContext, Assembly assembly = null, IStdOut stdOut = null) => 
            theContextBuilder.RegisterTags(testContext.MethodName, assembly ?? Assembly.GetCallingAssembly(), typeof(TestTagAttribute), stdOut ?? new StdOutTestOutputHelper(testContext));

        /// <summary>Registers an intent to use the <c>Description</c> attribute on test methods.</summary>
        /// <remarks>This causes MsTest descriptions to be written to the test log (.trx-file).</remarks>
        public static ContextBuilder RegisterDescription(this ContextBuilder theContextBuilder, TestContext testContext, Assembly assembly = null, IStdOut stdOut = null) =>
	        theContextBuilder.RegisterDescription(testContext.MethodName, assembly ?? Assembly.GetCallingAssembly(), typeof(TestDescriptionAttribute), stdOut ?? new StdOutTestOutputHelper(testContext));

        /// <summary>Registers an intent to use the LeanTest attribute on test methods.</summary>
        /// <remarks>This causes LeanTest scenario IDs and tags as well as MsTest descriptions to be written to the test log (.trx-file).</remarks>
        public static ContextBuilder RegisterAttributes(this ContextBuilder theContextBuilder, TestContext testContext, Assembly assembly = null)
        {
            assembly ??= Assembly.GetCallingAssembly();
            IStdOut stdOut = new StdOutTestOutputHelper(testContext);
            return theContextBuilder
                .RegisterDescription(testContext, assembly, stdOut)
                .RegisterScenarioId(testContext, assembly, stdOut)
                .RegisterTags(testContext, assembly, stdOut);
        }

	    private class StdOutTestOutputHelper : IStdOut
	    {
		    private readonly TestContext _testContext;

			public StdOutTestOutputHelper(TestContext testContext) => _testContext = testContext;
			public void WriteLine(string value) => _testContext.WriteLine(value);
	    }
    }
}
