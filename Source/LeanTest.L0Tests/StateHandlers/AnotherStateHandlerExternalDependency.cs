using System.Diagnostics.CodeAnalysis;
using System.Linq;
using LeanTest.Core.ExecutionHandling;
using LeanTest.L0Tests.ExternalDependencies;
using LeanTest.L0Tests.TestData;
using LeanTest.Mock;

namespace LeanTest.L0Tests.StateHandlers
{
	[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global", Justification = "Instantiated by the IoC container.")]
	internal class AnotherStateHandlerExternalDependency : TypedData<DataWithTwoStateHandlers>, IExternalDependency, 
		IStateHandler<DataWithTwoStateHandlers>
	{
		internal DataWithTwoStateHandlers DataWithTwoStateHandlers => Data.First();
	}
}