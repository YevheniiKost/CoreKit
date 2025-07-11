using System.IO;
using UnityEngine;

namespace YeKostenko.CoreKit.Logging
{
    public class FileLogger : ILogger
    {
        private readonly string _filePath = Application.dataPath + "/game_log.txt";

        public void Log(string message)
        {
            SaveLog("[Log]" + TimeStamp + message);
        }

        public void LogWarning(string message)
        {
            SaveLog("[Warning]" + TimeStamp + message);
        }

        public void LogError(string message)
        {
            SaveLog("[Error]" + TimeStamp + message);
        }

        private void SaveLog(string log)
        {
            using StreamWriter writer = new StreamWriter(_filePath, true);
            writer.WriteLine(log);
        }

        private string TimeStamp => 
            System.DateTime.Now.ToString("[yyyy-MM-dd HH:mm:ss] ");
    }
}