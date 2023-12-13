using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SolarSystemManager : MonoBehaviour
{
    //Universe Values
    public float G = 0.0000000000667f; //Gravitational constant

    //Earth Values
    public float earthgSlvl = 98.0f;
    public float earthRadius = 6371.0f;
    public float earthMass = 0f;

    //Moon Values
    public float moongSlvl = 16.0f;
    public float moonRadius = 1700.0f;
    public float moonMass = 0f;

    //Sun Values
    public float sunMass = 0f;
}
