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
using UnityEngine.SceneManagement;

namespace sHierarchy
{
    /// <summary>
    /// Demo script to control sHierarchy's preference options
    /// 
    /// So you can control the preferences per scene to show different feature.
    /// </summary>
    [CustomEditor(typeof(HierarchyControlPanel))]
    public class HierarchyControlPanelEditor : Editor
    {
        /* Variables */

        private const string INFO =
            @"If you have this component in the scene, then it will force enable/disable the feature even when you have set options from the preference window

P.S. You would need to remove this component in order to make the current scene respect to the preferences window's options";

        private HierarchyControlPanel mTarget = null;

        /* Setter & Getter */

        /* Functions */

        public override void OnInspectorGUI()
        {
            HierarchyUtil.CreateInfo(INFO);

            this.mTarget = (HierarchyControlPanel)this.target;

            EditorGUI.BeginChangeCheck();

            DrawOption(HierarchyData.f_alterRowShading, ref mTarget.f_alterRowShading,
                "Alternate Row Shading", @"Enable feature Alternate Row Shading");

            DrawOption(HierarchyData.f_separator, ref mTarget.f_separator,
                "Separator", @"Enable feature Separator");

            DrawOption(HierarchyData.f_tree, ref mTarget.f_tree,
                "Tree", @"Enable feature Tree");

            DrawOption(HierarchyData.f_log, ref mTarget.f_log,
                "Logs", @"Enable feature Logs");

            DrawOption(HierarchyData.f_icons, ref mTarget.f_icons,
                "Icons", @"Enable feature Icons");

            DrawOption(HierarchyData.f_components, ref mTarget.f_components,
                "Components", @"Enable feature Components");

            DrawOption(HierarchyData.f_tag, ref mTarget.f_tag,
                "Tag", @"Enable feature Tag");

            DrawOption(HierarchyData.f_layer, ref mTarget.f_layer,
                "Layer", @"Enable feature Layer");

            DrawOption(HierarchyData.f_instanceID, ref mTarget.f_instanceID,
                "Instance ID", @"Enable feature Instance ID");

            if (EditorGUI.EndChangeCheck())
            {
                EditorUtility.SetDirty(mTarget);
            }
        }

        private static void DrawOption(bool flag, ref bool option, string label, string tooltip)
        {
            // TODO: `tooltip` is now ignore due to not able to change option
            // from preference window.
            if (!flag) return;
            option = EditorGUILayout.Toggle(label, option);
        }
    }
}
#endif
