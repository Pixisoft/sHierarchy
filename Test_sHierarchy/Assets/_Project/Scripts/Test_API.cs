/**
 * $File: Test_API.cs $
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

/// <summary>
/// 
/// </summary>
public class Test_API 
    : MonoBehaviour 
{
    /* Variables */

    /* Setter & Getter */

    /* Functions */

    private void Start()
    {
        Type inspectorType = Type.GetType("UnityEditor.InspectorWindow,UnityEditor.dll");
        EditorWindow window = EditorWindow.GetWindow<ExampleClass>(new Type[] { inspectorType });

        var hierarchy = UnityEditorWindowHelper.GetWindow(WindowType.Hierarchy);

        print(window.docked);

    }
}
