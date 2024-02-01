using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Nozzle", menuName = "ScriptableObjects/Nozzle")]
public class Nozzle:ScriptableObject
{
    public string nozzleName = "default";
    public float thrustModifier = 0f;
    public float rateModifier = 1f;
    public float mass = 0f;
    public float cost = 0f;

    public Sprite sprite;
}
