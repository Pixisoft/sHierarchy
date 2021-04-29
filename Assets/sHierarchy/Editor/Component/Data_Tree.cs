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
using System.Collections.Generic;
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

        public bool foldout = false;

        public bool enabled = true;
        public bool colorizedLine = false;
        public bool colorizedItem = true;
        public float dividerHeight = 1;
        public Color baseLevelColor = Color.gray;
        public bool drawFill = false;
        public float overlayAlpha = 0.12f;
        public float lineAlpa = 0.8f;
        public float lineWidth = 1.0f;
        private int branchesLen = 0;

        /* Setter & Getters */

        public Color[] branches { get { return instance.branches; } }

        /* Functions */

        public string FormKey(string name) { return HierarchyUtil.FormKey("tree.") + name; }

        public void Init()
        {
            this.enabled = EditorPrefs.GetBool(FormKey("enabled"), this.enabled);
            this.colorizedLine = EditorPrefs.GetBool(FormKey("colorizedLine"), this.colorizedLine);
            this.colorizedItem = EditorPrefs.GetBool(FormKey("colorizedItem"), this.colorizedItem);
            this.dividerHeight = EditorPrefs.GetFloat(FormKey("dividerHeight"), this.dividerHeight);
            this.baseLevelColor = HierarchyUtil.GetColor(FormKey("baseLevelColor"), this.baseLevelColor);
            this.branchesLen = EditorPrefs.GetInt(FormKey("branches.Length"), this.branchesLen);
            this.overlayAlpha = EditorPrefs.GetFloat(FormKey("overlayAlpha"), this.overlayAlpha);
            this.lineAlpa = EditorPrefs.GetFloat(FormKey("lineAlpa"), this.lineAlpa);
            this.lineWidth = EditorPrefs.GetFloat(FormKey("lineWidth"), this.lineWidth);
            this.drawFill = EditorPrefs.GetBool(FormKey("drawFill"), this.drawFill);

            DynamicRefresh();
        }

        public void Draw()
        {
            foldout = EditorGUILayout.Foldout(foldout, "Tree");

            if (!foldout)
                return;

            HierarchyUtil.CreateGroup(() =>
            {
                this.enabled = EditorGUILayout.Toggle("Enabeld", this.enabled);
                this.colorizedLine = EditorGUILayout.Toggle("Colorized Line", this.colorizedLine);
                this.colorizedItem = EditorGUILayout.Toggle("Coloried Item", this.colorizedItem);
                this.dividerHeight = EditorGUILayout.Slider("Divider Height", this.dividerHeight, 0, 3);

                HierarchyUtil.BeginHorizontal(() =>
                {
                    this.baseLevelColor = EditorGUILayout.ColorField("Base Level Color", this.baseLevelColor);

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

                this.overlayAlpha = EditorGUILayout.Slider("Overlay Alpha", this.overlayAlpha, 0, 0.8f);
                HierarchyUtil.BeginHorizontal(() =>
                {
                    this.lineAlpa = EditorGUILayout.Slider("Line Alpha", this.lineAlpa, 0, 1.0f);

                    HierarchyUtil.Button("Reset", ResetLineAlpha);
                });
                HierarchyUtil.BeginHorizontal(() =>
                {
                    this.lineWidth = EditorGUILayout.Slider("Line Width", this.lineWidth, 0.001f, 3.0f);

                    HierarchyUtil.Button("Reset", ResetLineWidth);
                });
                this.drawFill = EditorGUILayout.Toggle("Draw Fill", this.drawFill);
            });
        }

        public void SavePref()
        {
            EditorPrefs.SetBool(FormKey("enabled"), this.enabled);
            EditorPrefs.SetBool(FormKey("colorizedLine"), this.colorizedLine);
            EditorPrefs.SetBool(FormKey("colorizedItem"), this.colorizedItem);
            EditorPrefs.SetFloat(FormKey("dividerHeight"), this.dividerHeight);
            HierarchyUtil.SetColor(FormKey("baseLevelColor"), this.baseLevelColor);
            {
                this.branchesLen = this.branches.Length;
                EditorPrefs.SetInt(FormKey("branches.Length"), this.branchesLen);

                for (int index = 0; index < branches.Length; ++index)
                    HierarchyUtil.SetColor(FormKey("branches" + index), instance.branches[index]);
            }
            EditorPrefs.SetFloat(FormKey("overlayAlpha"), this.overlayAlpha);
            EditorPrefs.SetFloat(FormKey("lineAlpa"), this.lineAlpa);
            EditorPrefs.SetFloat(FormKey("lineWidth"), this.lineWidth);
            EditorPrefs.SetBool(FormKey("drawFill"), this.drawFill);
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

        private void ResetBaseLevelColor()
        {
            baseLevelColor = Color.grey;
            SavePref();
        }

        private void ResetBranchesColor()
        {
            instance.branches = new Color[]
            {
                new Color(1, 0, 0, lineAlpa),
                new Color(1, 0.5f, 0, lineAlpa),
                new Color(1, 1, 0, lineAlpa),
                new Color(0, 1, 0, lineAlpa),
                new Color(0, 0, 1, lineAlpa),
                new Color(0.5f, 0, 1, lineAlpa),
                new Color(1, 0, 1, lineAlpa),
            };
            branchesLen = instance.branches.Length;
            SavePref();
        }

        private void ResetLineAlpha() { this.lineAlpa = 0.8f; }
        private void ResetLineWidth() { this.lineWidth = 1.0f; }
    }
}
#endif
