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
    public float earthSOI = 9000000;
    public float earthMass = 0f;
    public float earthAlt = 1000;
    public float earthDensitySlvl = 100f;

    //Moon Values
    public float moongSlvl = 16.0f;
    public float moonRadius = 17000.0f;
    public float moonMass = 0f;
    public float moonSOI = 600000f;

    //Sun Values
    public float sunSlvl = 274.4f;
    public float sunRadius = 6957.0E6f;
    public float sunSOI = 10000000;
    public float sunMass = 0f;
    public bool needUpdate = false;

    void LateUpdate()
    {
        

        //Physics.SyncTransforms();

    }

    void FixedUpdate()
    {
        
        
    }
}
