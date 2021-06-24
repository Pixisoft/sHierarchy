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
