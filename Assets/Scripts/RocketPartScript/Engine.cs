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
    //Reliability is between 0 and 1
    public float reliability;
    public float maxTime;
    public bool active = false;
    public bool operational = true;

    public void activate()
    {
        active = true;
    }

    public float CalculateOutputThrust(out bool withinRange)
    {
        float percentageOfThrust = Random.Range(reliability, 2-reliability);
        float outThrust = _thrust * percentageOfThrust;
        float minThrust = _thrust * 0.7f;
        float maxThrust = _thrust * 1.3f;
        if(outThrust < minThrust || outThrust > maxThrust)
        {
            withinRange = false;
        }else{
            withinRange = true;
        }
        return outThrust;
    }
}
