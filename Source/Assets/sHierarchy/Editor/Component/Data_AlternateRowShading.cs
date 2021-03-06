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
    public class Data_AlternateRowShading : HierarchyComponent
    {
        /* Variables */

        private const string INFO =
            @"Draw Row Shading";

        private const string FOLD_NAME = "Alternate Row Shading";
        public bool foldout = false;

        public Color color = new Color(0, 0, 0, .08f);
        public bool drawFill = true;

        /* Setter & Getters */

        /* Functions */

        public override bool GetEnabled()
        {
            var hcp = HierarchyControlPanel.instance;
            if (hcp != null) return hcp.enabled && hcp.f_alterRowShading;
            return this.enabled;
        }

        public override string FormKey(string name) { return HierarchyUtil.FormKey("ARS.") + name; }

        public override void Init()
        {
            this.enabled = EditorPrefs.GetBool(FormKey("enabled"), this.enabled);
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
                    this.color = HierarchyUtil.ColorField("Color", this.color,
                        @"Color for odd row");
                    HierarchyUtil.Button("Reset", ResetColor);
                });

                this.drawFill = HierarchyUtil.Toggle("Draw Fill", this.drawFill,
                    @"Draw the row entirely to the left");
            });
        }

        public override void SavePref()
        {
            EditorPrefs.SetBool(FormKey("enabled"), this.enabled);
            HierarchyUtil.SetColor(FormKey("color"), this.color);
            EditorPrefs.SetBool(FormKey("drawFill"), this.drawFill);
        }

        private void ResetColor() { this.color = new Color(0, 0, 0, .08f); }
    }
}
#endif
