using System.Diagnostics.CodeAnalysis;
using LeanTest.L0Tests.ExternalDependencies;
using LeanTest.L0Tests.TestData;
using LeanTest.Mock;

namespace LeanTest.L0Tests.Mocks
{
	[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global",
		Justification = "Instantiated by the IoC container.")]
	internal class MockForDataExternalDependency : TypedData<RegisteredData>, IExternalDependency, IMockForData<RegisteredData>
	{

	}
}