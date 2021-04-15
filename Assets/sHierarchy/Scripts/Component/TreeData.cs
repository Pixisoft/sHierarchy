#if UNITY_EDITOR
/**
 * Copyright (c) 2020 Federico Bellucci - febucci.com
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
    public class TreeData : HierarchyComponent
    {
        public bool foldout = false;
        public bool enabled = true;
        public bool drawOverlayOnColoredPrefabs = true;
        [Range(0, 3)] public float dividerHeigth = 1;
        public Color baseLevelColor = Color.gray;

        [System.Serializable]
        public class BranchGroup
        {
            public Color overlayColor;
            public Color[] colors = new Color[0];
        }

        public BranchGroup[] branches = new[]
        {
            new BranchGroup()
            {
                overlayColor =  new Color(1f, 0.44f, 0.97f, .04f),
                colors = new []
                {
                    new Color(1f, 0.44f, 0.97f),
                    new Color(0.56f, 0.44f, 1f),
                    new Color(0.44f, 0.71f, 1f),
                    new Color(0.19f, 0.53f, 0.78f)
                }
            },

            new BranchGroup()
            {
                overlayColor =  new Color(0.93f, 1f, 0.42f, .04f),
                colors = new []
                {
                    new Color(0.93f, 1f, 0.42f),
                    new Color(1f, 0.75f, 0.42f),
                    new Color(1f, 0.46f, 0.31f),
                    new Color(1f, 0.35f, 0.34f)
                }
            }
        };

        public string FormKey(string name) { return HierarchyUtil.FormKey("tree.") + name; }

        public void Init()
        {
            this.enabled = EditorPrefs.GetBool(FormKey("enabled"), false);
        }

        public void Draw()
        {
            foldout = EditorGUILayout.Foldout(foldout, "Tree");

            if (foldout)
            {
                this.enabled = EditorGUILayout.Toggle("Enabeld: ", this.enabled);
            }
        }

        public void SavePref()
        {
            EditorPrefs.SetBool(FormKey("enabled"), this.enabled);
        }
    }
}
#endif
