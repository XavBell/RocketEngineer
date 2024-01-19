using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SunScript : MonoBehaviour
{
    public SolarSystemManager SolarSystemManager;
    public float sunMass;
    public float gSlvl;
    public float G;
    public float sunRadius;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitializeSun()
    {
        SetSunMass();
        GetValues();
    }

    void SetSunMass()
    {
        GetValues();
        sunMass = gSlvl*(sunRadius*sunRadius)/G;
        SolarSystemManager.sunMass = sunMass;
    }

    void GetValues()
    {
        G = SolarSystemManager.G;
        sunMass = SolarSystemManager.sunMass;
        sunRadius = SolarSystemManager.sunRadius;
        gSlvl = SolarSystemManager.sunSlvl;
    }
}
