using System.Collections.Generic;

using Newtonsoft.Json;

namespace YeKostenko.CoreKit.Scripts.Saving
{
    [JsonObject(MemberSerialization.Fields)]
    public class GameSaveData
    {
        public string Version = "1.0.0";
        public long SavedAtUtc;
        public SaveGameMetadata Metadata = new SaveGameMetadata();
        public Dictionary<string, string> Data = new Dictionary<string, string>();
        public Dictionary<string, long> Timestamps = new Dictionary<string, long>();
    }
}
