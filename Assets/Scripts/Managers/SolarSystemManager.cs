using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolarSystemManager : MonoBehaviour
{
    //Universe Values
    public float G = 0.000_000_000_066_7f; //Gravitational constant

    //Earth Values
    public float earthgSlvl = 9.8f;
    public float earthRadius = 63710.0f;
    public float earthMass = 0f;

    //Moon Values
    public float moongSlvl = 16.0f;
    public float moonRadius = 17000.0f;
    public float moonMass = 0f;
    public float moonSOI = 371805f;

    //Sun Values
    public float sunMass = 0f;
}
