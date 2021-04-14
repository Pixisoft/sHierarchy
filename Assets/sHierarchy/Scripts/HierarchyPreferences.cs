#if UNITY_EDITOR
/**
 * Copyright (c) 2020 Federico Bellucci - febucci.com
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

namespace sHierarchy
{
    /// <summary>
    /// Preferences settings in `Preferences` window.
    /// </summary>
    public static class HierarchyPreferences
    {
        /* Variables */

        private static bool prefsLoaded = false;

        private static HierarchyData mData = new HierarchyData();

        /* Setter & Getters */

        public static HierarchyData data { get { return mData; } }

        /* Functions */

        [PreferenceItem("sHierarchy")]
        private static void CustomPreferencesGUI()
        {
            InitGUI();
            UpdateLogic();
            UpdateGUI();
        }

        private static void InitGUI()
        {
            if (prefsLoaded)
                return;

            mData = new HierarchyData();

            mData.enabled = EditorPrefs.GetBool("enabled", true);
            mData.updateInPlayMode = EditorPrefs.GetBool("updateInPlayMode", true);

            prefsLoaded = true;
        }

        private static void UpdateLogic()
        {
            mData.enabled = EditorGUILayout.Toggle("Enabeld: ", mData.enabled);
            mData.updateInPlayMode = EditorGUILayout.Toggle("Update In Play Mode: ", mData.updateInPlayMode);

            #region Foldout
            {
                //fo_icons = EditorGUILayout.Foldout(fo_icons, "Icons");
                //fo_prefabsData = EditorGUILayout.Foldout(fo_prefabsData, "Prefabs Data");
                //fo_alternatingBackground = EditorGUILayout.Foldout(fo_alternatingBackground, "Alternating Background");
                //fo_separator = EditorGUILayout.Foldout(fo_separator, "Separator");
                //fo_tree = EditorGUILayout.Foldout(fo_tree, "Tree");
            }
            #endregion

            //if (fo_icons) DrawIcons();
        }

        private static void UpdateGUI()
        {
            if (!GUI.changed)
                return;

            EditorPrefs.SetBool("enabled", mData.enabled);
            EditorPrefs.SetBool("updateInPlayMode", mData.updateInPlayMode);

            HierarchyDrawer.Initialize();
        }

        private static void DrawIcons()
        {
            mData.enabled = EditorGUILayout.Toggle("Enabeld: ", mData.enabled);
        }
    }
}
#endif
