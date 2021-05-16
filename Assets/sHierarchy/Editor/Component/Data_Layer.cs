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
    public class Data_Layer : HierarchyComponent
    {
        /* Variables */

        private const string INFO =
            @"Show the layer name";

        private const string FOLD_NAME = "Layer";
        public bool foldout = false;

        public Color colorDefault = Color.gray;
        public Color color = new Color(0.71f, 0.71f, 0.71f);

        /* Setter & Getter */

        /* Functions */

        public override string FormKey(string name) { return HierarchyUtil.FormKey("layer.") + name; }

        public override void Init()
        {
            {
                this.enabled = false;
            }

            this.enabled = EditorPrefs.GetBool(FormKey("enabled"), this.enabled);
            this.colorDefault = HierarchyUtil.GetColor(FormKey("colorDefault"), this.colorDefault);
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
                    this.colorDefault = EditorGUILayout.ColorField("Default Color", this.colorDefault);
                    HierarchyUtil.Button("Reset", ResetDefaultColor);
                });

                HierarchyUtil.BeginHorizontal(() =>
                {
                    this.color = EditorGUILayout.ColorField("Color", this.color);
                    HierarchyUtil.Button("Reset", ResetColor);
                });
            });
        }

        public override void SavePref()
        {
            EditorPrefs.SetBool(FormKey("enabled"), this.enabled);
            HierarchyUtil.SetColor(FormKey("colorDefault"), this.colorDefault);
            HierarchyUtil.SetColor(FormKey("color"), this.color);
        }

        private void ResetDefaultColor() { this.colorDefault = Color.gray; }
        private void ResetColor() { this.color = new Color(0.71f, 0.71f, 0.71f); }
    }
}
#endif