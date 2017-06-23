using System;
using ExampleApp.Domain;
using LeanTest.Core.ExecutionHandling;

namespace State.Examples.MsTest.StateHandlers
{
	public class MyStateHandler : IStateHandler<MyData>, IStateHandler<MyOtherData>
	{
		public void PreBuild()
		{
			// TODO: Pre-build (not on individual IStateHandler): Turn off constraints, delete all records from all (relevant) tables?
		}

		public void WithData(MyData data)
		{
		}

		public void WithData(MyOtherData data)
		{
		}

		public void Build(Type type)
		{
		}

		public void PostBuild()
		{
			// TODO: Post-build (not on individual IStateHandler): Turn on constraints?
		}
	}
}