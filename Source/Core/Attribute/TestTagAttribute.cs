using System;

namespace LeanTest.Attribute
{
    /// <summary>The test tag attribute.</summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class TestTagAttribute : System.Attribute, IAttributeValue
    {
        /// <summary>A prefix put in a .trx-file before each tag value.</summary>
        public const string Prefix = @"TestTag = ###---";
        /// <summary>A postfix put in a .trx-file after each tag value.</summary>
        public const string Postfix = @"---###";
        /// <summary>Put in a .trx-file for each 'not implemented' tag value.</summary>
        public const string NotImplemented = @"LeanTest.NotImplemented";

        /// <inheritdoc />
        public TestTagAttribute(string value) => Value = value;
        /// <summary>The value.</summary>
        public string Value { get; }
    }
}