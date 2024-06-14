using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Substance", menuName = "ScriptableObjects/Substance")]
public class Substance : ScriptableObject
{
    public string Name;
    public float Density; //in kg/m3
    public float LiquidTemperature; //up to 424
    public float GaseousTemperature; //and more
    public float SolidTemperature; //and below
    public float MolarMass; //g/mol
    public float SpecificHeatCapacity;
    
}
