#if UNITY_EDITOR
/**
 * Copyright (c) Pixisoft. All rights reserved.
 * 
 * pixisoft.tw@gmail.com
 */
using UnityEditor;

namespace sHierarchy
{
    [System.Serializable]
    public class Data_Components : HierarchyComponent
    {
        /* Variables */

        private const string INFO = 
            @"Show components icon on the right";

        private const string FOLD_NAME = "Components";
        public bool foldout = false;

        public bool focus = true;

        public float disableAlpa = 0.5f;

        /* Setter & Getters */

        /* Functions */

        public override bool GetEnabled()
        {
            var hcp = HierarchyControlPanel.instance;
            if (hcp != null) return hcp.enabled && hcp.f_components;
            return this.enabled;
        }

        public override string FormKey(string name) { return HierarchyUtil.FormKey("components.") + name; }

        public override void Init()
        {
            this.enabled = EditorPrefs.GetBool(FormKey("enabled"), true);
            this.focus = EditorPrefs.GetBool(FormKey("focus"), true);
            this.disableAlpa = EditorPrefs.GetFloat(FormKey("disableAlpa"), this.disableAlpa);
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

                this.focus = HierarchyUtil.Toggle("Folding", this.focus, 
                    @"Focus the component after clicking the icon");

                HierarchyUtil.BeginHorizontal(() =>
                {
                    this.disableAlpa = HierarchyUtil.Slider("Disable Alpha", this.disableAlpa, 0.1f, 0.9f,
                        @"Alpha for disabled components");
                    HierarchyUtil.Button("Reset", ResetDisableAlpha);
                });
            });
        }

        public override void SavePref()
        {
            EditorPrefs.SetBool(FormKey("enabled"), this.enabled);
            EditorPrefs.SetBool(FormKey("focus"), this.focus);
            EditorPrefs.SetFloat(FormKey("disableAlpa"), this.disableAlpa);
        }

        private void ResetDisableAlpha() { this.disableAlpa = 0.5f; }
    }
}
#endif
