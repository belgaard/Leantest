namespace LeanTest.Core.ExecutionHandling
{
	/// <summary>Implementations create a context builder.</summary>
	public interface ICreateContextBuilder
	{
		/// <summary>Create a context builder, and thus the IoC container.</summary>
		ContextBuilder CreateContextBuilder { get; }
	}
}