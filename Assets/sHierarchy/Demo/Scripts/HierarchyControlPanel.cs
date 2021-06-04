#if UNITY_EDITOR
/**
 * Copyright (c) Pixisoft Corporations. All rights reserved.
 * 
 * Licensed under the Source EULA. See COPYING in the asset root for license informtaion.
 */
using UnityEngine;

namespace sHierarchy
{
    /// <summary>
    /// Script will be used by `HierarchyControlPanelEditor`
    /// </summary>
    [ExecuteAlways]
    public class HierarchyControlPanel : MonoBehaviour
    {
        /* Variables */

        public static HierarchyControlPanel _instance = null;

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

        public static HierarchyControlPanel instance { get { return _instance; } }

        /* Functions */

        private void Update()
        {
            _instance = this;
        }
    }
}
#endif
