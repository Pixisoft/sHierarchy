#if UNITY_EDITOR
/**
 * Copyright (c) Pixisoft Corporations. All rights reserved.
 * 
 * Licensed under the Source EULA. See COPYING in the asset root for license informtaion.
 */
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;

namespace sHierarchy
{
    [InitializeOnLoad]
    public static class HierarchyLog
    {
        /* Variables */

        private static Dictionary<string, string> LOG = new Dictionary<string, string>();
        private static Dictionary<string, string> WARN = new Dictionary<string, string>();
        private static Dictionary<string, string> ERROR = new Dictionary<string, string>();

        /* Setter & Getter */

        /* Functions */

        static HierarchyLog()
        {
            LOG.Clear();
            WARN.Clear();
            ERROR.Clear();

            Application.logMessageReceived += HandleLog;
        }

        public static Dictionary<string, string> LogStorage(LogType type)
        {
            switch (type)
            {
                case LogType.Log: return LOG;
                case LogType.Warning: return WARN;
                default: return ERROR;
            }
        }

        private static void HandleLog(string logString, string stackTrace, LogType type)
        {
            Dictionary<string, string> storage = LogStorage(type);
            if (storage == null)
                return;

            string component = GetComponentName(stackTrace, type);
            storage[component] = logString + "\n\n" + stackTrace;
        }

        #region Parse Stacktrace

        private static int GetLogIndex(LogType type, string[] lst)
        {
            int result;

            switch (type)
            {
                case LogType.Log:
                case LogType.Warning:
                case LogType.Error: result = 1; break;
                default: result = 0; break;
            }

            if (result < 0) result = 0;
            else if (result >= lst.Length) result = lst.Length - 1;

            return result;
        }

        private static int GetLogLengthIndex(LogType type, string[] lst)
        {
            switch (type)
            {
                case LogType.Log:
                case LogType.Warning:
                case LogType.Error: return lst.Length - 1;
                default: return lst.Length - 2;
            }
        }

        private static string GetComponentName(string stackTrace, LogType type)
        {
            var str = stackTrace.Replace("(at", "\n(at");

            var list = str.Split('\n');
            var start = list[GetLogIndex(type, list)];
            list = start.Split(':');

            var name = list[0];
            list = name.Split('.');

            name = list[GetLogLengthIndex(type, list)];
            return name;
        }

        #endregion
    }
}
#endif
