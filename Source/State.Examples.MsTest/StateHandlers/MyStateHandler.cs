using System;
using ExampleApp.Domain;
using LeanTest.Core.ExecutionHandling;

namespace State.Examples.MsTest.StateHandlers
{
	public class MyStateHandler : IStateHandler<MyData>
	{
		public void WithData(MyData data)
		{
            
		}

		// TODO: Pre-build (not on individual IStateHandler): Turn off constraints, delete all records from all (relevant) tables?

		public void Clear(Type type) // TODO: Needed?
		{
		}

		public void Build()
		{
			// TODO: Insert into relevant table.
		}

		// TODO: Post-build (not on individual IStateHandler): Turn on constraints?
	}
}