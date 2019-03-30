using System;

namespace LeanTest.MSTest
{
	/// <inheritdoc />
	/// <summary>The test scenario ID attribute.</summary>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
	public class TestScenarioIdAttribute : Attribute
	{
		/// <summary>A prefix put in a .trx-file before each scenario ID value.</summary>
		public const string Prefix = @"TestScenarioId = ###---";
		/// <summary>A postfix put in a .trx-file after each scenario ID value.</summary>
		public const string Postfix = @"---###";

		/// <inheritdoc />
		public TestScenarioIdAttribute(string value) => Value = value;
		/// <summary>The value.</summary>
		public string Value { get; }
	}
}