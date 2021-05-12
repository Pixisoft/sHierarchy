﻿#if UNITY_EDITOR
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
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace sHierarchy
{
    [InitializeOnLoad]
    public static class HierarchyDrawer
    {
        /* Variables */

        public static float ROW_HEIGHT = 15;

        /* Setter & Getter */

        /* Functions */

        static HierarchyDrawer()
        {
            Initialize();
        }

        private static bool initialized = false;
        private static HierarchyData data { get { return HierarchyPreferences.data; } }
        private static List<int> iconsPositions = new List<int>();
        private static Dictionary<int, InstanceInfo> sceneGameObjects = new Dictionary<int, InstanceInfo>();
        private static Dictionary<int, Color> prefabColors = new Dictionary<int, Color>();

        #region Menu Items

        #region Internal

        [MenuItem("sHierarchy/Preferences", priority = 1)]
        public static void InitializeOrCreate()
        {
            SettingsService.OpenUserPreferences("Preferences/sHierarchy");
        }

        #endregion

        #region Links

        [MenuItem("sHierarchy/📝 Repository", priority = 50)]
        static void m_OpenBlog()
        {
            Application.OpenURL("https://github.com/jcs090218/sHierarchy");
        }

        [MenuItem("sHierarchy/🐦 Twitter", priority = 51)]
        static void m_OpenTwitter()
        {
            Application.OpenURL("https://twitter.com/jenchieh94");
        }

        #endregion

        #endregion

        #region Initialization/Helpers

        /// <summary>
        /// Initializes the script at the beginning. 
        /// </summary>
        public static void Initialize()
        {
            #region Unregisters previous events

            if (initialized)
            {
                // Prevents registering events multiple times
                EditorApplication.hierarchyChanged -= RetrieveDataFromScene;
                EditorApplication.hierarchyWindowItemOnGUI -= DrawCore;
            }

            #endregion

            initialized = false;

            initialized = true;

            if (data.enabled)
            {
                #region Registers events

                EditorApplication.hierarchyChanged += RetrieveDataFromScene;
                EditorApplication.hierarchyWindowItemOnGUI += DrawCore;

                #endregion

                RetrieveDataFromScene();

                prefabColors.Clear();
                foreach (var prefab in data.prefabsData.prefabs)
                {
                    if (prefab.color.a <= 0) continue;
                    if (!prefab.gameObject) continue;

                    int instanceID = prefab.gameObject.GetInstanceID();
                    if (prefabColors.ContainsKey(instanceID)) continue;

                    prefabColors.Add(instanceID, prefab.color);
                }
            }

            EditorApplication.RepaintHierarchyWindow();
        }

        #endregion

        /// <summary>
        /// Updates the list of objects to draw, icons etc.
        /// </summary>
        static void RetrieveDataFromScene()
        {
            // TEMPORARY: fix for performance reasons while in play mode
            if (!data.updateInPlayMode && Application.isPlaying)
                return;

            sceneGameObjects.Clear();
            iconsPositions.Clear();

            GameObject[] sceneRoots;
            Scene tempScene;

            for (int i = 0; i < SceneManager.sceneCount; ++i)
            {
                tempScene = SceneManager.GetSceneAt(i);

                if (!tempScene.isLoaded)
                    continue;

                sceneRoots = tempScene.GetRootGameObjects();

                // TODO: Find hierarchy sorting type...
                bool alpha = false;
                if (alpha)
                    sceneRoots = sceneRoots.OrderBy(go => go.name).ToArray();

                // Analyzes all scene's gameObjects
                for (int j = 0; j < sceneRoots.Length; ++j)
                {
                    var go = sceneRoots[j];

                    AnalyzeGoWithChildren(
                        sceneRoots[j],
                        nestingLevel: 0,
                        sceneRoots[j].transform.childCount > 0,
                        j,
                        j == (sceneRoots.Length - 1));
                }
            }
        }

        static void AnalyzeGoWithChildren(GameObject go, int nestingLevel, bool topParentHasChild, int nestingGroup, bool isLastChild)
        {
            int instanceID = go.GetInstanceID();

            if (!sceneGameObjects.ContainsKey(instanceID))  // processes the gameobject only if it wasn't processed already
            {
                InstanceInfo newInfo = new InstanceInfo();
                newInfo.iconIndexes = new List<int>();
                newInfo.isLastElement = isLastChild;
                newInfo.nestingLevel = nestingLevel;
                newInfo.nestingGroup = nestingGroup;
                newInfo.hasChilds = go.transform.childCount > 0;
                newInfo.isGoActive = go.activeInHierarchy;
                newInfo.topParentHasChild = topParentHasChild;
                newInfo.goName = go.name;

                if (data.prefabsData.enabled)
                {
                    var prefab = PrefabUtility.GetCorrespondingObjectFromSource(go);
                    if (prefab)
                        newInfo.prefabInstanceID = prefab.GetInstanceID();
                }

                if (data.separator.enabled)
                {
                    newInfo.isSeparator = String.Compare(go.tag, "EditorOnly", StringComparison.Ordinal) == 0  // gameobject has EditorOnly tag
                                          && (!string.IsNullOrEmpty(go.name) && !string.IsNullOrEmpty(data.separator.startString)
                                          && go.name.StartsWith(data.separator.startString));  // and also starts with '>'
                }

                if (data.icons.enabled && data.icons.pairs != null && data.icons.pairs.Length > 0)
                {
                    #region Components Information (icons)

                    Type classReferenceType; //todo opt
                    Type componentType;

                    foreach (var c in go.GetComponents<Component>())
                    {
                        if (!c) continue;

                        componentType = c.GetType();

                        for (int elementIndex = 0; elementIndex < data.icons.pairs.Length; ++elementIndex)
                        {
                            if (!data.icons.pairs[elementIndex].iconToDraw) continue;

                            // Class inherithance
                            foreach (var classReference in data.icons.pairs[elementIndex].targetClasses)
                            {
                                if (!classReference) continue;

                                classReferenceType = classReference.GetClass();

                                if (!classReferenceType.IsClass) continue;

                                // class ineriths 
                                if (componentType.IsAssignableFrom(classReferenceType) || componentType.IsSubclassOf(classReferenceType))
                                {
                                    // Adds the icon index to the "positions" list, to draw all of them in order later [if enabled] 
                                    if (!iconsPositions.Contains(elementIndex)) iconsPositions.Add(elementIndex);

                                    // Adds the icon index to draw, only if it's not present already
                                    if (!newInfo.iconIndexes.Contains(elementIndex))
                                        newInfo.iconIndexes.Add(elementIndex);

                                    break;
                                }
                            }
                        }
                    }

                    newInfo.iconIndexes.Sort();

                    #endregion
                }

                //Adds element to the array
                sceneGameObjects.Add(instanceID, newInfo);
            }

            #region Analyzes Childrens

            int childCount = go.transform.childCount;
            for (int j = 0; j < childCount; ++j)
            {
                // TODO: This wouldn't work with alphabetic sorting

                var child = go.transform.GetChild(j).gameObject;

                AnalyzeGoWithChildren(
                    child,
                    nestingLevel + 1,
                    topParentHasChild,
                    nestingGroup,
                    j == childCount - 1);
            }

            #endregion
        }

        #region Drawing

        private static int temp_iconsDrawedCount = -1;
        public static InstanceInfo currentItem;
        private static bool drawedPrefabOverlay = false;

        private static void DrawCore(int instanceID, Rect selectionRect)
        {
            // skips early if item is not registered or not valid
            if (!sceneGameObjects.ContainsKey(instanceID))
                return;

            currentItem = sceneGameObjects[instanceID];

            /* Initialzie draw variables */
            {
                ROW_HEIGHT = GUI.skin.label.lineHeight + 1;
            }

            DrawAlternatingBG(instanceID, selectionRect);
            DrawPrefabBG(instanceID, selectionRect);
            DrawTree(instanceID, selectionRect);
            DrawSeparators(instanceID, selectionRect);
            DrawLog(instanceID, selectionRect);
            DrawIcons(instanceID, selectionRect);
            DrawTag(instanceID, selectionRect);
            DrawInstanceID(instanceID, selectionRect);
        }

        private static void DrawAlternatingBG(int instanceID, Rect selectionRect)
        {
            if (!data.alternatingBG.enabled)
                return;

            var isOdd = Mathf.FloorToInt(((selectionRect.y - 4) / ROW_HEIGHT) % 2) != 0;
            if (isOdd) return;

            if (data.alternatingBG.drawFill)
                HierarchyRenderer.DrawFullItem(selectionRect, data.alternatingBG.color);
            else
                HierarchyRenderer.DrawSelection(selectionRect, data.alternatingBG.color);

        }

        private static void DrawPrefabBG(int instanceID, Rect selectionRect)
        {
            drawedPrefabOverlay = false;

            if (!data.prefabsData.enabled || prefabColors.Count <= 0)
                return;

            if (prefabColors.ContainsKey(currentItem.prefabInstanceID))
            {
                EditorGUI.DrawRect(selectionRect, prefabColors[currentItem.prefabInstanceID]);
                drawedPrefabOverlay = true;
            }
        }

        private static void DrawSeparators(int instanceID, Rect selectionRect)
        {
            // EditorOnly objects are only removed from build if they're not childrens
            if (!data.separator.enabled || data.separator.color.a <= 0 || !currentItem.isSeparator || currentItem.nestingLevel != 0)
                return;

            // Adds color on top of the label
            if (data.separator.drawFill)
                HierarchyRenderer.DrawFullItem(selectionRect, data.separator.color);
            else
                HierarchyRenderer.DrawSelection(selectionRect, data.separator.color);
        }

        private static void DrawTree(int instanceID, Rect selectionRect)
        {
            if (!data.tree.enabled || currentItem.nestingLevel < 0)
                return;

            // prevents drawing when the hierarchy search mode is enabled
            if (selectionRect.x >= 60)
            {
                // Group
                if (data.tree.colorizedItem && !drawedPrefabOverlay && currentItem.topParentHasChild)
                {
                    HierarchyRenderer.DrawNestGroupOverlay(selectionRect, currentItem.nestingLevel);
                }

                HierarchyRenderer.DrawDottedLine(selectionRect, 0, 0);

                if (currentItem.nestingLevel == 0 && !currentItem.hasChilds)
                {
                    HierarchyRenderer.DrawHalfVerticalLineFrom(selectionRect, true, 0, data.tree.baseLevelColor);
                    if (!currentItem.isLastElement)
                        HierarchyRenderer.DrawHalfVerticalLineFrom(selectionRect, false, 0, data.tree.baseLevelColor);
                }
                else
                {
                    // Draws a vertical line for each previous nesting level
                    for (int level = 0; level <= currentItem.nestingLevel; ++level)
                    {
                        bool currentLevel = (currentItem.nestingLevel == level);

                        if (currentLevel && currentItem.hasChilds)
                            continue;

                        if (currentLevel && !currentItem.hasChilds)
                        {
                            HierarchyRenderer.DrawHalfVerticalLineFrom(selectionRect, true, level);
                            if (!currentItem.isLastElement)
                                HierarchyRenderer.DrawHalfVerticalLineFrom(selectionRect, false, level);
                        }
                        else
                        {
                            HierarchyRenderer.DrawDottedLine(selectionRect, level);
                        }
                    }
                }

                if (!currentItem.hasChilds)
                    HierarchyRenderer.DrawHorizontalLineFrom(selectionRect, currentItem.nestingLevel, currentItem.hasChilds);
            }

            // draws a super small divider between different groups
            if (currentItem.nestingLevel == 0 && data.tree.dividerHeight > 0)
            {
                Rect boldGroupRect = new Rect(
                    32, selectionRect.y - data.tree.dividerHeight / 2f,
                    selectionRect.width + (selectionRect.x - 32),
                    data.tree.dividerHeight);
                EditorGUI.DrawRect(boldGroupRect, Color.black * .3f);
            }
        }

        private static void DrawLog(int instanceID, Rect selectionRect)
        {
            if (!data.log.enabled)
                return;

            Dictionary<string, string> log = HierarchyLog.LogStorage(LogType.Log);
            Dictionary<string, string> warn = HierarchyLog.LogStorage(LogType.Warning);
            Dictionary<string, string> error = HierarchyLog.LogStorage(LogType.Error);

            var go = EditorUtility.InstanceIDToObject(instanceID) as GameObject;

            if (!go.activeSelf)
                return;

            int iconLevel = 0;

            bool drawnLog = HierarchyRenderer.DrawLogIcon(go, selectionRect, log, LogType.Log, iconLevel);
            if (drawnLog) ++iconLevel;
            bool drawnWarn = HierarchyRenderer.DrawLogIcon(go, selectionRect, warn, LogType.Warning, iconLevel);
            if (drawnWarn) ++iconLevel;
            HierarchyRenderer.DrawLogIcon(go, selectionRect, error, LogType.Error, iconLevel);
        }

        private static void DrawIcons(int instanceID, Rect selectionRect)
        {
            if (!data.icons.enabled)
                return;

            temp_iconsDrawedCount = -1;

            #region Local Method

            // Draws each component icon
            void DrawIcon(int textureIndex)
            {
                //---Icon Alignment---
                if (data.icons.aligned)
                {
                    // Aligns icon based on texture's position on the array
                    int CalculateIconPosition()
                    {
                        for (int i = 0; i < iconsPositions.Count; ++i)
                        {
                            if (iconsPositions[i] == textureIndex)
                                return i;
                        }

                        return 0;
                    }

                    temp_iconsDrawedCount = CalculateIconPosition();
                }
                else
                {
                    ++temp_iconsDrawedCount;
                }

                GUI.DrawTexture(
                    new Rect(selectionRect.xMax - ROW_HEIGHT * (temp_iconsDrawedCount + 1) - 2, selectionRect.yMin, ROW_HEIGHT, ROW_HEIGHT),
                    data.icons.pairs[textureIndex].iconToDraw);
            }

            #endregion

            // Draws the gameobject icon, if present
            var content = EditorGUIUtility.ObjectContent(EditorUtility.InstanceIDToObject(instanceID), null);

            if (content.image && !string.IsNullOrEmpty(content.image.name))
            {
                if (content.image.name != "d_GameObject Icon" && content.image.name != "d_Prefab Icon")
                {
                    ++temp_iconsDrawedCount;

                    Rect rect = new Rect(
                            selectionRect.xMax - (ROW_HEIGHT * temp_iconsDrawedCount) - 5 - HierarchyUtil.InstanceIDLength(instanceID),
                            selectionRect.yMin,
                            ROW_HEIGHT, ROW_HEIGHT);

                    GUI.DrawTexture(rect, content.image);
                    /* Add tooltip? */
                    {
                        //GUI.Label(rect, new GUIContent("", "TOOLTIP HERE"));
                    }
                }
            }

            for (int i = 0; i < currentItem.iconIndexes.Count; ++i)
                DrawIcon(currentItem.iconIndexes[i]);
        }

        private static void DrawTag(int instanceID, Rect selectionRect)
        {
            if (!data.tag.enabled)
                return;

            GameObject go = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            string fullStr = go.tag;

            float offset = GUI.skin.label.CalcSize(new GUIContent(fullStr)).x;

            if (data.icons.enabled)
            {
                int iconCount = temp_iconsDrawedCount + 1;
                iconCount = (iconCount <= 0) ? 0 : iconCount;
                float iconOffset = (iconCount == 0) ? 0.0f : 5.0f;
                offset += iconCount * ROW_HEIGHT + iconOffset;
            }

            var x = selectionRect.xMax - offset + ROW_HEIGHT - 1 - HierarchyUtil.InstanceIDLength(instanceID);
            Rect rect = new Rect(x, selectionRect.y, selectionRect.width, selectionRect.height);

            GUIStyle style = new GUIStyle();
            style.normal.textColor = (go.tag == "Untagged") ? data.tag.colorUntagged : data.tag.color;

            GUI.Label(rect, fullStr, style);
        }

        private static void DrawInstanceID(int instanceID, Rect selectionRect)
        {
            if (!data.instanceID.enabled)
                return;

            string fullStr = instanceID.ToString();

            var x = selectionRect.xMax - HierarchyUtil.InstanceIDLength(instanceID) + ROW_HEIGHT - 1;
            Rect rect = new Rect(x, selectionRect.y, selectionRect.width, selectionRect.height);

            GUIStyle style = new GUIStyle();
            style.normal.textColor = data.instanceID.color;

            GUI.Label(rect, fullStr, style);
        }

        #endregion
    }
}
#endif
