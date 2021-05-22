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

        private const string COMP_ICON_BROKEN = "Warning@2x";

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

        public static void CreateInfo(string desc)
        {
            if (desc == "")
                return;

            GUILayout.BeginHorizontal(EditorStyles.helpBox);
            {
                GUILayout.Label(desc);
            }
            GUILayout.EndHorizontal();
        }

        public static bool IsType<T, U>()
        {
            return (typeof(T) == typeof(U));
        }

        /// <summary>
        /// Convert from generic T to any data type U.
        /// 
        /// See https://stackoverflow.com/questions/4092393/value-of-type-t-cannot-be-converted-to
        /// </summary>
        public static U ConvertType<T, U>(T data)
        {
            T newT1 = (T)(object)data;
            U newT2 = (U)(object)newT1;
            return newT2;
        }

        #region Preference Data Utility

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

        public static Dictionary<K, V> GetDictionary<K, V>(string key, Dictionary<K, V> defaultValue)
        {
            int len = EditorPrefs.GetInt(key + ".length", 0);

            Dictionary<K, V> newList = new Dictionary<K, V>();

            for (int count = 0; count < len; ++count)
            {
                K entryKey = GetData(key + ".entry." + count, default(K));
                V entryVal = GetData(key + ".value." + count, default(V));
                newList.Add(entryKey, entryVal);
            }

            return newList;
        }

        public static void SetDictionary<K, V>(string key, Dictionary<K, V> value)
        {
            int len = value.Count;
            EditorPrefs.SetInt(key + ".length", len);

            int count = 0;
            foreach (KeyValuePair<K, V> entry in value)
            {
                SetData(key + ".entry." + count, entry.Key);
                SetData(key + ".value." + count, entry.Value);
                ++count;
            }
        }

        public static T GetData<T>(string key, T defaultValue) 
        {
            object result = null;

            if (IsType<T, int>())
            {
                int val = ConvertType<T, int>(defaultValue);
                result = EditorPrefs.GetInt(key, val);
            }
            else if (IsType<T, float>())
            {
                float val = ConvertType<T, float>(defaultValue);
                result = EditorPrefs.GetFloat(key, val);
            }
            else if (IsType<T, bool>())
            {
                bool val = ConvertType<T, bool>(defaultValue);
                result = EditorPrefs.GetBool(key, val);
            }
            else if (IsType<T, string>())
            {
                string val = ConvertType<T, string>(defaultValue);
                result = EditorPrefs.GetString(key, val);
            }
            else if (IsType<T, Color>())
            {
                Color val = ConvertType<T, Color>(defaultValue);
                result = GetColor(key, val);
            }
            else
            {
                Debug.LogError("Invalid data type, set: " + typeof(T) + " with default value of " + defaultValue);
            }

            return (T)Convert.ChangeType(result, typeof(T));
        }

        public static void SetData<T>(string key, T value)
        {
            if (IsType<T, int>())
            {
                int val = ConvertType<T, int>(value);
                EditorPrefs.SetInt(key, val);
            }
            else if (IsType<T, float>())
            {
                float val = ConvertType<T, float>(value);
                EditorPrefs.SetFloat(key, val);
            }
            else if (IsType<T, bool>())
            {
                bool val = ConvertType<T, bool>(value);
                EditorPrefs.SetBool(key, val);
            }
            else if (IsType<T, string>())
            {
                string val = ConvertType<T, string>(value);
                EditorPrefs.SetString(key, val);
            }
            else if (IsType<T, Color>())
            {
                Color val = ConvertType<T, Color>(value);
                SetColor(key, val);
            }
            else
            {
                Debug.LogError("Invalid data type, set: " + typeof(T) + " with value of " + value);
            }
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

        #region Default UI

        public static bool Button(string text)
        {
            return GUILayout.Button(text, GUILayout.Width(60));
        }

        public static bool Button(string text, EmptyFunction func)
        {
            if (Button(text))
            {
                func.Invoke();
                return true;
            }
            return false;
        }

        public static void LabelField(string text)
        {
            EditorGUILayout.LabelField(text, EditorStyles.boldLabel);
        }

        #endregion

        private static float InstanceIDLength(int instanceID)
        {
            string fullStr = instanceID.ToString();
            return GUI.skin.label.CalcSize(new GUIContent(fullStr)).x;
        }

        public static float MaxIntLength(int[] instanceIDs, bool flag)
        {
            if (!flag) return 0.0f;

            float len = 0.0f;

            foreach (int instanceID in instanceIDs)
            {
                len = Mathf.Max(InstanceIDLength(instanceID), len);
            }

            return len;
        }

        private static float LabelLength(string label)
        {
            return GUI.skin.label.CalcSize(new GUIContent(label)).x;
        }

        public static float MaxLabelLength(string[] labels, bool flag)
        {
            if (!flag) return 0.0f;

            float len = 0.0f;

            foreach (string label in labels)
            {
                len = Mathf.Max(LabelLength(label.Trim()), len);
            }

            return len;
        }

        public static Texture BrokenIcon()
        {
            return EditorGUIUtility.IconContent(COMP_ICON_BROKEN).image;
        }

        public static Texture TypeTexture(Component comp, Type t)
        {
            var image = EditorGUIUtility.ObjectContent(comp, t).image;
            // icon for broken link
            if (image == null) image = BrokenIcon();
            return image;
        }

        #region Expand Components in Inspector

        public static void ExpandComponents(GameObject go, bool act)
        {
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

        #endregion

        public static void DrawTextureTooltip(Rect rect, Texture tex, string tooltip)
        {
            /* Recalculate the height base on constant width, 16 px. */
            {
                float width = HierarchyDrawer.ROW_HEIGHT;
                rect.width = width;
                rect.height = tex.height * width / tex.width;
            }
            /* shift y to center */
            {
                float height = HierarchyDrawer.ROW_HEIGHT;
                float texCenter = rect.height / 2.0f;
                float yDiff = (height / 2.0f) - texCenter;
                rect.y += yDiff;
            }
            GUI.DrawTexture(rect, tex);
            GUI.Label(rect, new GUIContent("", null, tooltip));  // add tooltip
        }

        public static void DrawTextureTooltip(Rect rect, Texture tex, string tooltip, float alpha)
        {
            Color prevColor = GUI.color;
            GUI.color = new Color(prevColor.r, prevColor.g, prevColor.b, alpha);
            DrawTextureTooltip(rect, tex, tooltip);
            GUI.color = prevColor;
        }

        #region Tooltip

        private static GUIContent CreateGUIContent(string name, string tooltip = "")
        {
            var gc = new GUIContent(name);
            if (tooltip != "") gc.tooltip = tooltip;
            return gc;
        }

        public static bool Toggle(string name, bool val, string tooltip = "", float width = -1.0f)
        {
            float originalValue = EditorGUIUtility.labelWidth;
            if (width != -1.0f) 
                EditorGUIUtility.labelWidth = width;
            bool result = EditorGUILayout.Toggle(CreateGUIContent(name, tooltip), val);
            EditorGUIUtility.labelWidth = originalValue;
            return result;
        }

        public static float Slider(string name, float val, float leftValue, float rightValue, string tooltip = "")
        {
            return EditorGUILayout.Slider(CreateGUIContent(name, tooltip), val, leftValue, rightValue);
        }

        public static Enum EnumPopup(string name, Enum val, string tooltip = "")
        {
            return EditorGUILayout.EnumPopup(CreateGUIContent(name, tooltip), val);
        }

        public static string TextField(string name, string val, string tooltip = "")
        {
            return EditorGUILayout.TextField(CreateGUIContent(name, tooltip), val);
        }

        public static Color ColorField(string name, Color val, string tooltip = "")
        {
            return EditorGUILayout.ColorField(CreateGUIContent(name, tooltip), val);
        }

        public static int Popup(int selectedIndex, string[] displayedOptions)
        {
            return EditorGUILayout.Popup(selectedIndex, displayedOptions, GUILayout.Width(175));
        }

        #endregion
    }
}
#endif
