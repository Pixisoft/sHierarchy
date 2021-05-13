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
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

namespace sHierarchy
{
    [InitializeOnLoad]
    public static class HierarchyWindowAdapter
    {
        private const string EDITOR_WINDOW_TYPE = "UnityEditor.SceneHierarchyWindow";
        private const double EDITOR_WINDOWS_CACHE_TTL = 2;

        private const BindingFlags INSTANCE_PRIVATE = BindingFlags.Instance | BindingFlags.NonPublic;
        private const BindingFlags INSTANCE_PUBLIC = BindingFlags.Instance | BindingFlags.Public;

        private static readonly FieldInfo SCENE_HIERARCHY_FIELD;
        private static readonly FieldInfo TREE_VIEW_FIELD;
        private static readonly PropertyInfo TREE_VIEW_DATA_PROPERTY;
        private static readonly MethodInfo TREE_VIEW_ITEMS_METHOD;
        private static readonly PropertyInfo TREE_VIEW_OBJECT_PROPERTY;

        // Windows cache
        private static double _nextWindowsUpdate;
        private static EditorWindow[] _windowsCache;

        //---------------------------------------------------------------------
        // Ctor
        //---------------------------------------------------------------------

        static HierarchyWindowAdapter()
        {
            // Reflection

            var assembly = Assembly.GetAssembly(typeof(EditorWindow));

            var hierarchyWindowType = assembly.GetType("UnityEditor.SceneHierarchyWindow");
            SCENE_HIERARCHY_FIELD = hierarchyWindowType.GetField("m_SceneHierarchy", INSTANCE_PRIVATE);

            var sceneHierarchyType = assembly.GetType("UnityEditor.SceneHierarchy");
            TREE_VIEW_FIELD = sceneHierarchyType.GetField("m_TreeView", INSTANCE_PRIVATE);

            var treeViewType = assembly.GetType("UnityEditor.IMGUI.Controls.TreeViewController");
            TREE_VIEW_DATA_PROPERTY = treeViewType.GetProperty("data", INSTANCE_PUBLIC);

            var treeViewDataType = assembly.GetType("UnityEditor.GameObjectTreeViewDataSource");
            TREE_VIEW_ITEMS_METHOD = treeViewDataType.GetMethod("GetRows", INSTANCE_PUBLIC);

            var treeViewItem = assembly.GetType("UnityEditor.GameObjectTreeViewItem");
            TREE_VIEW_OBJECT_PROPERTY = treeViewItem.GetProperty("objectPPTR", INSTANCE_PUBLIC);
        }

        //---------------------------------------------------------------------
        // Public
        //---------------------------------------------------------------------

        public static EditorWindow GetFirstHierarchyWindow()
        {
            return GetAllHierarchyWindows().FirstOrDefault();
        }

        [SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
        [SuppressMessage("ReSharper", "InvertIf")]
        public static IEnumerable<EditorWindow> GetAllHierarchyWindows(bool forceUpdate = false)
        {
            if (forceUpdate || _nextWindowsUpdate < EditorApplication.timeSinceStartup)
            {
                _nextWindowsUpdate = EditorApplication.timeSinceStartup + EDITOR_WINDOWS_CACHE_TTL;
                _windowsCache = GetAllWindowsByType(EDITOR_WINDOW_TYPE).ToArray();
            }

            return _windowsCache;
        }

        public static void ApplyIconByInstanceId(int instanceId, Texture2D icon)
        {
            var hierarchyWindows = GetAllHierarchyWindows();

            foreach (var window in hierarchyWindows)
            {
                var treeViewItems = GetTreeViewItems(window);
                var treeViewItem = treeViewItems.FirstOrDefault(item => item.id == instanceId);
                if (treeViewItem != null) treeViewItem.icon = icon;
            }
        }

        public static IEnumerable<EditorWindow> GetAllWindowsByType(string type)
        {
            var objectList = Resources.FindObjectsOfTypeAll(typeof(EditorWindow));
            var windows = from obj in objectList where obj.GetType().ToString() == type select (EditorWindow)obj;
            return windows;
        }

        //---------------------------------------------------------------------
        // Helpers
        //---------------------------------------------------------------------

        private static IEnumerable<TreeViewItem> GetTreeViewItems(EditorWindow window)
        {
            var sceneHierarchy = SCENE_HIERARCHY_FIELD.GetValue(window);
            var treeView = TREE_VIEW_FIELD.GetValue(sceneHierarchy);
            var treeViewData = TREE_VIEW_DATA_PROPERTY.GetValue(treeView, null);
            var treeViewItems = (IEnumerable<TreeViewItem>)TREE_VIEW_ITEMS_METHOD.Invoke(treeViewData, null);

            return treeViewItems;
        }
    }
}
#endif
