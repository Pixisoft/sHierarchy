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
    /// <summary>
    /// Preview selected GameObject in Hierarchy.
    /// </summary>
    [CustomPreview(typeof(GameObject))]
    public class HierarchyPreview : ObjectPreview
    {
        private GameObject mTarget;
        private Editor mEditor;

        public override bool HasPreviewGUI()
        {
            return (HierarchyPreferences.data.preview.enabled && Selection.activeGameObject != null);
        }

        public override void OnPreviewGUI(Rect r, GUIStyle background)
        {
            if (mTarget != Selection.activeGameObject)
            {
                if (mEditor != null) UnityEngine.Object.DestroyImmediate(mEditor);
                mTarget = Selection.activeGameObject;
            }

            if (mTarget != null)
            {
                if (mEditor == null)
                    mEditor = Editor.CreateEditor(mTarget);

                mEditor.OnInteractivePreviewGUI(GUILayoutUtility.GetLastRect(), EditorStyles.whiteLabel);
            }
        }
    }
}
#endif
