using System.Numerics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Engine : RocketPart
{ 
    public float _thrust;
    public float _rate;
    public float _tvcSpeed;
    public float _maxAngle;

    public string _tvcName;
    public string _nozzleName;
    public string _pumpName;
    public string _turbineName;
    public int stageNumber;
    public bool active = false;

    public void activate()
    {
        active = true;
    }
}
