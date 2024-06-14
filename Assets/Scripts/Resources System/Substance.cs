using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Substance", menuName = "ScriptableObjects/Material/Substance")]
public class Substance : ScriptableObject
{
    [Header("Substance General Properties")]
    public string Name;
    public Sprite Icon;
    public float Density; //in kg/m3
    public float LiquidTemperature; //up to 424
    public float GaseousTemperature; //and more
    public float SolidTemperature; //and below
    public float MolarMass; //g/mol
    public float SpecificHeatCapacity;
    
    public bool hasSolidState;
    public bool hasLiquidState;
    public bool hasGaseousState;

    [Header("Substance Possible Actions")]
    public bool cargoStorage;
    public bool liquidStorage;
    public bool tankStorage;

    
}
