/**
 * $File: MyPreview.cs $
 * $Date: #CREATIONDATE# $
 * $Revision: $
 * $Creator: Jen-Chieh Shen $
 * $Notice: See LICENSE.txt for modification and distribution information 
 *	                 Copyright © #CREATEYEAR# by Shen, Jen-Chieh $
 */
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPreview(typeof(GameObject))]
public class MyPreview : ObjectPreview
{
    GameObject gameObject;
    Editor gameObjectEditor;
    Texture2D background = null;

    public override bool HasPreviewGUI()
    {
        return false;
    }

    public override void OnPreviewGUI(Rect r, GUIStyle background)
    {
        if (gameObject != Selection.activeGameObject)
        {
            if (gameObjectEditor != null) UnityEngine.Object.DestroyImmediate(gameObjectEditor);
            gameObject = Selection.activeGameObject;
        }

        GUIStyle bgColor = new GUIStyle();

        if (gameObject != null)
        {
            if (gameObjectEditor == null)
                gameObjectEditor = Editor.CreateEditor(gameObject);

            gameObjectEditor.OnInteractivePreviewGUI(GUILayoutUtility.GetLastRect(), EditorStyles.whiteLabel);
        }
    }

    private Rect ScreenRect()
    {
        const float offset = 0.9f;
        float shortest = Mathf.Min(Screen.width, Screen.height) * offset;
        return GUILayoutUtility.GetRect(0, 0, shortest, shortest);
    }
}
