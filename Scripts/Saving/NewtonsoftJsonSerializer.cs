using System;
using Newtonsoft.Json;

namespace YeKostenko.CoreKit.Scripts.Saving
{
    public class NewtonsoftJsonSerializer : ISaveSerializer
    {
        public string Serialize<T>(T obj) => JsonConvert.SerializeObject(obj);
        public T Deserialize<T>(string json) => JsonConvert.DeserializeObject<T>(json);
        public object Deserialize(string json, Type type) => JsonConvert.DeserializeObject(json, type);
    }
}