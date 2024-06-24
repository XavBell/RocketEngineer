using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Propellants", menuName = "ScriptableObjects/Propellants")]
public class Propellants : ScriptableObject
{
    public string propellantName;
    public Substance oxidizer;
    public Substance fuel;
    public float oxidizerToFuelRatio;
}
