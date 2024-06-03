using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Localization.Plugins.XLIFF.V12;

[CustomEditor(typeof(RocketAssemblerManager))]
public class EditorButton : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        RocketAssemblerManager rocketAssemblerManager  = (RocketAssemblerManager)target;
        if(GUILayout.Button("Add Line"))
        {
            rocketAssemblerManager.AddLine();
        }

        if(GUILayout.Button("Set Line"))
        {
            //rocketAssemblerManager.SetLine(rocketAssemblerManager.lineName, rocketAssemblerManager.tankComponent);
        }
    } 
}
