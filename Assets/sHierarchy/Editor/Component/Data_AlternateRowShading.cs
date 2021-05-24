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
            var hcpe = HierarchyControlPanel.instance;
            if (hcpe != null) return hcpe.f_alterRowShading;
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
                    this.color = EditorGUILayout.ColorField("Color", this.color);
                    HierarchyUtil.Button("Reset", ResetColor);
                });

                this.drawFill = EditorGUILayout.Toggle("Draw Fill", this.drawFill);
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
