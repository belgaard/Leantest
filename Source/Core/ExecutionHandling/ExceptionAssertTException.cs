using System;
using System.Threading.Tasks;

namespace LeanTest.Core.ExecutionHandling
{
    /// <summary>
    /// Exception helper methods for tests.
    /// </summary>
    public static class ExceptionAssertTException
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TException"></typeparam>
        /// <param name="func"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static TException Throws<TException>(Func<Task> func, string message = "")
            where TException : Exception
        {
            return Throws<TException>(() =>
            {
                var action = new Task(() => func());
                action.RunSynchronously();
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

        /// <exception cref="AggregatedMessagesException">Thrown when the action threw and exception. </exception>
        public static void DoesNotThrow(Action action, string message = "")
        {
            try
            {
                action.Invoke();
            }
            catch (Exception ex)
            {
                throw new AggregatedMessagesException("ExceptionAssert.DoesNotThrow failed. " /*+ action.Method + " threw unexpected exception: "*/ + ex.GetType() + ". " + message, ex);
            }
        }
    }
}