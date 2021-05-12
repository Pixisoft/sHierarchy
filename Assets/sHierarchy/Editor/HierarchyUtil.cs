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
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace sHierarchy
{
    public delegate void EmptyFunction();

    public static class HierarchyUtil
    {
        /* Variables */

        private const string NAME = "sHierarchy";

        /* Setter & Getters */

        /* Functions */

        public static void IgnoreErrors(EmptyFunction func)
        {
            try
            {
                func.Invoke();
            }
            catch (Exception)
            {
                // ..
            }
        }

        public static string FormKey(string name)
        {
            return NAME + "." + name;
        }

        public static void CreateGroup(EmptyFunction func, bool flexibleSpace = false)
        {
            BeginHorizontal(() =>
            {
                BeginVertical(() =>
                {
                    Indent(func);
                });
            },
            flexibleSpace);
        }

        public static void BeginHorizontal(EmptyFunction func, bool flexibleSpace = false)
        {
            GUILayout.BeginHorizontal();
            if (flexibleSpace) GUILayout.FlexibleSpace();
            func.Invoke();
            GUILayout.EndHorizontal();
        }

        public static void BeginVertical(EmptyFunction func)
        {
            GUILayout.BeginVertical("box");
            func.Invoke();
            GUILayout.EndVertical();
        }

        public static void Indent(EmptyFunction func)
        {
            EditorGUI.indentLevel++;
            func.Invoke();
            EditorGUI.indentLevel--;
        }

        #region Color
        public static Color GetColor(string key, Color defaultValue)
        {
            Color color = defaultValue;
            color.r = EditorPrefs.GetFloat(key + ".r", defaultValue.r);
            color.g = EditorPrefs.GetFloat(key + ".g", defaultValue.g);
            color.b = EditorPrefs.GetFloat(key + ".b", defaultValue.b);
            color.a = EditorPrefs.GetFloat(key + ".a", defaultValue.a);
            return color;
        }

        public static void SetColor(string key, Color value)
        {
            EditorPrefs.SetFloat(key + ".r", value.r);
            EditorPrefs.SetFloat(key + ".g", value.g);
            EditorPrefs.SetFloat(key + ".b", value.b);
            EditorPrefs.SetFloat(key + ".a", value.a);
        }
        #endregion

        #region Vector3
        public static Vector3 GetVector3(string key, Vector3 defaultValue)
        {
            Vector3 vec = defaultValue;
            vec.x = EditorPrefs.GetFloat(key + ".x", defaultValue.x);
            vec.y = EditorPrefs.GetFloat(key + ".y", defaultValue.y);
            vec.z = EditorPrefs.GetFloat(key + ".z", defaultValue.z);
            return vec;
        }

        public static void SetVector3(string key, Vector3 vec)
        {
            EditorPrefs.SetFloat(key + ".x", vec.x);
            EditorPrefs.SetFloat(key + ".y", vec.y);
            EditorPrefs.SetFloat(key + ".z", vec.z);
        }
        #endregion

        /// <summary>
        /// Return true if focused window with the name as NAME.
        /// </summary>
        public static bool IsFocusedWindow(string name)
        {
            var win = EditorWindow.focusedWindow;
            if (win == null) return false;
            return win.titleContent.text == name;
        }

        public static void Button(string text, EmptyFunction func)
        {
            if (GUILayout.Button(text, GUILayout.Width(50)))
                func.Invoke();
        }

        private static float InstanceIDLength(int instanceID)
        {
            float len = 0.0f;
            string fullStr = instanceID.ToString();
            len = GUI.skin.label.CalcSize(new GUIContent(fullStr)).x;
            return len;
        }

        public static float MaxInstanceIDLength(int[] instanceIDs)
        {
            if (!HierarchyData.instance.instanceID.enabled)
                return 0.0f;

            float len = 0.0f;

            foreach (int instanceID in instanceIDs)
            {
                len = InstanceIDLength(instanceID);
            }

            return len;
        }

        private static float TagLength(string tag)
        {
            float len = 0.0f;
            len = GUI.skin.label.CalcSize(new GUIContent(tag)).x;
            return len;
        }

        public static float MaxTagLength(string[] tags)
        {
            if (!HierarchyData.instance.tag.enabled)
                return 0.0f;

            float len = 0.0f;

            foreach (string tag in tags)
            {
                len = TagLength(tag.Trim());
            }

            return len;
        }

        private static Texture ScriptTexture(Type t)
        {
            Texture tex = null;
            if (t.IsSubclassOf(typeof(MonoBehaviour)))
                tex = EditorGUIUtility.IconContent("cs Script Icon").image;
            else if (t.IsSubclassOf(typeof(ScriptableObject)))
                tex = EditorGUIUtility.IconContent("d_ScriptableObject Icon").image;
            return tex;
        }

        public static Texture TypeTexture(Type t)
        {
            var image = EditorGUIUtility.ObjectContent(null, t).image;
            if (image == null) image = ScriptTexture(t);
            // default icon
            if (image == null) image = EditorGUIUtility.IconContent("d__Help@2x").image;
            return image;
        }

        public static void ExpandComponents(GameObject go, bool act)
        {
            Selection.activeGameObject = go;

            foreach (var comp in go.GetComponents<Component>())
                InternalEditorUtility.SetIsInspectorExpanded(comp, act);
            ActiveEditorTracker.sharedTracker.ForceRebuild();
        }

        public static void ExpandComponents(int instanceID, bool act)
        {
            GameObject go = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            ExpandComponents(go, act);
        }

        private static void FocusComponent(GameObject go, Type current)
        {
            Selection.activeGameObject = go;

            foreach (var comp in go.GetComponents<Component>())
                InternalEditorUtility.SetIsInspectorExpanded(comp, (comp.GetType() == current));
            ActiveEditorTracker.sharedTracker.ForceRebuild();
        }

        public static void FocusComponent(int instanceID, Type current)
        {
            GameObject go = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            FocusComponent(go, current);
        }

        public static bool IsExpanded(GameObject go, Type current)
        {
            return InternalEditorUtility.GetIsInspectorExpanded(go.GetComponent(current));
        }

        public static bool IsExpanded(int instanceID, Type current)
        {
            GameObject go = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            return IsExpanded(go, current);
        }
    }
}
#endif
