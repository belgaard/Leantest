using System;
using System.Diagnostics.CodeAnalysis;
using LeanTest.Core.ExecutionHandling;
using LeanTest.L0Tests.TestData;
using LeanTest.Mock;

namespace LeanTest.L0Tests.StateHandlers
{
	[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "Instantiated by the IoC container.")]
	internal class StateHandlerWhichThrowsInBuild : TypedData<DataForStateHandlerWhichThrowsInBuild>,
		IStateHandler<DataForStateHandlerWhichThrowsInBuild>
	{
		public override void Build(Type type) => throw new Exception();
	}
}