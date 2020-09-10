using System;

namespace LeanTest.Attribute
{
	/// <summary>The test description attribute.</summary>
	[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
	public class TestDescriptionAttribute : System.Attribute, IAttributeValue
	{
		/// <summary>A prefix put in a .trx-file before each description value.</summary>
		public const string Prefix = DescriptionAttributeFix.Prefix;
		/// <summary>A postfix put in a .trx-file after each description value.</summary>
		public const string Postfix = DescriptionAttributeFix.Postfix;

		/// <inheritdoc />
		public TestDescriptionAttribute(string value) => Value = value;
		/// <summary>The value.</summary>
		public string Value { get; }
	}
}