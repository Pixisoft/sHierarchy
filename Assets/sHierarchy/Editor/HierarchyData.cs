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

        public bool enabled = true;
        public bool updateInPlayMode = true;

        public Data_Icons icons = new Data_Icons();
        public Data_Prefabs prefabsData = new Data_Prefabs();
        public Data_AlternatingBG alternatingBG = new Data_AlternatingBG();
        public Data_Separator separator = new Data_Separator();
        public Data_Tree tree = new Data_Tree();
        public Data_Log log = new Data_Log();
        public Data_Tag tag = new Data_Tag();
        public Data_InstanceID instanceID = new Data_InstanceID();

        public Data_Preview preview = new Data_Preview();

        /* Setter & Getter */

        public static HierarchyData instance { get { return HierarchyPreferences.data; } }

        /* Functions */

        public string FormKey(string name) { return HierarchyUtil.FormKey("root.") + name; }

        public void Init()
        {
            this.enabled = EditorPrefs.GetBool(FormKey("enabled"), this.enabled);
            this.updateInPlayMode = EditorPrefs.GetBool(FormKey("updateInPlayMode"), this.updateInPlayMode);

            alternatingBG.Init();
            prefabsData.Init();
            separator.Init();
            tree.Init();
            log.Init();
            tag.Init();
            icons.Init();
            instanceID.Init();

            preview.Init();
        }

        public void Draw()
        {
            this.enabled = EditorGUILayout.Toggle("Enabeld", this.enabled);
            this.updateInPlayMode = EditorGUILayout.Toggle("Update In Play Mode", this.updateInPlayMode);

            alternatingBG.Draw();
            prefabsData.Draw();
            separator.Draw();
            tree.Draw();
            log.Draw();
            tag.Draw();
            icons.Draw();
            instanceID.Draw();

            preview.Draw();
        }

        public void SavePref()
        {
            EditorPrefs.SetBool(FormKey("enabled"), this.enabled);
            EditorPrefs.SetBool(FormKey("updateInPlayMode"), this.updateInPlayMode);

            alternatingBG.SavePref();
            prefabsData.SavePref();
            separator.SavePref();
            tree.SavePref();
            log.SavePref();
            tag.SavePref();
            icons.SavePref();
            instanceID.SavePref();

            preview.SavePref();
        }
    }
}
#endif
