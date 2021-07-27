#if UNITY_EDITOR
/**
 * Copyright (c) Pixisoft. All rights reserved.
 * 
 * pixisoft.tw@gmail.com
 */
using UnityEditor;

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
