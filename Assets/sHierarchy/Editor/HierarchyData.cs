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

namespace sHierarchy
{
    [System.Serializable]
    public class HierarchyData : HierarchyComponent
    {
        /* Variables */

        public bool updateInPlayMode = true;
        public bool updateInPrefabIsoMode = true;

        public Data_AlternateRowShading alterRowShading = new Data_AlternateRowShading();
        public Data_Separator separator = new Data_Separator();
        public Data_Tree tree = new Data_Tree();
        public Data_Log log = new Data_Log();
        public Data_Icons icons = new Data_Icons();
        public Data_Components components = new Data_Components();
        public Data_Tag tag = new Data_Tag();
        public Data_Layer layer = new Data_Layer();
        public Data_InstanceID instanceID = new Data_InstanceID();
        public Data_Preview preview = new Data_Preview();

        public const bool f_alterRowShading = true;
        public const bool f_separator = true;
        public const bool f_tree = true;
        public const bool f_log = true;
        public const bool f_icons = true;
        public const bool f_components = true;
        public const bool f_tag = true;
        public const bool f_layer = true;
        public const bool f_instanceID = true;
        public const bool f_preview = true;

        /* Setter & Getter */

        public static HierarchyData instance { get { return HierarchyPreferences.data; } }

        /* Functions */

        public override bool GetEnabled()
        {
            var hcp = HierarchyControlPanel.instance;
            if (hcp != null) return hcp.enabled;
            return this.enabled;
        }

        public override string FormKey(string name) { return HierarchyUtil.FormKey("root.") + name; }

        public override void Init()
        {
            this.enabled = EditorPrefs.GetBool(FormKey("enabled"), this.enabled);
            this.updateInPlayMode = EditorPrefs.GetBool(FormKey("updateInPlayMode"), this.updateInPlayMode);
            this.updateInPlayMode = EditorPrefs.GetBool(FormKey("updateInPrefabIsoMode"), this.updateInPrefabIsoMode);

            ExecuteAll(HierarchyComponentFunctions.INIT);
        }

        public override void Draw()
        {
            const float spaces = 200;

            this.enabled = HierarchyUtil.Toggle("Enabeld", this.enabled,
                @"Enable the plugin sHierarchy", spaces);
            this.updateInPlayMode = HierarchyUtil.Toggle("Update in Play Mode", this.updateInPlayMode,
                @"Draw in Play Mode", spaces);
            this.updateInPrefabIsoMode = HierarchyUtil.Toggle("Update in Prefab Mode", this.updateInPrefabIsoMode,
                @"Draw in Prefab Isolation Mode", spaces);

            ExecuteAll(HierarchyComponentFunctions.DRAW);
        }

        public override void SavePref()
        {
            EditorPrefs.SetBool(FormKey("enabled"), this.enabled);
            EditorPrefs.SetBool(FormKey("updateInPlayMode"), this.updateInPlayMode);
            EditorPrefs.SetBool(FormKey("updateInPrefabIsoMode"), this.updateInPrefabIsoMode);

            ExecuteAll(HierarchyComponentFunctions.SAVE_PREF);
        }

        private static void Execute(HierarchyComponent hc, HierarchyComponentFunctions fnc, bool flag)
        {
            if (!flag)
            {
                hc.SetEnabled(flag);
                return;
            }

            switch (fnc)
            {
                case HierarchyComponentFunctions.INIT: hc.Init(); break;
                case HierarchyComponentFunctions.DRAW: hc.Draw(); break;
                case HierarchyComponentFunctions.SAVE_PREF: hc.SavePref(); break;
            }
        }

        private void ExecuteAll(HierarchyComponentFunctions fnc)
        {
            Execute(alterRowShading, fnc, f_alterRowShading);
            Execute(separator, fnc, f_separator);
            Execute(tree, fnc, f_tree);
            Execute(log, fnc, f_log);
            Execute(icons, fnc, f_icons);
            Execute(components, fnc, f_components);
            Execute(tag, fnc, f_tag);
            Execute(layer, fnc, f_layer);
            Execute(instanceID, fnc, f_instanceID);

            Execute(preview, fnc, f_preview);
        }
    }
}
#endif
