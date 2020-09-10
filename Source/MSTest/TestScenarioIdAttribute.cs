using System;
using LeanTest.Attribute;

namespace LeanTest.MSTest
{
	/// <summary>The test scenario ID attribute.</summary>
	[Obsolete("Use tags from the LeanTest.Attribute namespace instead")]
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
	public class TestScenarioIdAttribute : System.Attribute, IAttributeValue
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