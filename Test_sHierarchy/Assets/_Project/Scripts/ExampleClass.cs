using UnityEngine;
using UnityEditor;
using System;

// Create an editor window which can display a chosen GameObject.
// Use OnInteractivePreviewGUI to display the GameObject and
// allow it to be interactive.

public class ExampleClass : EditorWindow
{
    GameObject gameObject;
    Editor gameObjectEditor;
    Texture2D background = null;

    [MenuItem("Example/GameObject Editor")]
    static void ShowWindow()
    {
        ExampleClass window = (ExampleClass)GetWindow(typeof(ExampleClass));
        window.Show();
    }

    void OnGUI()
    {
        if (gameObject != Selection.activeGameObject)
        {
            if (gameObjectEditor != null) DestroyImmediate(gameObjectEditor);
            gameObject = Selection.activeGameObject;
        }

        GUIStyle bgColor = new GUIStyle();
        bgColor.normal.background = background;

        if (gameObject != null)
        {
            if (gameObjectEditor == null)
                gameObjectEditor = Editor.CreateEditor(gameObject);

            gameObjectEditor.OnInteractivePreviewGUI(ScreenRect(), bgColor);
        }
    }

    public void OnInspectorUpdate()
    {
        Repaint();
    }

    private Rect ScreenRect()
    {
        const float offset = 0.9f;
        float shortest = Mathf.Min(Screen.width, Screen.height) * offset;
        return GUILayoutUtility.GetRect(0, 0, shortest, shortest);
    }
}
