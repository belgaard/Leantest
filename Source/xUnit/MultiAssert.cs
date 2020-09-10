using System;
using LeanTest.Core.ExecutionHandling;
using Xunit.Sdk;

namespace LeanTest.Xunit
{
	/// <summary>The Xunit adaption to <c>MultiAssertForTException</c>.</summary>
	public static class MultiAssert
	{
		/// <summary>Executes all actions regardless of failed assertions (a.k.a. <c>AssertFailedException</c> thrown). If any assertions fail, a single 
		/// assertion failure with aggregated assertion message texts is generated.</summary>
		public static void Aggregate(params Action[] actions)
		{
			try
			{
				MultiAssertForTException.Aggregate<XunitException>(actions);
			}
			// Turn aggregated messages into a proper MS Test assert failed exception - but let other exceptions, including 'inconclusive' fall through:
			catch (AggregatedMessagesException e)
			{
				throw new XunitException(e.Message);
			}
		}
	}
}