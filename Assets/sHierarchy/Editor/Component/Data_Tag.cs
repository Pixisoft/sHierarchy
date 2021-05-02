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
    public class Data_Tag : HierarchyComponent
    {
        /* Variables */

        public bool foldout = false;

        public bool enabled = true;
        public Color colorUntagged = Color.gray;
        public Color color = new Color(0.71f, 0.71f, 0.71f);

        /* Setter & Getter */

        /* Functions */

        public string FormKey(string name) { return HierarchyUtil.FormKey("tag.") + name; }

        public void Init()
        {
            this.enabled = EditorPrefs.GetBool(FormKey("enabled"), this.enabled);
            this.colorUntagged = HierarchyUtil.GetColor(FormKey("colorUntagged"), this.colorUntagged);
            this.color = HierarchyUtil.GetColor(FormKey("color"), this.color);
        }

        public void Draw()
        {
            foldout = EditorGUILayout.Foldout(foldout, "Tag");

            if (!foldout)
                return;

            HierarchyUtil.CreateGroup(() =>
            {
                this.enabled = EditorGUILayout.Toggle("Enabled", this.enabled);

                HierarchyUtil.BeginHorizontal(() =>
                {
                    this.colorUntagged = EditorGUILayout.ColorField("Untagged Color", this.colorUntagged);

                    HierarchyUtil.Button("Reset", ResetUntaggedColor);
                });

                HierarchyUtil.BeginHorizontal(() =>
                {
                    this.color = EditorGUILayout.ColorField("Color", this.color);

                    HierarchyUtil.Button("Reset", ResetColor);
                });
            });
        }

        public void SavePref()
        {
            EditorPrefs.SetBool(FormKey("enabled"), this.enabled);
            HierarchyUtil.SetColor(FormKey("colorUntagged"), this.colorUntagged);
            HierarchyUtil.SetColor(FormKey("color"), this.color);
        }

        private void ResetUntaggedColor() { this.colorUntagged = Color.gray; }
        private void ResetColor() { this.color = new Color(0.71f, 0.71f, 0.71f); }
    }
}
#endif
