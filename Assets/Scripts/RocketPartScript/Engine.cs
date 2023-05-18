using System.Numerics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Engine : RocketPart
{ 
    public float _thrust{get; set;}
    public float _rate{get; set;}

    [field: SerializeField]public GameObject _turbopump{get; private set;}
    [field: SerializeField]public GameObject _nozzleStart{get; private set;}
    [field: SerializeField]public GameObject _nozzleEnd{get; private set;}
}
