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
    public class Data_Log : HierarchyComponent
    {
        /* Variables */

        private const string INFO =
            @"Show log/warning/error icon after GameObject Name";

        private const string FOLD_NAME = "Log";
        public bool foldout = false;

        public bool hideLog = false;
        public bool hideWarning = false;
        public bool hideError = false;

        /* Setter & Getter */

        /* Functions */

        public override bool GetEnabled()
        {
            var hcp = HierarchyControlPanel.instance;
            if (hcp != null) return hcp.enabled && hcp.f_log;
            return this.enabled;
        }

        public override string FormKey(string name) { return HierarchyUtil.FormKey("log.") + name; }

        public override void Init()
        {
            this.enabled = EditorPrefs.GetBool(FormKey("enabled"), this.enabled);
            this.hideLog = EditorPrefs.GetBool(FormKey("hideLog"), this.hideLog);
            this.hideWarning = EditorPrefs.GetBool(FormKey("hideWarning"), this.hideWarning);
            this.hideError = EditorPrefs.GetBool(FormKey("hideError"), this.hideError);
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

                HierarchyUtil.LabelField("Hide Flags");

                HierarchyUtil.CreateGroup(() =>
                {
                    HierarchyUtil.CreateInfo(@"Hide specific log level");

                    this.hideLog = HierarchyUtil.Toggle("Hide Log", this.hideLog,
                        @"Hide log");
                    this.hideWarning = HierarchyUtil.Toggle("Hide Warning", this.hideWarning,
                        @"Hide warning");
                    this.hideError = HierarchyUtil.Toggle("Hide Error", this.hideError,
                        @"Hide error");
                });
            });
        }

        public override void SavePref()
        {
            EditorPrefs.SetBool(FormKey("enabled"), this.enabled);
            EditorPrefs.SetBool(FormKey("hideLog"), this.hideLog);
            EditorPrefs.SetBool(FormKey("hideWarning"), this.hideWarning);
            EditorPrefs.SetBool(FormKey("hideError"), this.hideError);
        }
    }
}
#endif
