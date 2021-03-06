#if UNITY_EDITOR
/**
 * Copyright (c) Pixisoft. All rights reserved.
 * 
 * pixisoft.tw@gmail.com
 */
using UnityEditor;
using UnityEngine;

namespace sHierarchy
{
    [System.Serializable]
    public class Data_Separator : HierarchyComponent
    {
        /* Variables */

        private const string INFO =
            @"Separator requires the tag `EditorOnly` and PREFIX string to match";

        private const string FOLD_NAME = "Separator";
        public bool foldout = false;

        public string prefix = ">";
        public Color color = new Color(0, 1, 1, 0.15f);
        public bool drawFill = true;

        /* Setter & Getters */

        /* Functions */

        public override bool GetEnabled()
        {
            var hcp = HierarchyControlPanel.instance;
            if (hcp != null) return hcp.enabled && hcp.f_separator;
            return this.enabled;
        }

        public override string FormKey(string name) { return HierarchyUtil.FormKey("separator.") + name; }

        public override void Init()
        {
            this.enabled = EditorPrefs.GetBool(FormKey("enabled"), this.enabled);
            this.prefix = EditorPrefs.GetString(FormKey("prefix"), this.prefix);
            this.color = HierarchyUtil.GetColor(FormKey("color"), this.color);
            this.drawFill = EditorPrefs.GetBool(FormKey("drawFill"), this.drawFill);
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

                HierarchyUtil.BeginHorizontal(() =>
                {
                    this.prefix = HierarchyUtil.TextField("Prefix", this.prefix,
                        @"Prefix string for separator in GameObject's name");
                    HierarchyUtil.Button("Reset", ResetPrefix);
                });

                HierarchyUtil.BeginHorizontal(() =>
                {
                    this.color = HierarchyUtil.ColorField("Color", this.color,
                        @"Color of the separator");
                    HierarchyUtil.Button("Reset", ResetColor);
                });

                this.drawFill = HierarchyUtil.Toggle("Draw Fill", this.drawFill,
                    @"Draw separator from start all the way to the end");
            });
        }

        public override void SavePref()
        {
            EditorPrefs.SetBool(FormKey("enabled"), this.enabled);
            EditorPrefs.SetString(FormKey("prefix"), this.prefix);
            HierarchyUtil.SetColor(FormKey("color"), this.color);
            EditorPrefs.SetBool(FormKey("drawFill"), this.drawFill);
        }

        private void ResetPrefix() { this.prefix = ">"; }
        private void ResetColor() { this.color = new Color(0, 1, 1, 0.15f); }
    }
}
#endif