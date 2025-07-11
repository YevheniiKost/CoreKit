using UnityEngine;

namespace YeKostenko.CoreKit.Logging
{
    [AddComponentMenu("YellowTape/Utilities/Logger/ComponentLogger"), DisallowMultipleComponent]
    public class ComponentLogger : MonoBehaviour, ILogger
    {
        [Header("Settings")] 
        [SerializeField] private bool _showLogs = true;
        [SerializeField] private string _prefix;
        [SerializeField] private Color _prefixColor = Color.white;

        public void Log(string message)
        {
            if (_showLogs)
                Debug.Log($"<color=#{GetColor()}>{_prefix}</color> {message}");
        }

        public void LogWarning(string message)
        {
            if (_showLogs)
                Debug.LogWarning($"<color=#{GetColor()}>{_prefix}</color> {message}");
        }

        public void LogError(string message)
        {
            if (_showLogs)
                Debug.LogError($"<color=#{GetColor()}>{_prefix}</color> {message}");
        }
        
        private string GetColor()
        {
            return ColorUtility.ToHtmlStringRGB(_prefixColor);
        }
    }
}