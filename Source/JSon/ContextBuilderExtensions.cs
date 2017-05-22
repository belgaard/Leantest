using System.Collections.Generic;
using LeanTest.Core.ExecutionHandling;

namespace JSon
{
    /// <summary>
    /// Adds support for adding test data in Json format.
    /// </summary>
    public static class ContextBuilderExtensions
    {
        /// <summary>
        /// Declare data of type <c>T</c> to be stored, then used to fill in 'mocks' and 'state' during <c>Build</c>.
        /// </summary>
        public static ContextBuilder WithData<T>(this ContextBuilder contextBuilder, string json)
        {
            var creator = new JsonDataCreatorForTest<T>();
            contextBuilder.WithData(creator.DeserializeFromJson(json));

            return contextBuilder;
        }
        /// <summary>
        /// Declare an enumeration of data of type <c>T</c> to be stored, then used to fill in 'mocks' and 'state' during <c>Build</c>.
        /// </summary>
        public static ContextBuilder WithEnumerableData<T>(this ContextBuilder contextBuilder, string json)
        {
            var creator = new JsonDataCreatorForTest<T>();
            IEnumerable<T> enumerable = creator.DeserializeCollectionFromJsonArray(json);
            contextBuilder.WithEnumerableData(enumerable);

            return contextBuilder;
        }
    }
}