using LeanTest.Core.ExecutionHandling;

namespace LeanTest.Mock
{
    /// <summary>
    /// Instantiate and initialize a mocking builder to handle IMockForData-based mocks
    /// </summary>
    public static class MockingBuilderFactory
    {
        /// <summary>
        /// Call this in order to allow the mocking builder to kick in and handle IMockForData-based mocks.
        /// </summary>
        public static void Initialize()
        {
            ContextBuilderFactory.AddBuilderFactory((container, dataStore) => new MockingBuilder(container, dataStore));
        }
    }
}