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
using UnityEditor;
using UnityEngine;

namespace sHierarchy
{
    public delegate void EmptyFunction();

    public static class HierarchyUtil
    {
        public static string FormKey(string name)
        {
            return "sHierarchy." + name;
        }

        public static void CreateGroup(EmptyFunction func)
        {
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical("box");
            Indent(func);
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
        }

        public static void Indent(EmptyFunction func)
        {
            EditorGUI.indentLevel++;
            func.Invoke();
            EditorGUI.indentLevel--;
        }

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
    }
}
#endif
