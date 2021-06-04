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
    public class Data_InstanceID : HierarchyComponent
    {
        /* Variables */

        private const string INFO =
            @"Show game object instance ID";

        private const string FOLD_NAME = "Instance ID";
        public bool foldout = false;

        public Color color = Color.gray;

        /* Setter & Getter */

        /* Functions */

        public override bool GetEnabled()
        {
            var hcp = HierarchyControlPanel.instance;
            if (hcp != null) return hcp.enabled && hcp.f_instanceID;
            return this.enabled;
        }

        public override string FormKey(string name) { return HierarchyUtil.FormKey("instanceID.") + name; }

        public override void Init()
        {
            {
                this.enabled = false;
            }

            this.enabled = EditorPrefs.GetBool(FormKey("enabled"), this.enabled);
            this.color = HierarchyUtil.GetColor(FormKey("color"), this.color);
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
                        @"Text color for instance ID");
                    HierarchyUtil.Button("Reset", ResetColor);
                });
            });
        }

        public override void SavePref()
        {
            EditorPrefs.SetBool(FormKey("enabled"), this.enabled);
            HierarchyUtil.SetColor(FormKey("color"), this.color);
        }

        private void ResetColor() { this.color = Color.gray; }
    }
}
#endif
