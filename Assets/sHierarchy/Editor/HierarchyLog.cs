#if UNITY_EDITOR
/**
 * Copyright (c) 2021 Jen-Chieh Shen
 * 
 * Permission is hereby granted, free of charge, to any person obtaining a copy of this software/algorithm and associated
 * documentation files (the "Software"), to use, copy, modify, merge or distribute copies of the Software, and to permit
 * persons to whom the Software is furnished to do so, subject to the following conditions:
 * 
 * - The Software, substantial portions, or any modified version be kept free of charge and cannot be sold commercially.
 * 
 * - The above copyright and this permission notice shall be included in all copies, substantial portions or modified
 * versions of the Software.
 * 
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE
 * WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR
 * COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
 * OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
 * 
 * For any other use, please ask for permission by contacting the author.
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
                case LogType.Error: return ERROR;
            }
            return null;
        }

        private static void HandleLog(string logString, string stackTrace, LogType type)
        {
            Dictionary<string, string> storage = LogStorage(type);
            if (storage == null)
                return;

            string component = GetComponentName(stackTrace);
            storage[component] = stackTrace;
        }

        private static string GetComponentName(string stackTrace)
        {
            var list = stackTrace.Split('\n');
            var start = list[1];
            list = start.Split(':');

            var name = list[0];
            list = name.Split('.');

            name = list[list.Length - 1];

            return name;
        }
    }
}
#endif
