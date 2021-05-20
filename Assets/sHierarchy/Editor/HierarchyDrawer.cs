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
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.SceneManagement;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace sHierarchy
{
    [InitializeOnLoad]
    public static class HierarchyDrawer
    {
        /* Variables */

        public static float ROW_HEIGHT = 15;

        private static bool initialized = false;
        private static HierarchyData data { get { return HierarchyPreferences.data; } }
        private static Dictionary<int, InstanceInfo> sceneGameObjects = new Dictionary<int, InstanceInfo>();

        private static HashSet<string> tags = new HashSet<string>();
        private static float MAX_TAG_LEN = 0.0f;

        private static HashSet<string> layers = new HashSet<string>();
        private static float MAX_LAYER_LEN = 0.0f;

        private static HashSet<int> instanceIDs = new HashSet<int>();
        private static float MAX_INSTID_LEN = 0.0f;

        private static string STAGE_NAME = "";

        /* Setter & Getter */

        /* Functions */

        static HierarchyDrawer()
        {
            Initialize();
        }

        private static bool IsMainStage() { return STAGE_NAME == "MainStage"; }

        private static void Repaint()
        {
            if (IsMainStage())
                EditorApplication.RepaintHierarchyWindow();
            else
                SceneView.RepaintAll();
        }

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
                EditorApplication.hierarchyChanged -= RetrieveDataFromHierarchy;
                EditorApplication.hierarchyWindowItemOnGUI -= DrawCore;
            }

            #endregion

            initialized = true;

            if (data.enabled)
            {
                #region Registers events

                EditorApplication.hierarchyChanged += RetrieveDataFromHierarchy;
                EditorApplication.hierarchyWindowItemOnGUI += DrawCore;

                #endregion

                RetrieveDataFromHierarchy();
            }

            Repaint();
        }

        #endregion

        static void RetrieveDataFromHierarchy()
        {
            Stage stage = StageUtility.GetCurrentStage();

            STAGE_NAME = stage.name;

            if (IsMainStage())
            {
                RetrieveDataFromScene();
            }
            else
            {
                PrefabStage ps = PrefabStageUtility.GetCurrentPrefabStage();
                GameObject prefabGO = ps.prefabContentsRoot;

                if (data.updateInPrefabIsoMode)
                    RetrieveFromGameObjects(new GameObject[] { prefabGO });
            }
        }

        /// <summary>
        /// Updates the list of objects to draw, icons etc.
        /// </summary>
        static void RetrieveDataFromScene()
        {
            // TEMPORARY: fix for performance reasons while in play mode
            if (!data.updateInPlayMode && Application.isPlaying)
                return;

            sceneGameObjects.Clear();
            tags.Clear();
            layers.Clear();
            instanceIDs.Clear();

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

                RetrieveFromGameObjects(sceneRoots);
            }
        }

        static void RetrieveFromGameObjects(GameObject[] gos)
        {
            // Analyzes all scene's gameObjects
            for (int j = 0; j < gos.Length; ++j)
            {
                AnalyzeGoWithChildren(
                    gos[j],
                    nestingLevel: 0,
                    gos[j].transform.childCount > 0,
                    j,
                    j == (gos.Length - 1));
            }
        }

        static void AnalyzeGoWithChildren(GameObject go, int nestingLevel, bool topParentHasChild, int nestingGroup, bool isLastChild)
        {
            int instanceID = go.GetInstanceID();

            if (!sceneGameObjects.ContainsKey(instanceID))  // processes the gameobject only if it wasn't processed already
            {
                InstanceInfo newInfo = new InstanceInfo();
                newInfo.components = new List<Component>();
                newInfo.isLastElement = isLastChild;
                newInfo.nestingLevel = nestingLevel;
                newInfo.nestingGroup = nestingGroup;
                newInfo.hasChilds = go.transform.childCount > 0;
                newInfo.isGoActive = go.activeInHierarchy;
                newInfo.topParentHasChild = topParentHasChild;
                newInfo.goName = go.name;

                // We minus 1 when inside prefab isolation mode since
                // there is no scene root in that view.
                if (!IsMainStage())
                    --newInfo.nestingLevel;

                tags.Add(go.tag);
                layers.Add(LayerMask.LayerToName(go.layer));
                instanceIDs.Add(instanceID);

                /* Prefab Data */
                {
                    var prefab = PrefabUtility.GetCorrespondingObjectFromSource(go);
                    if (prefab)
                        newInfo.prefabInstanceID = prefab.GetInstanceID();
                }

                if (data.separator.enabled)
                {
                    newInfo.isSeparator = String.Compare(go.tag, "EditorOnly", StringComparison.Ordinal) == 0  // gameobject has EditorOnly tag
                                          && (!string.IsNullOrEmpty(go.name) && !string.IsNullOrEmpty(data.separator.prefix)
                                          && go.name.StartsWith(data.separator.prefix));  // and also starts with '>'
                }

                if (data.components.enabled)
                {
                    #region Components Information (icons)

                    var comps = go.GetComponents<Component>();

                    foreach (var c in comps)
                    {
                        if (c == null)
                        {
                            newInfo.components.Add(null);  // missing
                            continue;
                        }
                        newInfo.components.Add(c);
                    }

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
        public static GameObject currentGO;

        // Right boundary
        private static float RIGHT_BOUNDARY = 0.0f;

        private static void DrawCore(int instanceID, Rect selectionRect)
        {
            // skips early if item is not registered or not valid
            if (!sceneGameObjects.ContainsKey(instanceID))
                return;

            currentGO = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
            if (currentGO == null)
                return;

            currentItem = sceneGameObjects[instanceID];

            /* Initialzie draw variables */
            {
                ROW_HEIGHT = GUI.skin.label.lineHeight + 1;  // default is 16 pixels
                MAX_TAG_LEN = HierarchyUtil.MaxLabelLength(tags.ToArray(), data.tag.enabled);
                MAX_LAYER_LEN = HierarchyUtil.MaxLabelLength(layers.ToArray(), data.layer.enabled);
                MAX_INSTID_LEN = HierarchyUtil.MaxIntLength(instanceIDs.ToArray(), data.instanceID.enabled);

                RIGHT_BOUNDARY = 0.0f;  // reset
                if (currentItem.prefabInstanceID != default(int))
                    RIGHT_BOUNDARY += ROW_HEIGHT;
            }

            //-- Full
            DrawAlternatingBG(instanceID, selectionRect);
            DrawTree(instanceID, selectionRect);
            DrawSeparators(instanceID, selectionRect);
            //-- Left
            DrawIcons(instanceID, selectionRect);
            DrawLog(instanceID, selectionRect);
            //-- Right
            DrawComponents(instanceID, selectionRect);
            DrawTag(instanceID, selectionRect);
            DrawLayer(instanceID, selectionRect);
            DrawInstanceID(instanceID, selectionRect);
        }

        private static void DrawAlternatingBG(int instanceID, Rect selectionRect)
        {
            if (!data.alterRowShading.enabled)
                return;

            var isOdd = Mathf.FloorToInt(((selectionRect.y - 4) / ROW_HEIGHT) % 2) != 0;
            if (isOdd) return;

            if (data.alterRowShading.drawFill)
                HierarchyRenderer.DrawFullItem(selectionRect, data.alterRowShading.color);
            else
                HierarchyRenderer.DrawSelection(selectionRect, data.alterRowShading.color);
        }

        private static void DrawSeparators(int instanceID, Rect selectionRect)
        {
            // EditorOnly objects are only removed from build if they're not childrens
            if (!data.separator.enabled || data.separator.color.a <= 0 || !currentItem.isSeparator)
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
                if (data.tree.colorizedItem && currentItem.topParentHasChild && !currentItem.isSeparator)
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
                EditorGUI.DrawRect(boldGroupRect, data.tree.dividerColor * .3f);
            }
        }

        private static void DrawIcons(int instanceID, Rect selectionRect)
        {
            if (!data.icons.enabled)
                return;

            // Draws the gameobject icon, if present
            var content = EditorGUIUtility.ObjectContent(currentGO, null);

            if (content.image && !string.IsNullOrEmpty(content.image.name))
            {
                if (content.image.name != "d_GameObject Icon" && content.image.name != "d_Prefab Icon")
                {
                    HierarchyWindowAdapter.ApplyIconByInstanceId(instanceID, (Texture2D)content.image);
                }
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

            if (go == null || !go.activeSelf)
                return;

            int iconLevel = 0;

            bool drawnLog = HierarchyRenderer.DrawLogIcon(go, selectionRect, log, LogType.Log, iconLevel);
            if (drawnLog) ++iconLevel;
            bool drawnWarn = HierarchyRenderer.DrawLogIcon(go, selectionRect, warn, LogType.Warning, iconLevel);
            if (drawnWarn) ++iconLevel;
            HierarchyRenderer.DrawLogIcon(go, selectionRect, error, LogType.Error, iconLevel);
        }

        private static void OnClick_Component(int instanceID, Type t)
        {
            bool selected = Selection.activeGameObject == currentGO;
            Selection.activeGameObject = currentGO;

            if (!selected)
                return;

            var check = currentItem.components[0];
            var checkType = check.GetType();
            if (checkType == t && currentItem.components.Count > 1)
                checkType = currentItem.components[1].GetType();

            bool otherExpanded = HierarchyUtil.IsExpanded(instanceID, checkType);
            bool selfExpanded = HierarchyUtil.IsExpanded(instanceID, t);

            if (!selfExpanded || !selected)
                HierarchyUtil.FocusComponent(instanceID, t);
            else if (!otherExpanded)
                HierarchyUtil.ExpandComponents(instanceID, true);
            else
                HierarchyUtil.FocusComponent(instanceID, t);
        }

        private static void DrawComponents(int instanceID, Rect selectionRect)
        {
            if (!data.components.enabled)
                return;

            temp_iconsDrawedCount = 0;
            float offsetX_const = 3;
            float offsetX = RIGHT_BOUNDARY + offsetX_const + MAX_TAG_LEN + MAX_LAYER_LEN + MAX_INSTID_LEN;

            foreach (Component comp in currentItem.components)
            {
                // When component is null, meaning there is broken link
                var t = (comp == null) ? null : comp.GetType();
                var image = HierarchyUtil.TypeTexture(comp, t);

                float offset = offsetX + (ROW_HEIGHT * temp_iconsDrawedCount);
                float x = selectionRect.xMax - offset;
                Rect rect = new Rect(x, selectionRect.yMin, ROW_HEIGHT, ROW_HEIGHT);

                HierarchyRenderer.DrawComponent(t, currentGO, rect, image);

                if (data.components.focus && GUI.Button(rect, "", "Label"))
                    OnClick_Component(instanceID, t);

                ++temp_iconsDrawedCount;
            }
        }

        private static void DrawTag(int instanceID, Rect selectionRect)
        {
            if (!data.tag.enabled)
                return;

            string fullStr = currentGO.tag;
            float offset = GUI.skin.label.CalcSize(new GUIContent(fullStr)).x;

            Rect rect = selectionRect;
            rect.x = selectionRect.xMax - RIGHT_BOUNDARY - offset + (ROW_HEIGHT - 1) - MAX_LAYER_LEN - MAX_INSTID_LEN;

            GUIStyle style = new GUIStyle();
            style.normal.textColor = (fullStr == "Untagged") ? data.tag.colorUntagged : data.tag.color;

            GUI.Label(rect, fullStr, style);
        }

        private static void DrawLayer(int instanceID, Rect selectionRect)
        {
            if (!data.layer.enabled)
                return;

            string fullStr = LayerMask.LayerToName(currentGO.layer);
            float offset = GUI.skin.label.CalcSize(new GUIContent(fullStr)).x;

            Rect rect = selectionRect;
            rect.x = selectionRect.xMax - RIGHT_BOUNDARY - offset + (ROW_HEIGHT - 1) - MAX_INSTID_LEN;

            GUIStyle style = new GUIStyle();
            style.normal.textColor = (fullStr == "Default") ? data.layer.colorDefault : data.layer.color;

            GUI.Label(rect, fullStr, style);
        }

        private static void DrawInstanceID(int instanceID, Rect selectionRect)
        {
            if (!data.instanceID.enabled)
                return;

            string fullStr = instanceID.ToString();

            float offset = MAX_INSTID_LEN - ROW_HEIGHT + 1 + RIGHT_BOUNDARY;
            float x = selectionRect.xMax - offset;
            Rect rect = new Rect(x, selectionRect.y, selectionRect.width, selectionRect.height);

            GUIStyle style = new GUIStyle();
            style.normal.textColor = data.instanceID.color;

            GUI.Label(rect, fullStr, style);
        }

        #endregion
    }
}
#endif
