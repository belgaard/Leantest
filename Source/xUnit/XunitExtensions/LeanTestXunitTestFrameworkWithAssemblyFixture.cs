using System.Reflection;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace LeanTest.Xunit.XunitExtensions
{
    /// <inheritdoc />
    public class LeanTestXunitTestFrameworkWithAssemblyFixture : XunitTestFramework
    {
        /// <inheritdoc />
        public LeanTestXunitTestFrameworkWithAssemblyFixture(IMessageSink messageSink)
            : base(messageSink) { }

        /// <inheritdoc />
        protected override ITestFrameworkExecutor CreateExecutor(AssemblyName assemblyName)
            => new XunitTestFrameworkExecutorWithAssemblyFixture(assemblyName, SourceInformationProvider, DiagnosticMessageSink);
    }
}