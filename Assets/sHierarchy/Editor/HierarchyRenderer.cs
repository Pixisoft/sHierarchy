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
using UnityEngine;
using UnityEditor;

namespace sHierarchy
{
    public static class HierarchyRenderer
    {
        static float ROW_HEIGHT { get { return HierarchyDrawer.ROW_HEIGHT; } }
        static HierarchyData data { get { return HierarchyData.instance; } }
        static Color[] currentBranch { get { return data.tree.branches; } }
        static InstanceInfo currentItem { get { return HierarchyDrawer.currentItem; } }

        private const int barOffsetX = 15;

        public static void DrawNestGroupOverlay(Rect originalRect, int nestlevel)
        {
            Color color = GetNestColor(nestlevel);
            color.a = (FoundBranchColor(nestlevel)) ? 0 : data.tree.overlayAlpha;
            if (data.tree.drawFill)
                DrawFullItem(originalRect, color);
            else
                DrawSelection(originalRect, color);
        }

        public static void DrawFullItem(Rect originalRect, Color color)
        {
            const int offset = 32;
            originalRect = new Rect(offset, originalRect.y, originalRect.width + originalRect.x, originalRect.height);
            EditorGUI.DrawRect(originalRect, color);
        }

        public static void DrawSelection(Rect originalRect, Color color)
        {
            EditorGUI.DrawRect(originalRect, color);
        }

        /// <summary>
        /// Return the started x position in nested level.
        /// </summary>
        static float GetStartX(Rect originalRect, int nestLevel)
        {
            return 37 + (originalRect.height - 2) * nestLevel;
        }

        /// <summary>
        /// Return the started x position after the name of gameobject.
        /// </summary>
        static float GetNameX(Rect originalRect, int nestLevel)
        {
            float goWidth = ROW_HEIGHT + 10;
            return GetStartX(originalRect, nestLevel + 1) + goWidth +
                GUI.skin.label.CalcSize(new GUIContent(currentItem.goName)).x;
        }

        static bool FoundBranchColor(int nestLevel)
        {
            return (nestLevel == 0 || (nestLevel - 1) >= currentBranch.Length);
        }

        static Color GetNestColor(int nestLevel)
        {
            if (FoundBranchColor(nestLevel))
                return data.tree.baseLevelColor;
            return currentBranch[nestLevel - 1];
        }

        public static void DrawVerticalLineFrom(Rect originalRect, int nestLevel)
        {
            DrawHalfVerticalLineFrom(originalRect, true, nestLevel);
            DrawHalfVerticalLineFrom(originalRect, false, nestLevel);
        }

        public static void DrawHalfVerticalLineFrom(Rect originalRect, bool startsOnTop, int nestLevel)
        {
            DrawHalfVerticalLineFrom(originalRect, startsOnTop, nestLevel, GetNestColor(nestLevel));
        }

        public static void DrawHalfVerticalLineFrom(Rect originalRect, bool startsOnTop, int nestLevel, Color color)
        {
            color = (data.tree.colorizedLine) ? color : data.tree.baseLevelColor;
            color.a = data.tree.lineAlpa;

            Rect rect = new Rect(
                    GetStartX(originalRect, nestLevel) + barOffsetX,
                    startsOnTop ? originalRect.y : (originalRect.y + originalRect.height / 2f),
                    data.tree.lineWidth,
                    originalRect.height / 2f);

            // Vertical rect, starts from the very left and then proceeds to te right
            EditorGUI.DrawRect(rect, color);
        }

        public static void DrawHorizontalLineFrom(Rect originalRect, int nestLevel, bool hasChilds)
        {
            Color color = (data.tree.colorizedLine) ? GetNestColor(nestLevel) : data.tree.baseLevelColor;
            color.a = data.tree.lineAlpa;

            // Vertical rect, starts from the very left and then proceeds to te right
            Rect rect = new Rect(
                    GetStartX(originalRect, nestLevel) + barOffsetX,
                    originalRect.y + originalRect.height / 2f,
                    originalRect.height + (hasChilds ? -5 : 2 - 12),
                    data.tree.lineWidth);

            EditorGUI.DrawRect(rect, color);
        }

        public static void DrawDottedLine(Rect originalRect, int nestLevel, float offsetX = barOffsetX)
        {
            Color color = (data.tree.colorizedLine) ? GetNestColor(nestLevel) : data.tree.baseLevelColor;
            color.a = data.tree.lineAlpa;

            float x = GetStartX(originalRect, nestLevel) + offsetX;
            float y = originalRect.y;
            float height = originalRect.height / 4 - 2;
            float centerY = y + originalRect.height / 2f;

            // startsOnTop? originalRect.y: (originalRect.y + originalRect.height / 2f),

            Rect dot1 = new Rect(x, y, data.tree.lineWidth, height);
            Rect dot2 = new Rect(x, (y + centerY) / 2.0f, data.tree.lineWidth, height);
            Rect dot3 = new Rect(x, centerY, data.tree.lineWidth, height);
            Rect dot4 = new Rect(x, (centerY + y + originalRect.height) / 2.0f, data.tree.lineWidth, height);

            EditorGUI.DrawRect(dot1, color);
            EditorGUI.DrawRect(dot2, color);
            EditorGUI.DrawRect(dot3, color);
            EditorGUI.DrawRect(dot4, color);
        }

        public static GUIContent GetLogIcon(LogType type, string stackTrace)
        {
            switch (type)
            {
                case LogType.Log: return EditorGUIUtility.IconContent("console.infoicon.sml", stackTrace);
                case LogType.Warning: return EditorGUIUtility.IconContent("console.warnicon.sml", stackTrace);
                case LogType.Error: return EditorGUIUtility.IconContent("console.erroricon.sml", stackTrace);
            }
            return null;
        }

        public static bool DrawLogIcon(GameObject go, Rect selectionRect, Dictionary<string, string> log, LogType type, int iconLevel)
        {
            foreach (KeyValuePair<string, string> entry in log)
            {
                string t = entry.Key;

                if (go.GetComponent(t) == null)
                    continue;

                var stackTrace = entry.Value;
                var c = GetLogIcon(type, stackTrace);

                var x = GetNameX(selectionRect, currentItem.nestingLevel) + (ROW_HEIGHT * iconLevel);

                Rect r = new Rect(x, selectionRect.yMin, ROW_HEIGHT, ROW_HEIGHT);

                GUI.DrawTexture(r, c.image);

                return true;
            }

            return false;
        }
    }
}
#endif
