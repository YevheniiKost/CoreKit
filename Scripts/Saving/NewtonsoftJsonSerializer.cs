using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace YeKostenko.CoreKit.Scripts.Saving
{
    public class NewtonsoftJsonSerializer : ISaveSerializer
    {
        private static readonly JsonSerializerSettings SerializerSettings = CreateSettings();

        public string Serialize<T>(T obj)
        {
            return JsonConvert.SerializeObject(obj, SerializerSettings);
        }

        public T Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json, SerializerSettings);
        }

        public object Deserialize(string json, Type type)
        {
            return JsonConvert.DeserializeObject(json, type, SerializerSettings);
        }

        private static JsonSerializerSettings CreateSettings()
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
            settings.ContractResolver = new DefaultContractResolver();
            settings.NullValueHandling = NullValueHandling.Include;
            return settings;
        }
    }
}