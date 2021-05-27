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
    [System.Serializable]
    public class Data_Tag : HierarchyComponent
    {
        /* Variables */

        private const string INFO =
            @"";

        private const string FOLD_NAME = "Tag";
        public bool foldout = false;

        // --- Text ----

        public bool enabledText = true;
        public Color textColorUntagged = Color.gray;
        public Color textColor = new Color(0.71f, 0.71f, 0.71f);

        // --- Item ----

        public bool enabledItem = true;
        public Dictionary<string, Color> itemColors = new Dictionary<string, Color>();  // tag -> color
        public float gradientLength = 0.6f;
        public bool invertDirection = false;

        /* Setter & Getter */

        /* Functions */

        public override bool GetEnabled()
        {
            var hcp = HierarchyControlPanel.instance;
            if (hcp != null) return hcp.enabled && hcp.f_tag;
            return this.enabled;
        }

        public override string FormKey(string name)
        {
            return HierarchyUtil.FormKey("tag." + HierarchyUtil.PROJECT_NAME + ".") + name;
        }

        public override void Init()
        {
            this.enabled = EditorPrefs.GetBool(FormKey("enabled"), this.enabled);

            this.enabledText = EditorPrefs.GetBool(FormKey("enabledText"), this.enabledText);
            this.textColorUntagged = HierarchyUtil.GetColor(FormKey("textColorUntagged"), this.textColorUntagged);
            this.textColor = HierarchyUtil.GetColor(FormKey("textColor"), this.textColor);

            this.enabledItem = EditorPrefs.GetBool(FormKey("enabledItem"), this.enabledItem);
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
                    HierarchyUtil.CreateInfo("Show Tag name");

                    this.enabledText = HierarchyUtil.Toggle("Enabeld", this.enabledText,
                        @"Display text on the side");

                    HierarchyUtil.BeginHorizontal(() =>
                    {
                        this.textColorUntagged = HierarchyUtil.ColorField("Untagged Color", this.textColorUntagged,
                            @"Color for layer, Untagged");
                        HierarchyUtil.Button("Reset", ResetTextUntaggedColor);
                    });

                    HierarchyUtil.BeginHorizontal(() =>
                    {
                        this.textColor = HierarchyUtil.ColorField("Color", this.textColor,
                            @"Text color for other meaningful tags");
                        HierarchyUtil.Button("Reset", ResetTextColor);
                    });
                });

                HierarchyUtil.LabelField("Item");

                HierarchyUtil.CreateGroup(() =>
                {
                    HierarchyUtil.CreateInfo("Colorized Item");

                    this.enabledItem = HierarchyUtil.Toggle("Enabeld", this.enabledItem,
                        @"Colorized items in hierarchy");

                    HierarchyUtil.BeginHorizontal(() =>
                    {
                        HierarchyUtil.LabelField("Colors", @"Color mapping for each tag");
                        HierarchyUtil.Button("Add", AddTagColor);
                    });

                    HierarchyUtil.CreateGroup(() =>
                    {
                        string[] tags = InternalEditorUtility.tags;

                        int drawn = 0;

                        for (int count = 0; count < itemColors.Count; ++count)
                        {
                            string oldName = itemColors.Keys.ElementAt(count);
                            int oldSelection = TagToIndex(oldName);

                            HierarchyUtil.BeginHorizontal(() =>
                            {
                                int currentSelection = HierarchyUtil.Popup(oldSelection, tags);
                                string currentName = IndexToTag(currentSelection);

                                Color col = HierarchyUtil.ColorField("", itemColors[oldName],
                                    @"Color for this tag, " + currentName);

                                RemoveTagColor(oldName);
                                if (itemColors.ContainsKey(currentName))
                                    itemColors[currentName] = col;
                                else
                                    itemColors.Add(currentName, col);

                                if (HierarchyUtil.Button("Remove"))
                                    RemoveTagColor(currentName);

                                ++drawn;
                            });
                        }

                        if (drawn == 0)
                            EditorGUILayout.LabelField("No specification");
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

            EditorPrefs.SetBool(FormKey("enabledText"), this.enabledText);
            HierarchyUtil.SetColor(FormKey("textColorUntagged"), this.textColorUntagged);
            HierarchyUtil.SetColor(FormKey("textColor"), this.textColor);

            EditorPrefs.SetBool(FormKey("enabledItem"), this.enabledItem);
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

            string name = "Untagged";

            if (!itemColors.ContainsKey(name))
                itemColors.Add(name, defaultColor);
        }
        private void RemoveTagColor(string selection) { itemColors.Remove(selection); }
        private void ResetGradientLength() { this.gradientLength = 0.6f; }

        private int TagToIndex(string tag)
        {
            List<string> tags = InternalEditorUtility.tags.ToList();
            int id = tags.IndexOf(tag);
            return id;
        }

        private string IndexToTag(int id)
        {
            return InternalEditorUtility.tags[id];
        }

        public Color GetColorByTag(string tag)
        {
            if (itemColors.ContainsKey(tag)) return this.itemColors[tag];
            return Color.clear;
        }
    }
}
#endif
