using System.Collections.Generic;

namespace YeKostenko.CoreKit.Scripts.Saving
{
    public class GameSaveData
    {
        public string Version = "1.0.0";
        public Dictionary<string, string> Data = new();

        public Dictionary<string, long> Timestamps = new();
    }
}