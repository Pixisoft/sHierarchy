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

namespace sHierarchy
{
    [System.Serializable]
    public class Data_Layer : HierarchyComponent
    {
        /* Variables */

        private const string INFO =
            @"";

        private const string FOLD_NAME = "Layer";
        public bool foldout = false;

        // --- Text ----

        public bool enabledText = true;
        public Color textColorDefault = Color.gray;
        public Color textColor = new Color(0.71f, 0.71f, 0.71f);

        // --- Item ----

        public bool enabledItem = true;
        public Dictionary<string, Color> itemColors = new Dictionary<string, Color>();  // tag -> color
        public float gradientLength = 0.6f;
        public bool invertDirection = true;

        /* Setter & Getter */

        /* Functions */

        public override bool GetEnabled()
        {
            var hcp = HierarchyControlPanel.instance;
            if (hcp != null) return hcp.enabled && hcp.f_layer;
            return this.enabled;
        }

        public override string FormKey(string name) { return HierarchyUtil.FormKey("layer.") + name; }

        public override void Init()
        {
            // Here emphasize we want this to be disabled by default!
            {
                this.enabled = false;
            }

            this.enabled = EditorPrefs.GetBool(FormKey("enabled"), this.enabled);

            this.enabledText = EditorPrefs.GetBool(FormKey("enabledText"), this.enabledText);
            this.textColorDefault = HierarchyUtil.GetColor(FormKey("textColorDefault"), this.textColorDefault);
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
                    HierarchyUtil.CreateInfo("Show Layer name");

                    this.enabledText = HierarchyUtil.Toggle("Enabeld", this.enabledText,
                        @"Display text on the side");

                    HierarchyUtil.BeginHorizontal(() =>
                    {
                        this.textColorDefault = HierarchyUtil.ColorField("Default Color", this.textColorDefault,
                            @"Color for layer, Default");
                        HierarchyUtil.Button("Reset", ResetDefaultColor);
                    });

                    HierarchyUtil.BeginHorizontal(() =>
                    {
                        this.textColor = HierarchyUtil.ColorField("Color", this.textColor,
                            @"Text color for other meaningful layers");
                        HierarchyUtil.Button("Reset", ResetColor);
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
                        HierarchyUtil.LabelField("Colors", @"Color mapping for each layer");
                        HierarchyUtil.Button("Add", AddLayerColor);
                    });

                    HierarchyUtil.CreateGroup(() =>
                    {
                        string[] layers = Layers();

                        int drawn = 0;

                        for (int count = 0; count < itemColors.Count; ++count)
                        {
                            string oldName = itemColors.Keys.ElementAt(count);
                            int oldSelection = LayerMask.NameToLayer(oldName);

                            HierarchyUtil.BeginHorizontal(() =>
                            {
                                int currentSelection = HierarchyUtil.Popup(oldSelection, layers);
                                string currentName = LayerMask.LayerToName(currentSelection);

                                Color col = HierarchyUtil.ColorField("", itemColors[oldName],
                                    @"Color for this layer, " + currentName);

                                RemoveLayerColor(oldName);
                                if (itemColors.ContainsKey(currentName))
                                    itemColors[currentName] = col;
                                else
                                    itemColors.Add(currentName, col);

                                if (HierarchyUtil.Button("Remove"))
                                    RemoveLayerColor(currentName);

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
            HierarchyUtil.SetColor(FormKey("colorDefault"), this.textColorDefault);
            HierarchyUtil.SetColor(FormKey("textColor"), this.textColor);

            EditorPrefs.SetBool(FormKey("enabledItem"), this.enabledItem);
            HierarchyUtil.SetDictionary(FormKey("itemColors"), this.itemColors);
            HierarchyUtil.SetData(FormKey("gradientLength"), this.gradientLength);
            HierarchyUtil.SetData(FormKey("invertDirection"), this.invertDirection);
        }

        private void ResetDefaultColor() { this.textColorDefault = Color.gray; }
        private void ResetColor() { this.textColor = new Color(0.71f, 0.71f, 0.71f); }
        private void AddLayerColor()
        {
            Color defaultColor = Color.gray;
            defaultColor.a = 0.12f;

            string name = LayerMask.LayerToName(0);

            if (!itemColors.ContainsKey(name))
                itemColors.Add(name, defaultColor);
        }
        private void RemoveLayerColor(string selection) { itemColors.Remove(selection); }
        private void ResetGradientLength() { this.gradientLength = 0.6f; }

        private Color SafeGetColor(int id)
        {
            string name = LayerMask.LayerToName(id);
            if (itemColors.ContainsKey(name)) return this.itemColors[name];
            return Color.clear;
        }

        public string[] Layers()
        {
            List<string> layerNames = new List<string>();
            for (int index = 0; index <= 31; ++index)
            {
                var layerN = LayerMask.LayerToName(index);
                if (layerN.Length > 0)
                    layerNames.Add(layerN);
            }
            return layerNames.ToArray();
        }

        public Color GetColorByLayer(string tag)
        {
            List<string> tags = Layers().ToList();
            int id = tags.IndexOf(tag);
            return SafeGetColor(id);
        }
    }
}
#endif
