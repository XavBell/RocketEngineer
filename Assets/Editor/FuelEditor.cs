using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(FuelConsumerComponent))]
public class FuelEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        FuelConsumerComponent fuelConsumerComponent  = (FuelConsumerComponent)target;
        if(GUILayout.Button("Add tanks"))
        {
            fuelConsumerComponent.FindTanks();
        }
    }
}
