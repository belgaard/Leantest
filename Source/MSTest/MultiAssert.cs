using System;
using LeanTest.Core.ExecutionHandling;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LeanTest.MSTest
{
	/// <summary>The MS Test adaption to <c>MultiAssertForTException</c>.</summary>
	public static class MultiAssert
	{
		/// <summary>Executes all actions regardless of failed assertions (a.k.a. <c>AssertFailedException</c> thrown). If any assertions fail, a single 
		/// assertion failure with aggregated assertion message texts is generated.</summary>
		public static void Aggregate(params Action[] actions)
		{
			try
			{
				MultiAssertForTException.Aggregate<AssertFailedException>(actions);
			}
			// Turn aggregated messages into a proper MS Test assert failed exception - but let other exceptions, including 'inconclusive' fall through:
			catch (AggregatedMessagesException e)
			{
				throw new AssertFailedException(e.Message);
			}
		}
	}
}