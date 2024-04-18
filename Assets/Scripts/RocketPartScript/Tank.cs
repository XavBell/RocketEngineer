using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tank : RocketPart
{
    public float _volume{get; set;}
    public float x_scale;
    public float y_scale;
    public string tankMaterial;
    public string propellantCategory;
    public float conductivity = 10;
    public bool tested = false;

    void Start()
    {
        container container = this.GetComponent<container>();
        container.tankHeight = y_scale;
        container.tankVolume = _volume;
        container.tankThermalConductivity = conductivity;
        container.tankSurfaceArea = x_scale*Mathf.PI*2 + (x_scale*Mathf.PI*y_scale);
        container.tested = tested;

    }
}
