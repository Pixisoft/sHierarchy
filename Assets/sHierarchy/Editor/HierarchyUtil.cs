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
using UnityEngine;

namespace sHierarchy
{
    public delegate void EmptyFunction();

    public static class HierarchyUtil
    {
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
            return "sHierarchy." + name;
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

        #region Hierarchy
        // SOURCE: https://github.com/sandolkakos/unity-utilities/blob/main/Scripts/Editor/SceneHierarchyUtility.cs
        //
        // Thanks for `Marllon Vilano`!

        /// <summary>
        /// Check if the target GameObject is expanded (aka unfolded) in the Hierarchy view.
        /// </summary>
        public static bool IsExpanded(GameObject go)
        {
            return GetExpandedGameObjects().Contains(go);
        }

        /// <summary>
        /// Get a list of all GameObjects which are expanded (aka unfolded) in the Hierarchy view.
        /// </summary>
        public static List<GameObject> GetExpandedGameObjects()
        {
            object sceneHierarchy = GetSceneHierarchy();

            MethodInfo methodInfo = sceneHierarchy
                .GetType()
                .GetMethod("GetExpandedGameObjects");

            object result = methodInfo.Invoke(sceneHierarchy, new object[0]);

            return (List<GameObject>)result;
        }

        /// <summary>
        /// Set the target GameObject as expanded (aka unfolded) in the Hierarchy view.
        /// </summary>
        public static void SetExpanded(GameObject go, bool expand)
        {
            object sceneHierarchy = GetSceneHierarchy();

            MethodInfo methodInfo = sceneHierarchy
                .GetType()
                .GetMethod("ExpandTreeViewItem", BindingFlags.NonPublic | BindingFlags.Instance);

            methodInfo.Invoke(sceneHierarchy, new object[] { go.GetInstanceID(), expand });
        }

        /// <summary>
        /// Set the target GameObject and all children as expanded (aka unfolded) in the Hierarchy view.
        /// </summary>
        public static void SetExpandedRecursive(GameObject go, bool expand)
        {
            object sceneHierarchy = GetSceneHierarchy();

            MethodInfo methodInfo = sceneHierarchy
                .GetType()
                .GetMethod("SetExpandedRecursive", BindingFlags.Public | BindingFlags.Instance);

            methodInfo.Invoke(sceneHierarchy, new object[] { go.GetInstanceID(), expand });
        }

        private static object GetSceneHierarchy()
        {
            EditorWindow window = GetHierarchyWindow();

            object sceneHierarchy = typeof(EditorWindow).Assembly
                .GetType("UnityEditor.SceneHierarchyWindow")
                .GetProperty("sceneHierarchy")
                .GetValue(window);

            return sceneHierarchy;
        }

        private static EditorWindow GetHierarchyWindow()
        {
            // For it to open, so that it the current focused window.
            EditorApplication.ExecuteMenuItem("Window/General/Hierarchy");
            return EditorWindow.focusedWindow;
        }
        #endregion
    }
}
#endif
