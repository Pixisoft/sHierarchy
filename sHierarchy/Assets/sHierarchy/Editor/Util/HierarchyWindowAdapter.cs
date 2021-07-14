#if UNITY_EDITOR
/**
 * Copyright (c) Pixisoft. All rights reserved.
 * 
 * pixisoft.tw@gmail.com
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
        /* Variables */

        private const string EDITOR_WINDOW_TYPE = "UnityEditor.SceneHierarchyWindow";
        private const double EDITOR_WINDOWS_CACHE_TTL = 2;

        private const BindingFlags INSTANCE_PRIVATE = BindingFlags.Instance | BindingFlags.NonPublic;
        private const BindingFlags INSTANCE_PUBLIC = BindingFlags.Instance | BindingFlags.Public;

        private static readonly FieldInfo SCENE_HIERARCHY_FIELD;
        private static readonly FieldInfo TREE_VIEW_FIELD;
        private static readonly PropertyInfo TREE_VIEW_DATA_PROPERTY;
        private static readonly MethodInfo TREE_VIEW_ITEMS_METHOD;
        private static readonly MethodInfo IsUsingAlphaSort_METHOD;
        private static readonly PropertyInfo TREE_VIEW_OBJECT_PROPERTY;

        // Windows cache
        private static double _nextWindowsUpdate;
        private static EditorWindow[] _windowsCache;

        /* Setter & Getters */

        /* Functions */

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
            IsUsingAlphaSort_METHOD = treeViewDataType.GetMethod("IsUsingAlphaSort", INSTANCE_PRIVATE);

            var treeViewItem = assembly.GetType("UnityEditor.GameObjectTreeViewItem");
            TREE_VIEW_OBJECT_PROPERTY = treeViewItem.GetProperty("objectPPTR", INSTANCE_PUBLIC);
        }

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

        public static void ApplyIconByInstanceId(int instanceId, Texture icon)
        {
            ApplyIconByInstanceId(instanceId, (Texture2D)icon);
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

        private static IEnumerable<TreeViewItem> GetTreeViewItems(EditorWindow window)
        {
            var sceneHierarchy = SCENE_HIERARCHY_FIELD.GetValue(window);
            var treeView = TREE_VIEW_FIELD.GetValue(sceneHierarchy);
            var treeViewData = TREE_VIEW_DATA_PROPERTY.GetValue(treeView, null);
            var treeViewItems = (IEnumerable<TreeViewItem>)TREE_VIEW_ITEMS_METHOD.Invoke(treeViewData, null);
            return treeViewItems;
        }

        public static bool IsUsingAlphaSort()
        {
            var hierarchyWindows = GetAllHierarchyWindows();
            foreach (var window in hierarchyWindows)
            {
                var sceneHierarchy = SCENE_HIERARCHY_FIELD.GetValue(window);
                var treeView = TREE_VIEW_FIELD.GetValue(sceneHierarchy);
                var treeViewData = TREE_VIEW_DATA_PROPERTY.GetValue(treeView, null);
                return (bool)IsUsingAlphaSort_METHOD.Invoke(treeViewData, null);
            }
            return false;
        }
    }
}
#endif
