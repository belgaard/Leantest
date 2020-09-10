using System.Collections.Generic;
using System.Reflection;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace LeanTest.Xunit.XunitExtensions
{
	/// <inheritdoc />
	public class XunitTestFrameworkExecutorWithAssemblyFixture : XunitTestFrameworkExecutor
    {
	    /// <inheritdoc />
	    public XunitTestFrameworkExecutorWithAssemblyFixture(AssemblyName assemblyName, ISourceInformationProvider sourceInformationProvider, IMessageSink diagnosticMessageSink)
            : base(assemblyName, sourceInformationProvider, diagnosticMessageSink)
        { }

	    /// <inheritdoc />
	    protected override async void RunTestCases(IEnumerable<IXunitTestCase> testCases, IMessageSink executionMessageSink, ITestFrameworkExecutionOptions executionOptions)
        {
            using (var assemblyRunner = new XunitTestAssemblyRunnerWithAssemblyFixture(TestAssembly, testCases, DiagnosticMessageSink, executionMessageSink, executionOptions))
                await assemblyRunner.RunAsync();
        }
    }
}