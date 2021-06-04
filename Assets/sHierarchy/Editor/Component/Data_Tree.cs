#if UNITY_EDITOR
/**
 * Copyright (c) Pixisoft Corporations. All rights reserved.
 * 
 * Licensed under the Source EULA. See COPYING in the asset root for license informtaion.
 */
using UnityEditor;
using UnityEngine;

namespace sHierarchy
{
    public class Container_TreeData : ScriptableObject
    {
        public Color[] branches = new Color[0];
    }

    [System.Serializable]
    public class Data_Tree : HierarchyComponent
    {
        /* Variables */

        private static Container_TreeData instance = ScriptableObject.CreateInstance<Container_TreeData>();
        private static SerializedObject serializedObject = null;
        private static SerializedProperty propBranches = null;

        private const string INFO = 
            @"";

        private const string FOLD_NAME = "Tree";
        public bool foldout = false;

        // --- Bar ----

        public bool colorizedLine = false;
        public float lineAlpha = 0.8f;
        public float lineWidth = 1.0f;

        // --- Item ----

        public bool colorizedItem = false;
        public DrawMode drawMode = DrawMode.GRADIENT;
        public float gradientLength = 0.6f;
        public Color baseLevelColor = Color.gray;
        public float overlayAlpha = 0.12f;

        private int branchesLen = 0;

        // --- Divider ----

        public Color dividerColor = Color.black;
        public float dividerHeight = 0.0f;

        /* Setter & Getters */

        public Color[] branches { get { return instance.branches; } }

        /* Functions */

        public override bool GetEnabled()
        {
            var hcp = HierarchyControlPanel.instance;
            if (hcp != null) return hcp.enabled && hcp.f_tree;
            return this.enabled;
        }

        public override string FormKey(string name) { return HierarchyUtil.FormKey("tree.") + name; }

        public override void Init()
        {
            this.enabled = EditorPrefs.GetBool(FormKey("enabled"), this.enabled);
            // Bar
            {
                this.colorizedLine = EditorPrefs.GetBool(FormKey("colorizedLine"), this.colorizedLine);
                this.lineAlpha = EditorPrefs.GetFloat(FormKey("lineAlpha"), this.lineAlpha);
                this.lineWidth = EditorPrefs.GetFloat(FormKey("lineWidth"), this.lineWidth);
            }
            // Item
            {
                this.colorizedItem = EditorPrefs.GetBool(FormKey("colorizedItem"), this.colorizedItem);
                this.drawMode = (DrawMode)EditorPrefs.GetInt(FormKey("drawMode"), (int)this.drawMode);
                this.gradientLength = EditorPrefs.GetFloat(FormKey("gradientLength"), this.gradientLength);
                this.baseLevelColor = HierarchyUtil.GetColor(FormKey("baseLevelColor"), this.baseLevelColor);
                this.branchesLen = EditorPrefs.GetInt(FormKey("branches.Length"), this.branchesLen);
                this.overlayAlpha = EditorPrefs.GetFloat(FormKey("overlayAlpha"), this.overlayAlpha);
            }
            // Divider
            {
                this.dividerColor = HierarchyUtil.GetColor(FormKey("dividerColor"), this.dividerColor);
                this.dividerHeight = EditorPrefs.GetFloat(FormKey("dividerHeight"), this.dividerHeight);
            }

            DynamicRefresh();
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

                HierarchyUtil.LabelField("Bar");

                HierarchyUtil.CreateGroup(() =>
                {
                    HierarchyUtil.CreateInfo("Colorized Bar by tree level");

                    this.colorizedLine = HierarchyUtil.Toggle("Colorized Line", this.colorizedLine,
                        @"Enabled/Disabled to colorized bar");

                    HierarchyUtil.BeginHorizontal(() =>
                    {
                        this.lineAlpha = HierarchyUtil.Slider("Line Alpha", this.lineAlpha, 0.0f, 1.0f,
                            @"Alpha level for the bar");
                        HierarchyUtil.Button("Reset", ResetLineAlpha);
                    });

                    HierarchyUtil.BeginHorizontal(() =>
                    {
                        this.lineWidth = HierarchyUtil.Slider("Line Width", this.lineWidth, 0.001f, 3.0f,
                            @"Line width for the bar");
                        HierarchyUtil.Button("Reset", ResetLineWidth);
                    });
                });

                HierarchyUtil.LabelField("Item");

                HierarchyUtil.CreateGroup(() =>
                {
                    HierarchyUtil.CreateInfo("Colorized Item by tree level");

                    this.colorizedItem = HierarchyUtil.Toggle("Colorized Item", this.colorizedItem,
                        @"Enabled/Disabled to colorized item");

                    this.drawMode = (DrawMode)HierarchyUtil.EnumPopup("Draw Mode", this.drawMode,
                        @"Mode for item to draw");

                    HierarchyUtil.BeginHorizontal(() =>
                    {
                        this.gradientLength = HierarchyUtil.Slider("Gradient Length", this.gradientLength, 0.01f, 1.0f,
                            @"Time of the gradient faded to alpha 0");
                        HierarchyUtil.Button("Reset", ResetGradientLength);
                    });

                    HierarchyUtil.BeginHorizontal(() =>
                    {
                        this.overlayAlpha = HierarchyUtil.Slider("Overlay Alpha", this.overlayAlpha, 0.0f, 0.8f,
                            @"Alpha level for the item");
                        HierarchyUtil.Button("Reset", ResetOverlayAlpha);
                    });

                    HierarchyUtil.BeginHorizontal(() =>
                    {
                        this.baseLevelColor = HierarchyUtil.ColorField("Base Level Color", this.baseLevelColor,
                            @"Base color for colorized item");
                        HierarchyUtil.Button("Reset", ResetBaseLevelColor);
                    });

                    HierarchyUtil.BeginHorizontal(() =>
                    {
                        instance = ScriptableObject.CreateInstance<Container_TreeData>();
                        serializedObject = new SerializedObject(instance);
                        propBranches = serializedObject.FindProperty("branches");

                        DynamicRefresh();

                        serializedObject.Update();
                        EditorGUILayout.PropertyField(propBranches);
                        serializedObject.ApplyModifiedProperties();

                        HierarchyUtil.Button("Reset", ResetBranchesColor);
                    });
                });

                HierarchyUtil.LabelField("Divider");

                HierarchyUtil.CreateGroup(() =>
                {
                    HierarchyUtil.CreateInfo("Thin lines indicate each group of tree level");

                    HierarchyUtil.BeginHorizontal(() =>
                    {
                        this.dividerColor = HierarchyUtil.ColorField("Color", this.dividerColor,
                            @"Color for the divider");
                        HierarchyUtil.Button("Reset", ResetDividerColor);
                    });

                    HierarchyUtil.BeginHorizontal(() =>
                    {
                        this.dividerHeight = HierarchyUtil.Slider("Divider Height", this.dividerHeight, 0.0f, 3.0f,
                            @"Height of the divider");
                        HierarchyUtil.Button("Reset", ResetDividerHeight);
                    });
                });
            });
        }

        public override void SavePref()
        {
            EditorPrefs.SetBool(FormKey("enabled"), this.enabled);

            // Bar
            {
                EditorPrefs.SetBool(FormKey("colorizedLine"), this.colorizedLine);
                EditorPrefs.SetFloat(FormKey("lineAlpha"), this.lineAlpha);
                EditorPrefs.SetFloat(FormKey("lineWidth"), this.lineWidth);
            }

            // Item
            {
                EditorPrefs.SetBool(FormKey("colorizedItem"), this.colorizedItem);
                EditorPrefs.SetInt(FormKey("drawMode"), (int)this.drawMode);
                EditorPrefs.SetFloat(FormKey("gradientLength"), this.gradientLength);
                HierarchyUtil.SetColor(FormKey("baseLevelColor"), this.baseLevelColor);
                {
                    this.branchesLen = this.branches.Length;
                    EditorPrefs.SetInt(FormKey("branches.Length"), this.branchesLen);

                    for (int index = 0; index < branches.Length; ++index)
                        HierarchyUtil.SetColor(FormKey("branches" + index), instance.branches[index]);
                }
                EditorPrefs.SetFloat(FormKey("overlayAlpha"), this.overlayAlpha);
            }

            // Divider
            {
                HierarchyUtil.SetColor(FormKey("dividerColor"), this.dividerColor);
                EditorPrefs.SetFloat(FormKey("dividerHeight"), this.dividerHeight);
            }
        }

        private void DynamicRefresh()
        {
            if (branchesLen > 0)
            {
                instance.branches = new Color[branchesLen];
                for (int index = 0; index < branchesLen; ++index)
                    instance.branches[index] = HierarchyUtil.GetColor(FormKey("branches" + index), instance.branches[index]);
            }
        }

        private void ResetDividerHeight() { this.dividerHeight = 0.0f; }
        private void ResetDividerColor() { this.dividerColor = Color.black; }

        private void ResetBaseLevelColor()
        {
            baseLevelColor = Color.grey;
            SavePref();
        }

        private void ResetBranchesColor()
        {
            instance.branches = new Color[]
            {
                new Color(1, 0, 0, lineAlpha),
                new Color(1, 0.5f, 0, lineAlpha),
                new Color(1, 1, 0, lineAlpha),
                new Color(0, 1, 0, lineAlpha),
                new Color(0, 0, 1, lineAlpha),
                new Color(0.5f, 0, 1, lineAlpha),
                new Color(1, 0, 1, lineAlpha),
            };
            branchesLen = instance.branches.Length;
            SavePref();
        }

        private void ResetOverlayAlpha() { this.overlayAlpha = 0.12f; }

        private void ResetLineAlpha() { this.lineAlpha = 0.8f; }
        private void ResetLineWidth() { this.lineWidth = 1.0f; }
        private void ResetGradientLength() { this.gradientLength = 0.6f; }
    }
}
#endif
