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
    public class HierarchyData : HierarchyComponent
    {
        /* Variables */

        public bool updateInPlayMode = true;

        public Data_AlternatingBG alternatingBG = new Data_AlternatingBG();
        public Data_Prefabs prefabsData = new Data_Prefabs();
        public Data_Separator separator = new Data_Separator();
        public Data_Tree tree = new Data_Tree();
        public Data_Log log = new Data_Log();
        public Data_Tag tag = new Data_Tag();
        public Data_Icons icons = new Data_Icons();
        public Data_Components components = new Data_Components();
        public Data_InstanceID instanceID = new Data_InstanceID();
        public Data_Preview preview = new Data_Preview();

        private const bool f_alternatingBG = true;
        private const bool f_prefabsData = true;
        private const bool f_separator = true;
        private const bool f_tree = true;
        private const bool f_log = true;
        private const bool f_icons = true;
        private const bool f_components = true;
        private const bool f_tag = true;
        private const bool f_instanceID = true;
        private const bool f_preview = true;

        /* Setter & Getter */

        public static HierarchyData instance { get { return HierarchyPreferences.data; } }

        /* Functions */

        public string FormKey(string name) { return HierarchyUtil.FormKey("root.") + name; }

        public override void Init()
        {
            this.enabled = EditorPrefs.GetBool(FormKey("enabled"), this.enabled);
            this.updateInPlayMode = EditorPrefs.GetBool(FormKey("updateInPlayMode"), this.updateInPlayMode);

            ExecuteAll(HierarchyComponentFunctions.INIT);
        }

        public override void Draw()
        {
            this.enabled = EditorGUILayout.Toggle("Enabeld", this.enabled);
            this.updateInPlayMode = EditorGUILayout.Toggle("Update In Play Mode", this.updateInPlayMode);

            ExecuteAll(HierarchyComponentFunctions.DRAW);
        }

        public override void SavePref()
        {
            EditorPrefs.SetBool(FormKey("enabled"), this.enabled);
            EditorPrefs.SetBool(FormKey("updateInPlayMode"), this.updateInPlayMode);

            ExecuteAll(HierarchyComponentFunctions.SAVE_PREF);
        }

        private static void Execute(HierarchyComponent hc, HierarchyComponentFunctions fnc, bool flag)
        {
            hc.enabled = flag;
            if (!flag) return;

            switch (fnc)
            {
                case HierarchyComponentFunctions.INIT: hc.Init(); break;
                case HierarchyComponentFunctions.DRAW: hc.Draw(); break;
                case HierarchyComponentFunctions.SAVE_PREF: hc.SavePref(); break;
            }
        }

        private void ExecuteAll(HierarchyComponentFunctions fnc)
        {
            Execute(alternatingBG, fnc, f_alternatingBG);
            Execute(prefabsData, fnc, f_prefabsData);
            Execute(separator, fnc, f_separator);
            Execute(tree, fnc, f_tree);
            Execute(log, fnc, f_log);
            Execute(icons, fnc, f_icons);
            Execute(components, fnc, f_components);
            Execute(tag, fnc, f_tag);
            Execute(instanceID, fnc, f_instanceID);

            Execute(preview, fnc, f_preview);
        }
    }
}
#endif
