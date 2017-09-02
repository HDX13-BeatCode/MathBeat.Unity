using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UE = UnityEngine;


namespace MathBeat.Core
{
    public class Log : UE.MonoBehaviour
    {
        const string LOG_FORMAT = "{2}: {0}\n{1}/**************/\n";
        string logPath;

        private void Awake()
        {
            logPath = UE.Application.persistentDataPath + "/log_" + DateTime.Now.ToString("yyyy_MM_dd") + ".txt";
            if (!File.Exists(logPath))
            {
                File.WriteAllText(logPath, string.Empty);
            }

#if DEVELOPMENT_BUILD || UNITY_EDITOR
            Debug(ToString() + " has awaken at " + DateTime.Now);
#endif

        }

        /// <summary>
        /// Creates a log message (debug/dev build only)
        /// </summary>
        /// <param name="log"></param>
        public static void Debug(string log)
        {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            UE.Debug.Log(log);
#endif
        }

        /// <summary>
        /// Creates a log message with format (debug/dev build only)
        /// </summary>
        /// <param name="log"></param>
        public static void Debug(string format, params object[] args)
        {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
            UE.Debug.LogFormat(format, args);
#endif
        }

        /// <summary>
        /// Creates a log message
        /// </summary>
        /// <param name="log"></param>
        public static void Info(string log)
        {
            UE.Debug.Log(log);
        }

        /// <summary>
        /// Creates a log message with format
        /// </summary>
        /// <param name="log"></param>
        public static void Info(string format, params object[] args)
        {
            UE.Debug.LogFormat(format, args);
        }

        /// <summary>
        /// Creates a warning message
        /// </summary>
        /// <param name="log"></param>
        public static void Warn(string log)
        {
            UE.Debug.LogWarning(log);
        }

        /// <summary>
        /// Creates a warning message with format
        /// </summary>
        /// <param name="log"></param>
        public static void Warn(string log, params object[] args)
        {
            UE.Debug.LogWarningFormat(log, args);
        }

        /// Creates an error message
        /// </summary>
        /// <param name="log"></param>
        public static void Error(string log)
        {
            UE.Debug.LogError(log);
        }

        /// <summary>
        /// Creates an error message with format
        /// </summary>
        /// <param name="log"></param>
        public static void Error(string format, params object[] args)
        {
            UE.Debug.LogErrorFormat(format, args);
        }

        /// <summary>
        /// Creates an <see cref="System.Exception"/> message
        /// </summary>
        /// <param name="log"></param>
        public static void Exception(Exception exception)
        {
            UE.Debug.LogException(exception);
        }

        /// <summary>
        /// Creates an <see cref="System.Exception"/> message from the context <see cref="UE.Object"/>
        /// </summary>
        /// <param name="log"></param>
        public static void Exception(Exception exception, UE.Object context)
        {
            UE.Debug.LogException(exception, context);
        }

        private void OnEnable()
        {
            UE.Application.logMessageReceived += WriteLog;
        }

        private void OnDisable()
        {
            UE.Application.logMessageReceived -= WriteLog;
        }

        private void WriteLog(string condition, string stackTrace, UE.LogType type)
        {
            string logText = string.Format(LOG_FORMAT, condition, stackTrace, type.ToString());
            try
            {
                File.AppendAllText(logPath, logText);
            }
            catch (Exception e)
            {
                Exception(e);
            }
            
        }
    }
}
