using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class savePart
{
    public string type;
    public string name;
    public string path;
    public Guid guid;
    public float mass;
    public float posX;
    public float posY;
    public float cost;

    //AttachPoints
    public Guid guidTop;
    public Guid guidBottom;
    public Guid guidLeft;
    public Guid guidRight;

    //ENGINE
    public float thrust;
    public float rate;
    public float tvcSpeed;
    public float maxAngle;
    public string tvcName;
    public string nozzleName;
    public string pumpName;
    public string turbineName;
    public float reliability;
    public float maxTime;
    public bool operational;
    public bool willFail;
    public float timeOfFail;
    public bool willExplode;

    //TANK
    public float _volume;
    public float x_scale;
    public float y_scale;
    public string tankMaterial;
    public string propellantCategory;
}
