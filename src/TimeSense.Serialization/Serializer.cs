using System;
using Newtonsoft.Json;

namespace TimeSense.Serialization
{
    public class Serializer : ISerializer
    {
        public string Serialize<T>(T input) where T : class
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }
            
            return JsonConvert.SerializeObject(input);
        }

        public T Deserialize<T>(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                throw new ArgumentNullException(nameof(input));
            }

            return JsonConvert.DeserializeObject<T>(input);
        }
    }
}
