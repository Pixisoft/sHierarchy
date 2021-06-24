#if UNITY_EDITOR
/**
 * Copyright (c) Pixisoft Corporations. All rights reserved.
 * 
 * Licensed under the Source EULA. See COPYING in the asset root for license informtaion.
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

            HierarchyUtil.BeginHorizontal(() => 
            {
                GUILayout.Space(10);

                HierarchyUtil.BeginVertical(() =>
                {
                    data.Draw();
                }, "");
            });
        }

        private static void SavePref()
        {
            if (!GUI.changed)
                return;

            data.SavePref();

            HierarchyDrawer.Initialize();
        }
    }
}
#endif
