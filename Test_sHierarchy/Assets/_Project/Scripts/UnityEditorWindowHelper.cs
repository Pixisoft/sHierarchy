using UnityEngine;
using UnityEditor;
using System;

public enum WindowType
{
    Game,
    Scene,
    Hierarchy,
    Console,
    Inspector,
}
public static class UnityEditorWindowHelper
{
    public static EditorWindow GetWindow(WindowType windowType)
    {
        var assembly = typeof(UnityEditor.EditorWindow).Assembly;
        var type = assembly.GetType(Convert(windowType));
        return EditorWindow.GetWindow(type);
    }

    private static string Convert(WindowType windowType)
    {
        string name = string.Empty;
        switch (windowType)
        {
            case WindowType.Game:
                name = "UnityEditor.GameView";
                break;
            case WindowType.Scene:
                name = "UnityEditor.SceneView";
                break;
            case WindowType.Hierarchy:
                name = "UnityEditor.SceneHierarchyWindow";
                break;
            case WindowType.Console:
                name = "UnityEditor.ConsoleWindow";
                break;
            case WindowType.Inspector:
                name = "UnityEditor.InspectorWindow";
                break;
            default:
                throw new NotImplementedException();
        }

        return name;
    }

}