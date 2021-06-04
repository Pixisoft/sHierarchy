#if UNITY_EDITOR
/**
 * Copyright (c) Pixisoft Corporations. All rights reserved.
 * 
 * Licensed under the Source EULA. See COPYING in the asset root for license informtaion.
 */
using UnityEditor;
using UnityEngine;

namespace sHierarchy
{
    [System.Serializable]
    public class Data_Icons : HierarchyComponent
    {
        /* Variables */

        private const string INFO =
            @"Replace default GameObject icon on the top left of the inspector";

        private const string FOLD_NAME = "Icons";
        public bool foldout = false;

        public bool guess = true;

        public bool omitComp = true;

        /* Setter & Getters */

        /* Functions */

        public override bool GetEnabled()
        {
            var hcp = HierarchyControlPanel.instance;
            if (hcp != null) return hcp.enabled && hcp.f_icons;
            return this.enabled;
        }

        public override string FormKey(string name) { return HierarchyUtil.FormKey("icons.") + name; }

        public override void Init()
        {
            this.enabled = EditorPrefs.GetBool(FormKey("enabled"), true);

            this.guess = EditorPrefs.GetBool(FormKey("guess"), this.guess);
            this.omitComp = EditorPrefs.GetBool(FormKey("omitComp"), this.omitComp);
        }

        public override void Draw()
        {
            foldout = EditorGUILayout.Foldout(foldout, FOLD_NAME);

            if (!foldout)
                return;

            HierarchyUtil.CreateGroup(() =>
            {
                HierarchyUtil.CreateInfo(INFO);

                this.enabled = HierarchyUtil.Toggle("Enabeld", this.enabled,
                    @"Enable/Disable all features from this section");

                this.guess = HierarchyUtil.Toggle("Guess", this.guess,
                    @"Guess the icon by the name of the GameObject");

                this.omitComp = HierarchyUtil.Toggle("Omit Component", this.omitComp,
                    @"Omit guessed component");
            });
        }

        public override void SavePref()
        {
            EditorPrefs.SetBool(FormKey("enabled"), this.enabled);

            EditorPrefs.SetBool(FormKey("guess"), this.guess);
            EditorPrefs.SetBool(FormKey("omitComp"), this.omitComp);
        }
    }
}
#endif
