using System;

namespace LeanTest.Xunit
{
	/// <summary>This is the implementation of a trick which allows xUnit to simulate MsTest's AssemblyInitialize attribute.
	/// See bradwilson's example code here: https://github.com/xunit/samples.xunit/tree/main/AssemblyFixtureExample.</summary>
	[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true)]
	public class AssemblyFixtureAttribute : System.Attribute
	{
		/// <summary>ctor</summary>
		/// <param name="fixtureType"></param>
		public AssemblyFixtureAttribute(Type fixtureType) => FixtureType = fixtureType;
		/// <summary></summary>
		public Type FixtureType { get; }
	}
}