using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Turbine", menuName = "ScriptableObjects/Turbine")]
public class Turbine:ScriptableObject
{
    public string turbineName = "default";
    public float thrustModifier = 0f;
    public float rate = 0f;
    public float mass = 0f;
    public float cost = 0f;
}
