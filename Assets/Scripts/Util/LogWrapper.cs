using UnityEngine;

namespace UnityGame
{
    public static class LogWrapper
    {
        public static void Log(string message, params object[] parameters)
        {
            if(parameters == null || parameters.Length == 0)
            {
                Debug.Log(message);
            }
            else
            {
                Debug.Log(string.Format(message, parameters));
            }
        }

        public static void LogError(string message, params object[] parameters)
        {
            if (parameters == null || parameters.Length == 0)
            {
                Debug.LogError(message);
            }
            else
            {
                Debug.LogError(string.Format(message, parameters));
            }
        }
    }
}
