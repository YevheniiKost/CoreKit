using System.Collections.Generic;

namespace YeKostenko.CoreKit.Logging
{
    public static class Logger
    {
        private static readonly List<ILogger> Loggers = new List<ILogger>();
        
        private static bool s_showLogs = true;
        
        static Logger()
        {
            Loggers.Add(new UnityLogger());
        }
        
        public static void SetLogsVisibility(bool showLogs)
        {
            s_showLogs = showLogs;
        }
        
        public static void RegisterLogger(ILogger logger)
        {
            if (logger == null || Loggers.Contains(logger))
            {
                return;
            }
            
            Loggers.Add(logger);
        }
        
        public static void Log(string message)
        {
            if (!s_showLogs)
            {
                return;
            }
            
            foreach (var logger in Loggers)
            {
                logger.Log(message);
            }
        }
        
        public static void LogWarning(string message)
        {
            if (!s_showLogs)
            {
                return;
            }
            
            foreach (var logger in Loggers)
            {
                logger.LogWarning(message);
            }
        }
        
        public static void LogError(string message)
        {
            if (!s_showLogs)
            {
                return;
            }
            
            foreach (var logger in Loggers)
            {
                logger.LogError(message);
            }
        }
    }
}