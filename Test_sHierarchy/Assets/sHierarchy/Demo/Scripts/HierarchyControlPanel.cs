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
