using UnityEngine;

namespace JackSParrot.Utils
{
    [CreateAssetMenu(fileName = "New UnityLogger", menuName = "JackSParrot/Services/UnityLogger", order = 1)]
    public class UnityLoggerServiceSO : CustomLoggerServiceSO
    {
        [SerializeField] private LogLevel _defaultLevel = LogLevel.Debug;
        private LogLevel _logLevel = LogLevel.Debug;

        public static UnityLoggerServiceSO CreateInstance(LogLevel level = LogLevel.Debug)
        {
            UnityLoggerServiceSO retVal = CreateInstance<UnityLoggerServiceSO>();
            retVal._logLevel = level;
            return retVal;
        }

        public override void SetLogLevel(LogLevel level)
        {
            _logLevel = level;
        }

        public override void LogDebug(string message)
        {
            if(_logLevel != LogLevel.None)
            {
                Debug.Log(message);
            }
        }

        public override void LogError(string message)
        {
            if (_logLevel == LogLevel.Error)
            {
                Debug.LogError(message);
            }
        }

        public override void LogWarning(string message)
        {
            if (_logLevel == LogLevel.Warning || _logLevel == LogLevel.Error)
            {
                Debug.LogWarning(message);
            }
        }

        private void OnEnable()
        {
            _logLevel = _defaultLevel;
        }
    }
}