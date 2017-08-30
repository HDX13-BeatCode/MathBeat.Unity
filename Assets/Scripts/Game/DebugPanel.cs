using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System.Linq;
using System.IO;
using System;

namespace MathBeat.Game
{
    public class DebugPanel : MonoBehaviour
    {
        public Text DebugText;

        private void Start()
        {
            // clear the log text
            DebugText.text = string.Empty;
        }

        void OnEnable()
        {
            Application.logMessageReceived += LogMessage;
        }

        void OnDisable()
        {
            Application.logMessageReceived -= LogMessage;
        }

        public void LogMessage(string message, string stackTrace, LogType type)
        {
#if !UNITY_EDITOR
            if (type == LogType.Error || type == LogType.Exception)
            {
#endif
                if (DebugText.text.Length > 100)
                {
                    DebugText.text = string.Empty;
                }
                
                DebugText.text += message + "\n"; 
            }
#if !UNITY_EDITOR
        }
#endif
    }
}