using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace LeanTest.MSTest
{
	/// <summary>
	/// Extension methods for MS Tests TestContext.
	/// </summary>
	[Obsolete("Don't use!")]
	public static class TestContextExtensions
	{
		/// <summary>Registers an intend to use the <c>TestScenarioId</c> attribute on test methods.</summary>
		/// <param name="testContext"></param>
		/// <param name="assemblyContainingTest">Assembly containing the test for which to register scenario id for</param>
		/// <remarks>This causes scenario IDs to be written to the test log (.trx-file).</remarks>
		public static TestContext RegisterScenarioId(this TestContext testContext, Assembly assemblyContainingTest)
		{
			if (testContext == null) throw new ArgumentNullException(nameof(testContext));
			if (assemblyContainingTest == null) throw new ArgumentNullException(nameof(assemblyContainingTest));

			foreach (var testScenarioIdAttribute in GetAttributesForTestMethod<TestScenarioIdAttribute>(testContext, assemblyContainingTest))
			{
				Console.WriteLine($@"{TestScenarioIdAttribute.Prefix}{testScenarioIdAttribute?.Value}{TestScenarioIdAttribute.Postfix}");
			}

			return testContext;
		}

		/// <summary>Registers an intend to use the LeanTest attribute on test methods.</summary>
		/// <param name="testContext"></param>
		/// <param name="assemblyContainingTest">Assembly containing the test for which to register tags for</param>
		/// <remarks>This causes scenario IDs and tags to be written to the test log (.trx-file).</remarks>
		public static TestContext RegisterTags(this TestContext testContext, Assembly assemblyContainingTest)
		{
			if (testContext == null) throw new ArgumentNullException(nameof(testContext));
			if (assemblyContainingTest == null) throw new ArgumentNullException(nameof(assemblyContainingTest));

			foreach (var testTagAttribute in GetAttributesForTestMethod<TestTagAttribute>(testContext, assemblyContainingTest))
			{
				Console.WriteLine($@"{TestTagAttribute.Prefix}{testTagAttribute?.Value}{TestTagAttribute.Postfix}");
			}

			return testContext;
		}

		/// <summary>Registers an intend to use the LeanTest attribute on test methods.</summary>
		/// <param name="testContext"></param>
		/// <param name="assemblyContainingTest">Assembly containing the test for which to register attributes for. If not passed, GetCallingAssembly will be used. </param>
		/// <remarks>This causes scenario IDs and tags to be written to the test log (.trx-file).</remarks>
		[System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)]
		public static TestContext RegisterAttributes(this TestContext testContext, Assembly assemblyContainingTest = null)
		{
			if (testContext == null) throw new ArgumentNullException(nameof(testContext));

			// We assume the calling assembly is the assembly containing the test for which we need to register attributes.
			// For this to work we need to make sure JIT doesn't inline this method, so we use MethodImplOptions.NoInlining
			var assembly = assemblyContainingTest ?? Assembly.GetCallingAssembly();

			return testContext
				.RegisterScenarioId(assembly)
				.RegisterTags(assembly);
		}

		private static IEnumerable<TAttributeType> GetAttributesForTestMethod<TAttributeType>(TestContext testContext, Assembly assemblyContainingTest)
		{
			var testClassType = assemblyContainingTest.GetType(testContext.FullyQualifiedTestClassName) ?? throw new ArgumentNullException(nameof(assemblyContainingTest));
			
			var testMethod = testClassType.GetMethod(testContext.TestName) ?? throw new Exception($"Unable to find test method {testContext.TestName} on type {testContext.FullyQualifiedTestClassName}");
			return testMethod.GetCustomAttributes(typeof(TAttributeType), false).Cast<TAttributeType>();
		}
	}
}
