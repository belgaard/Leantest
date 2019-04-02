using System;
using System.Linq;
using System.Reflection;
using LeanTest.Core.ExecutionHandling;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LeanTest.MSTest
{
	/// <summary>
	/// Adds support for adding test data in Json format.
	/// </summary>
	public static class ContextBuilderExtensions
	{
		/// <summary>Registers an intend to use the <c>TestScenarioId</c> attribute on test methods.</summary>
		/// <remarks>This causes scenario IDs to be written to the test log (.trx-file).</remarks>
		public static ContextBuilder RegisterScenarioId(this ContextBuilder theContextBuilder, TestContext testContext)
		{
			if (testContext == null) throw new ArgumentNullException(nameof(testContext));

			testContext.RegisterScenarioId();
			return theContextBuilder;
		}

		/// <summary>Registers an intend to use the LeanTest attribute on test methods.</summary>
		/// <remarks>This causes scenario IDs and tags to be written to the test log (.trx-file).</remarks>
		public static ContextBuilder RegisterTags(this ContextBuilder theContextBuilder, TestContext testContext)
		{
			if (testContext == null) throw new ArgumentNullException(nameof(testContext));

			testContext.RegisterTags();
			return theContextBuilder;
		}

		/// <summary>Registers an intend to use the LeanTest attribute on test methods.</summary>
		/// <remarks>This causes scenario IDs and tags to be written to the test log (.trx-file).</remarks>
		// TODO: Use the builder pattern - defer writing until build!?
		public static ContextBuilder RegisterAttributes(this ContextBuilder theContextBuilder, TestContext testContext)
		{
			if (testContext == null) throw new ArgumentNullException(nameof(testContext));

			testContext.RegisterAttributes();
			return theContextBuilder;
		}
	}
}