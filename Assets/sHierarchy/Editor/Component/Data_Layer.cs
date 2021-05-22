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
    public class Data_Layer : HierarchyComponent
    {
        /* Variables */

        private const string INFO =
            @"Show Layer name";

        private const string FOLD_NAME = "Layer";
        public bool foldout = false;

        public Color textColorDefault = Color.gray;
        public Color textColor = new Color(0.71f, 0.71f, 0.71f);

        // --- Item ----

        public Dictionary<int, Color> itemColors = new Dictionary<int, Color>();  // tag -> color
        public float gradientLength = 0.6f;
        public bool invertDirection = true;

        /* Setter & Getter */

        /* Functions */

        public override bool GetEnabled()
        {
            var hcpe = HierarchyControlPanelEditor.instance;
            if (hcpe != null) return hcpe.f_layer;
            return this.enabled;
        }

        public override string FormKey(string name) { return HierarchyUtil.FormKey("layer.") + name; }

        public override void Init()
        {
            {
                this.enabled = false;
            }

            this.enabled = EditorPrefs.GetBool(FormKey("enabled"), this.enabled);

            this.textColorDefault = HierarchyUtil.GetColor(FormKey("textColorDefault"), this.textColorDefault);
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

                HierarchyUtil.BeginHorizontal(() =>
                {
                    this.textColorDefault = EditorGUILayout.ColorField("Default Color", this.textColorDefault);
                    HierarchyUtil.Button("Reset", ResetDefaultColor);
                });

                HierarchyUtil.BeginHorizontal(() =>
                {
                    this.textColor = EditorGUILayout.ColorField("Color", this.textColor);
                    HierarchyUtil.Button("Reset", ResetColor);
                });

                HierarchyUtil.LabelField("Item");

                HierarchyUtil.CreateGroup(() =>
                {
                    HierarchyUtil.BeginHorizontal(() =>
                    {
                        EditorGUILayout.LabelField("Colors");
                        HierarchyUtil.Button("Add", AddLayerColor);
                    });

                    HierarchyUtil.CreateGroup(() =>
                    {
                        string[] layers = Layers();

                        if (itemColors.Count == 0)
                            EditorGUILayout.LabelField("No specification");

                        for (int count = 0; count < itemColors.Count; ++count)
                        {
                            HierarchyUtil.BeginHorizontal(() =>
                            {
                                int oldSelection = itemColors.Keys.ElementAt(count);
                                int currentSelection = HierarchyUtil.Popup(oldSelection, layers);

                                Color col = HierarchyUtil.ColorField("", itemColors[oldSelection]);

                                RemoveLayerColor(oldSelection);
                                if (itemColors.ContainsKey(currentSelection))
                                    itemColors[currentSelection] = col;
                                else
                                    itemColors.Add(currentSelection, col);

                                if (HierarchyUtil.Button("Remove"))
                                    RemoveLayerColor(currentSelection);
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

            HierarchyUtil.SetColor(FormKey("colorDefault"), this.textColorDefault);
            HierarchyUtil.SetColor(FormKey("textColor"), this.textColor);

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

            if (!itemColors.ContainsKey(0))
                itemColors.Add(0, defaultColor);
        }
        private void RemoveLayerColor(int selection) { itemColors.Remove(selection); }
        private void ResetGradientLength() { this.gradientLength = 0.6f; }

        private Color SafeGetColor(int id)
        {
            if (itemColors.ContainsKey(id)) return this.itemColors[id];
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
