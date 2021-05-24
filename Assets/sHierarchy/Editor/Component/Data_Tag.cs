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
using System.Linq;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditorInternal;

namespace sHierarchy
{
    public class Data_Tag : HierarchyComponent
    {
        /* Variables */

        private const string INFO =
            @"Show Tag name";

        private const string FOLD_NAME = "Tag";
        public bool foldout = false;

        // --- Text ----

        public Color textColorUntagged = Color.gray;
        public Color textColor = new Color(0.71f, 0.71f, 0.71f);

        // --- Item ----

        public Dictionary<int, Color> itemColors = new Dictionary<int, Color>();  // tag -> color
        public float gradientLength = 0.6f;
        public bool invertDirection = false;

        /* Setter & Getter */

        /* Functions */

        public override bool GetEnabled()
        {
            var hcpe = HierarchyControlPanel.instance;
            if (hcpe != null) return hcpe.f_tag;
            return this.enabled;
        }

        public override string FormKey(string name) { return HierarchyUtil.FormKey("tag.") + name; }

        public override void Init()
        {
            this.enabled = EditorPrefs.GetBool(FormKey("enabled"), this.enabled);

            this.textColorUntagged = HierarchyUtil.GetColor(FormKey("textColorUntagged"), this.textColorUntagged);
            this.textColor = HierarchyUtil.GetColor(FormKey("textColor"), this.textColor);

            this.itemColors = HierarchyUtil.GetDictionary(FormKey("itemColors"), this.itemColors);
            this.gradientLength = HierarchyUtil.GetData(FormKey("gradientLength"), this.gradientLength);
            this.invertDirection = HierarchyUtil.GetData(FormKey("invertDirection"), this.invertDirection);
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

                HierarchyUtil.LabelField("Text");

                HierarchyUtil.CreateGroup(() =>
                {
                    HierarchyUtil.BeginHorizontal(() =>
                    {
                        this.textColorUntagged = EditorGUILayout.ColorField("Untagged Color", this.textColorUntagged);
                        HierarchyUtil.Button("Reset", ResetTextUntaggedColor);
                    });

                    HierarchyUtil.BeginHorizontal(() =>
                    {
                        this.textColor = EditorGUILayout.ColorField("Color", this.textColor);
                        HierarchyUtil.Button("Reset", ResetTextColor);
                    });
                });

                HierarchyUtil.LabelField("Item");

                HierarchyUtil.CreateGroup(() =>
                {
                    HierarchyUtil.BeginHorizontal(() =>
                    {
                        EditorGUILayout.LabelField("Colors");
                        HierarchyUtil.Button("Add", AddTagColor);
                    });

                    HierarchyUtil.CreateGroup(() =>
                    {
                        string[] tags = InternalEditorUtility.tags;

                        if (itemColors.Count == 0)
                            EditorGUILayout.LabelField("No specification");

                        for (int count = 0; count < itemColors.Count; ++count)
                        {
                            HierarchyUtil.BeginHorizontal(() =>
                            {
                                int oldSelection = itemColors.Keys.ElementAt(count);
                                int currentSelection = HierarchyUtil.Popup(oldSelection, tags);

                                Color col = HierarchyUtil.ColorField("", itemColors[oldSelection]);

                                RemoveTagColor(oldSelection);
                                if (itemColors.ContainsKey(currentSelection))
                                    itemColors[currentSelection] = col;
                                else
                                    itemColors.Add(currentSelection, col);

                                if (HierarchyUtil.Button("Remove"))
                                    RemoveTagColor(currentSelection);
                            });
                        }
                    });

                    HierarchyUtil.BeginHorizontal(() =>
                    {
                        this.gradientLength = HierarchyUtil.Slider("Gradient Length", this.gradientLength, 0.01f, 1.0f,
                            @"Time of the gradient faded to alpha 0");
                        HierarchyUtil.Button("Reset", ResetGradientLength);
                    });

                    this.invertDirection = HierarchyUtil.Toggle("Invert", this.invertDirection,
                        @"Invert the gradient direction");
                });
            });
        }

        public override void SavePref()
        {
            EditorPrefs.SetBool(FormKey("enabled"), this.enabled);

            HierarchyUtil.SetColor(FormKey("textColorUntagged"), this.textColorUntagged);
            HierarchyUtil.SetColor(FormKey("textColor"), this.textColor);

            HierarchyUtil.SetDictionary(FormKey("itemColors"), this.itemColors);
            HierarchyUtil.SetData(FormKey("gradientLength"), this.gradientLength);
            HierarchyUtil.SetData(FormKey("invertDirection"), this.invertDirection);
        }

        private void ResetTextUntaggedColor() { this.textColorUntagged = Color.gray; }
        private void ResetTextColor() { this.textColor = new Color(0.71f, 0.71f, 0.71f); }
        private void AddTagColor()
        {
            Color defaultColor = Color.gray;
            defaultColor.a = 0.12f;

            if (!itemColors.ContainsKey(0))
                itemColors.Add(0, defaultColor);
        }
        private void RemoveTagColor(int selection) { itemColors.Remove(selection); }
        private void ResetGradientLength() { this.gradientLength = 0.6f; }

        private Color SafeGetColor(int id)
        {
            if (itemColors.ContainsKey(id)) return this.itemColors[id];
            return Color.clear;
        }

        public Color GetColorByTag(string tag)
        {
            List<string> tags = InternalEditorUtility.tags.ToList();
            int id = tags.IndexOf(tag);
            return SafeGetColor(id);
        }
    }
}
#endif
