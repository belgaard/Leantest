namespace LeanTest.Attribute
{
	/// <summary>Used to send output to std out (which will differ across test frameworks).</summary>
	public interface IStdOut
	{
		/// <summary>Write a line to stdout.</summary>
		void WriteLine(string value);
	}
}