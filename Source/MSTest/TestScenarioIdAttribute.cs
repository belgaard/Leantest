using System;

namespace LeanTest.MSTest
{
	/// <summary>The test scenario ID attribute.</summary>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
	public class TestScenarioIdAttribute : Attribute
	{
		public const string Prefix = @"TestScenarioId = ###---";
		public const string Postfix = @"---###";

		/// <inheritdoc />
		public TestScenarioIdAttribute(string value) => Value = value;
		/// <summary>The value.</summary>
		public string Value { get; }
	}
}