using System;
using System.Threading.Tasks;
using LeanTest.Core.ExecutionHandling;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace LeanTest.MSTest
{
	/// <summary>The MS Test adaption to <c>ExceptionAssertTException</c>.</summary>
	public static class ExceptionAssert
	{
		/// <summary>Throws an <c>AssertFailedException</c> exception unless <c>func</c> throws a <c>TException</c>.</summary>
		public static TException Throws<TException>(Func<Task> func, string message = "") where TException : Exception =>
			ExceptionAssertTException.Adapter(func, message, ExceptionAssertTException.Throws<TException>, m => new AssertFailedException(m));
		/// <summary>Throws an <c>AssertFailedException</c> exception unless <c>action</c> throws a <c>TException</c>.</summary>
		public static TException Throws<TException>(Action action, string message = "") where TException : Exception =>
			ExceptionAssertTException.Adapter(action, message, ExceptionAssertTException.Throws<TException>, m => new AssertFailedException(m));
	}
}