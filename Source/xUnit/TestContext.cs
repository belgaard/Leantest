using System;
using System.Reflection;
using Xunit.Abstractions;

namespace LeanTest.Xunit
{
	/// <summary>Xunit does not have a TestContext. The following gives us access to the name of the current test.</summary>
	/// <remarks>This code is heavily inspired by https://github.com/SimonCropp/XunitContext#current-test.</remarks>
	public class TestContext
	{
		private readonly ITestOutputHelper _testOutput;
		private ITest _test;

		/// <summary>ctor</summary>
		/// <param name="testOutput">This is what you were passed in your Xunit test class ctor.</param>
		public TestContext(ITestOutputHelper testOutput) => _testOutput = testOutput ?? throw new ArgumentNullException();
		/// <summary></summary>
		public string MethodName => GetTest().TestCase.TestMethod.Method.Name;

		private ITest GetTest() => _test ??= (ITest)GetTestMethod().GetValue(_testOutput);

		private FieldInfo GetTestMethod()
		{
			var testOutputType = _testOutput.GetType();
			var testMethod = testOutputType.GetField("test", BindingFlags.Instance | BindingFlags.NonPublic);
			if (testMethod == null)
				throw new Exception($"Unable to find 'test' field on {testOutputType.FullName}");

			return testMethod;
		}

		internal void WriteLine(string value) => _testOutput.WriteLine(value);
	}
}