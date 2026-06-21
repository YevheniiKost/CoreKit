using Newtonsoft.Json;

namespace YeKostenko.CoreKit.Scripts.Saving
{
    [JsonObject(MemberSerialization.Fields)]
    public class SaveGameMetadata
    {
        public string LocationName = string.Empty;
        public float PlayTimeSeconds;
    }
}
