using System;

namespace LeanTest.MSTest
{
    /// <summary>The test tag attribute.</summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class TestTagAttribute : Attribute
    {
        public const string Prefix = @"TestTag = ###---";
        public const string Postfix = @"---###";
        public const string NotImplemented = @"LeanTest.NotImplemented";

        /// <inheritdoc />
        public TestTagAttribute(string value) => Value = value;
        /// <summary>The value.</summary>
        public string Value { get; }
    }
}