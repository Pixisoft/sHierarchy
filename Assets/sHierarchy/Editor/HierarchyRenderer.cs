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
using System;

namespace sHierarchy
{
    public static class HierarchyRenderer
    {
        /* Variables */

        private const int barOffsetX = 15;

        /* Setter & Getters */

        private static float ROW_HEIGHT { get { return HierarchyDrawer.ROW_HEIGHT; } }
        private static HierarchyData data { get { return HierarchyData.instance; } }
        private static Color[] currentBranch { get { return data.tree.branches; } }
        private static InstanceInfo currentItem { get { return HierarchyDrawer.currentItem; } }

        /* Functions */

        public static void DrawNestGroupOverlay(Rect originalRect, int nestlevel)
        {
            Color color = GetNestColor(nestlevel);
            color.a = (FoundBranchColor(nestlevel)) ? 0 : data.tree.overlayAlpha;

            switch (data.tree.drawMode)
            {
                case DrawMode.GRADIENT:
                    {
                        Texture2D texture = GradientImage(color);
                        originalRect.x = GetGOIconStartX(originalRect, nestlevel);
                        GUI.DrawTexture(originalRect, texture);
                    }
                    break;
                case DrawMode.FILL:
                    {
                        DrawFullItem(originalRect, color);
                    }
                    break;
                case DrawMode.SEMI:
                    {
                        DrawSelection(originalRect, color);
                    }
                    break;
            }
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
        public static float GetStartX(Rect originalRect, int nestLevel)
        {
            return 37 + (originalRect.height - 2) * nestLevel;
        }

        /// <summary>
        /// Return the started x position after the name of gameobject.
        /// </summary>
        public static float GetNameX(Rect originalRect, int nestLevel)
        {
            float goWidth = ROW_HEIGHT + 10;
            return GetStartX(originalRect, nestLevel + 1) + goWidth +
                GUI.skin.label.CalcSize(new GUIContent(currentItem.goName)).x;
        }

        public static float GetGOStartX(Rect originalRect, int nestLevel)
        {
            return GetStartX(originalRect, nestLevel) + ROW_HEIGHT + (ROW_HEIGHT / 2.0f) - 1;
        }

        public static float GetGOIconStartX(Rect originalRect, int nestLevel)
        {
            float goStart = GetGOStartX(originalRect, nestLevel);
            return goStart + 16;
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
            color.a = data.tree.lineAlpha;

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
            color.a = data.tree.lineAlpha;

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
            color.a = data.tree.lineAlpha;

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

                Component component = go.GetComponent(t);
                if (component == null)
                    continue;

                var stackTrace = entry.Value;
                var c = GetLogIcon(type, stackTrace);

                var x = GetNameX(selectionRect, currentItem.nestingLevel) + (ROW_HEIGHT * iconLevel);

                Rect rect = new Rect(x, selectionRect.yMin, ROW_HEIGHT, ROW_HEIGHT);

                HierarchyUtil.DrawTextureTooltip(rect, c.image, stackTrace);

                return true;
            }

            return false;
        }

        public static Texture2D GradientImage(Color start, bool up = false)
        {
            const float width = 16.0f;
            const float height = 16.0f;

            Texture2D texture = new Texture2D((int)width, (int)height, TextureFormat.ARGB32, false);
            texture.alphaIsTransparency = true;
            texture.filterMode = FilterMode.Bilinear;
            texture.wrapMode = TextureWrapMode.Clamp;
            texture.hideFlags = HideFlags.HideAndDontSave;

            Gradient gradient = new Gradient();

            GradientColorKey[] DARKNESS_COLOR_KEY = {
                new GradientColorKey(start, 0),
                new GradientColorKey(start, 1),
            };
            GradientAlphaKey[] DARKNESS_ALPHA_KEY = {
                new GradientAlphaKey(start.a, 0),
                new GradientAlphaKey(0, data.tree.gradientLength),
            };

            gradient.SetKeys(DARKNESS_COLOR_KEY, DARKNESS_ALPHA_KEY);

            if (up)
            {
                float yStep = 1.0f / height;

                for (int y = 0; y < Mathf.CeilToInt(height); ++y)
                {
                    Color color = gradient.Evaluate(y * yStep);

                    for (int x = 0; x < Mathf.CeilToInt(width); ++x)
                    {
                        texture.SetPixel(Mathf.CeilToInt(x), Mathf.CeilToInt(y), color);
                    }
                }
            }
            else
            {
                float inv = 1f / (width - 1);

                for (int y = 0; y < height; ++y)
                {
                    for (int x = 0; x < width; ++x)
                    {
                        var t = x * inv;
                        Color col = gradient.Evaluate(t);
                        texture.SetPixel(x, y, col);
                    }
                }
            }

            texture.Apply();

            return texture;
        }

        /// <summary>
        /// Draw component texture with flag `enabled`.
        /// </summary>
        private static void DrawEnableComponentTexture(Type t, Rect rect, Texture image, bool enabled)
        {
            // If we are drawing behbaviour, we draw with alpha channel weather
            // the component is enabled.
            if (enabled)
                HierarchyUtil.DrawTextureTooltip(rect, image, t.Name);
            else
                HierarchyUtil.DrawTextureTooltip(rect, image, t.Name, data.components.disableAlpa);
        }

        public static void DrawComponent(Type t, GameObject go, Rect rect, Texture image)
        {
            if (t.IsSubclassOf(typeof(Behaviour)))
            {
                var comp = go.GetComponent(t) as Behaviour;
                DrawEnableComponentTexture(t, rect, image, comp.enabled);
            }
            else if (t.IsSubclassOf(typeof(Collider)))
            {
                var comp = go.GetComponent(t) as Collider;
                DrawEnableComponentTexture(t, rect, image, comp.enabled);
            }
            else if (t.IsSubclassOf(typeof(Renderer)))
            {
                var comp = go.GetComponent(t) as Renderer;
                DrawEnableComponentTexture(t, rect, image, comp.enabled);
            }
            else
            {
                HierarchyUtil.DrawTextureTooltip(rect, image, t.Name);
            }
        }
    }
}
#endif
