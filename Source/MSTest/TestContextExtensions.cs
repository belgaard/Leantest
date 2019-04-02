using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace LeanTest.MSTest
{
	/// <summary>
	/// Extension methods for MS Tests TestContext.
	/// </summary>
	public static class TestContextExtensions
	{
		/// <summary>Registers an intend to use the <c>TestScenarioId</c> attribute on test methods.</summary>
		/// <remarks>This causes scenario IDs to be written to the test log (.trx-file).</remarks>
		public static TestContext RegisterScenarioId(this TestContext testContext)
		{
			if (testContext == null) throw new ArgumentNullException(nameof(testContext));

			foreach (var testScenarioIdAttribute in GetAttributesForTestMethod<TestScenarioIdAttribute>(testContext))
			{
				Console.WriteLine($@"{TestScenarioIdAttribute.Prefix}{testScenarioIdAttribute?.Value}{TestScenarioIdAttribute.Postfix}");
			}

			return testContext;
		}

		/// <summary>Registers an intend to use the LeanTest attribute on test methods.</summary>
		/// <remarks>This causes scenario IDs and tags to be written to the test log (.trx-file).</remarks>
		public static TestContext RegisterTags(this TestContext testContext)
		{
			if (testContext == null) throw new ArgumentNullException(nameof(testContext));

			foreach (var testTagAttribute in GetAttributesForTestMethod<TestTagAttribute>(testContext))
			{
				Console.WriteLine($@"{TestTagAttribute.Prefix}{testTagAttribute?.Value}{TestTagAttribute.Postfix}");
			}

			return testContext;
		}

		/// <summary>Registers an intend to use the LeanTest attribute on test methods.</summary>
		/// <remarks>This causes scenario IDs and tags to be written to the test log (.trx-file).</remarks>
		public static TestContext RegisterAttributes(this TestContext testContext)
		{
			if (testContext == null) throw new ArgumentNullException(nameof(testContext));

			return testContext
				.RegisterScenarioId()
				.RegisterTags();
		}

		private static IEnumerable<TAttributeType> GetAttributesForTestMethod<TAttributeType>(TestContext testContext)
		{
			var testClassType = Type.GetType(testContext.FullyQualifiedTestClassName, throwOnError: true);
			var testMethod = testClassType.GetMethod(testContext.TestName) ?? throw new Exception($"Unable to find test method {testContext.TestName} on type {testContext.FullyQualifiedTestClassName}");
			return testMethod.GetCustomAttributes(typeof(TAttributeType), inherit: false).Cast<TAttributeType>();
		}
	}
}
