using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Miio.Devices.Logic
{
    public class JsonSerializer : ISerializer
    {
        public string Serialize(object toSerialize)
        {
            var serializerSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };
            return JsonConvert.SerializeObject(toSerialize, serializerSettings);
        }
    }
}
