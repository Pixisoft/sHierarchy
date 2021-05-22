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
    /// Demo script to control sHierarchy's preference options
    /// 
    /// So you can control the preferences per scene to show different feature.
    /// </summary>
    [CustomEditor(typeof(HierarchyControlPanel))]
    public class HierarchyControlPanelEditor : Editor
    {
        public static HierarchyControlPanelEditor instance = null;

        /* Variables */

        private const string INFO =
            @"If you have this component in the scene, then it won't respect the option from preference window";

        public bool f_alterRowShading = true;
        public bool f_separator = true;
        public bool f_tree = true;
        public bool f_log = true;
        public bool f_icons = true;
        public bool f_components = true;
        public bool f_tag = true;
        public bool f_layer = true;
        public bool f_instanceID = true;
        public bool f_preview = true;

        /* Setter & Getter */

        /* Functions */

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();


            instance = this;

            HierarchyUtil.CreateInfo(INFO);

            DrawOption(HierarchyData.f_alterRowShading, ref f_alterRowShading,
                "Alternate Row Shading", @"Enable feature Alternate Row Shading");

            DrawOption(HierarchyData.f_separator, ref f_separator,
                "Separator", @"Enable feature Separator");

            DrawOption(HierarchyData.f_tree, ref f_tree,
                "Tree", @"Enable feature Tree");

            DrawOption(HierarchyData.f_log, ref f_log,
                "Logs", @"Enable feature Logs");

            DrawOption(HierarchyData.f_icons, ref f_icons,
                "Icons", @"Enable feature Icons");

            DrawOption(HierarchyData.f_components, ref f_components,
                "Components", @"Enable feature Components");

            DrawOption(HierarchyData.f_tag, ref f_tag,
                "Tag", @"Enable feature Tag");

            DrawOption(HierarchyData.f_layer, ref f_layer,
                "Layer", @"Enable feature Layer");

            DrawOption(HierarchyData.f_instanceID, ref f_instanceID,
                "Instance ID", @"Enable feature Instance ID");

            DrawOption(HierarchyData.f_preview, ref f_preview,
                "Preview", @"Enable feature Preview");
        }

        private void OnDisable()
        {
            instance = null;
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
