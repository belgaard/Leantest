using System.Collections.Generic;
using Newtonsoft.Json;

namespace LeanTest.JSon
{
    /// <summary>
    /// Utility for creating test data of type <c>T</c> from JSon.
    /// </summary>
    public class JsonDataCreatorForTest<T>
    {
        /// <summary>
        /// Deserialize a single instance of type <c>T</c>.
        /// </summary>
        public T DeserializeFromJson(string jsonResource)
        {
            return JsonConvert.DeserializeObject<T>(jsonResource);
        }

        /// <summary>
        /// Interprets a string that represents an array of Json documents of type <c>T</c>.
        /// </summary>
        public IEnumerable<T> DeserializeCollectionFromJsonArray(string jsonResource)
        {
            return JsonConvert.DeserializeObject<IEnumerable<T>>(jsonResource);
        }
    }
}
