using System;
using System.Threading.Tasks;

namespace LeanTest.Core.ExecutionHandling
{
	/// <summary>
	/// Exception helper methods for tests.
	/// </summary>
	public static class ExceptionAssertTException
	{
		/// <summary></summary>
		public static TException Throws<TException>(Func<Task> func, string message = "")
			where TException : Exception
		{
			return Throws<TException>(() =>
			{
				Task action = Task.WhenAll(func());
				if (!action.IsFaulted)
					return;
				if (action.Exception?.InnerException != null)
					throw action.Exception.InnerException;
			}, message);
		}

		/// <exception cref="AggregatedMessagesException">Thrown when the action did not throw the required exception.</exception>
		public static TException Throws<TException>(Action action, string message)
			where TException : Exception
		{
			Exception exception = null;

			try
			{
				action.Invoke();
			}
			catch (TException ex)
			{
				return ex;
			}
			catch (Exception ex)
			{
				exception = ex;
			}

			if (exception == null)
				throw new AggregatedMessagesException("ExceptionAssert.Throws failed. No exception was thrown. Expected: " + typeof(TException) + ". " + message);

			throw new AggregatedMessagesException("ExceptionAssert.Throws failed. Expected exception type: " + typeof(TException).Name + ". Thrown: " + exception.GetType() + ". " + message);
		}

		/// <exception cref="AggregatedMessagesException">Thrown when the action threw an exception. </exception>
		public static void DoesNotThrow(Action action, string message = "")
		{
			try
			{
				action.Invoke();
			}
			catch (Exception ex)
			{
				throw new AggregatedMessagesException("ExceptionAssert.DoesNotThrow failed. " + ex.GetType() + ". " + message, ex);
			}
		}

		/// <exception cref="AggregatedMessagesException">Thrown when the action threw an exception. </exception>
		public static async Task DoesNotThrowAsync(Func<Task> action, string message = "")
		{
			try
			{
#pragma warning disable ConfigureAwaitEnforcer // ConfigureAwaitEnforcer
				await action.Invoke();
#pragma warning restore ConfigureAwaitEnforcer // ConfigureAwaitEnforcer
			}
			catch (Exception ex)
			{
				throw new AggregatedMessagesException("ExceptionAssert.DoesNotThrow failed. " + ex.GetType() + ". " + message, ex);
			}
		}

		/// <summary></summary>
		public static void Adapter<TFunc>(TFunc action, string message, Action<TFunc, string> throws, Func<string, Exception> assertFailedException)
		{
			try { throws(action, message); }
			catch (AggregatedMessagesException e) { throw assertFailedException(e.Message); }
		}

		/// <summary></summary>
		public static TException Adapter<TException, TFunc>(TFunc action, string message, Func<TFunc, string, TException> throws, Func<string, Exception> assertFailedException)
			where TException : Exception
		{
			try { return throws(action, message); }
			catch (AggregatedMessagesException e) { throw assertFailedException(e.Message); }
		}
		/// <summary></summary>
		public static async Task AdapterAsync(Func<Task> action, string message, Func<Func<Task>, string, Task> throws, Func<string, Exception> assertFailedException)
		{
			try
			{
				var x = new Func<Func<Task>, string, Task>(DoesNotThrowAsync);
#pragma warning disable ConfigureAwaitEnforcer // ConfigureAwaitEnforcer
				await x(action, message);
#pragma warning restore ConfigureAwaitEnforcer // ConfigureAwaitEnforcer
			}
			catch (Exception e) { throw assertFailedException(e.Message); }
		}
	}
}