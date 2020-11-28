using Microsoft.Extensions.Hosting;
using System;
using System.Threading.Tasks;
using System.Timers;

namespace LeanTest
{
	/// <summary>
	/// Interface for a TestServiceRunner which should be able to run the hosted services under test
	/// </summary>
	public interface ITestServiceRunner : IDisposable
	{
		/// <summary>
		/// Starts the service
		/// </summary>
		void StartService();

		/// <summary>
		/// Starts the service
		/// </summary>
		Task StartServiceAsync();

		/// <summary>
		/// Starts the service
		/// </summary>
		/// <param name="miliseconds">The time the service should run, autostops after</param>
		/// <returns></returns>
		Task StartServiceAsync(int miliseconds);

		/// <summary>
		/// Stops the service
		/// </summary>
		void StopService();

		/// <summary>
		/// Stops the service
		/// </summary>
		/// <returns></returns>
		Task StopServiceAsync();
	}

	/// <summary>
	/// A Test wrapper around an IHost that can be used to start / stop the host
	/// </summary>
	public class TestServiceRunner : ITestServiceRunner
	{
		private static IHost _host;

		/// <summary>
		/// Create a new TestServiceRunner based on the provided host
		/// </summary>
		/// <param name="host"></param>
		public TestServiceRunner(IHost host)
		{
			_host = host;
		}

		/// <summary>
		/// Cleans up resources
		/// </summary>
		public void Dispose()
		{
			_host.Dispose();
		}

		/// <summary>
		/// Starts this TestSErviceRunner host, it runs untill the internal hosted services stop.
		/// 
		/// Be aware that hosted services have the tendency to run endlessly, 
		/// invoke a stop or provide a run duration if the hostedservices under test does not stop by themselves
		/// </summary>
		public void StartService()
		{
			_host.Start();
		}

		/// <summary>
		/// Starts this TestSErviceRunner host, it runs untill the internal hosted services stop.
		/// 
		/// Be aware that hosted services have the tendency to run endlessly, 
		/// invoke a stop or provide a run duration if the hostedservices under test does not stop by themselves
		/// </summary>
		/// <returns></returns>
		public Task StartServiceAsync()
		{
			return _host.StartAsync();
		}

		/// <summary>
		/// Starts this TestServiceRunner host, excecution time is limited to the specified milliseconds
		/// </summary>
		/// <param name="milliseconds"></param>
		/// <returns></returns>
		public Task StartServiceAsync(int milliseconds)
		{
			var timer = new Timer(milliseconds)
			{
				AutoReset = false,			
			};
			timer.Elapsed += (source, args) => { _host.StopAsync(); };
			var awaitable = _host.StartAsync();
			timer.Start();
			return awaitable;
		}

		/// <summary>
		/// Stops this TestServiceRunner host that is running the hosted services
		/// 
		/// Use: if your hosted service does not stop itself (running endlessly) you need to call this after your test
		/// </summary>
		public void StopService()
		{
			_host.StopAsync().Wait();
		}

		/// <summary>
		/// Stops this TestServiceRunner host that is running the hosted services
		/// 
		/// Use: if your hosted service does not stop itself (running endlessly) you need to call this after your test
		/// </summary>
		/// <returns></returns>
		public Task StopServiceAsync()
		{
			return _host.StopAsync();
		}
	}
}
