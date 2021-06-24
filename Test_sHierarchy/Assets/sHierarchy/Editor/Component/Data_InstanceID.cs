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
