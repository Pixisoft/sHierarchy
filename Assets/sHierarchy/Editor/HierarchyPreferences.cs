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

namespace sHierarchy
{
    /// <summary>
    /// Preferences settings in `Preferences` window.
    /// </summary>
    [InitializeOnLoad]
    public static class HierarchyPreferences
    {
        /* Variables */

        private static bool prefsLoaded = false;

        /* Setter & Getters */

        public static HierarchyData data { get; private set; } = new HierarchyData();

        /* Functions */

        static HierarchyPreferences()
        {
            Init();
        }

#if UNITY_2018_3_OR_NEWER
        private class sHierarchyProvider : SettingsProvider
        {
            public sHierarchyProvider(string path, SettingsScope scopes = SettingsScope.User)
                : base(path, scopes)
            { }

            public override void OnGUI(string searchContext)
            {
                CustomPreferencesGUI();
            }
        }

        [SettingsProvider]
        static SettingsProvider Create_sHierarchyProvider()
        {
            return new sHierarchyProvider("Preferences/sHierarchy", SettingsScope.User);
        }
#else
        [PreferenceItem("sHierarchy")]
#endif

        private static void CustomPreferencesGUI()
        {
            Init();
            Draw();
            SavePref();
        }

        private static void Init()
        {
            if (prefsLoaded)
                return;

            data = new HierarchyData();

            data.Init();

            prefsLoaded = true;
        }

        private static void Draw()
        {
            GUILayout.Space(10);

            data.Draw();
        }

        private static void SavePref()
        {
            if (!GUI.changed)
                return;

            data.SavePref();

            HierarchyDrawer.Initialize();
            HierarchyPreview.Draw();
        }
    }
}
#endif
