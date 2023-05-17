using System.Numerics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Engine : RocketPart
{ 
    public float _thrust{get; set;}
    private UnityEngine.Vector2 _turbopumpSize{get; set;}
    private UnityEngine.Vector2 _nozzleStartSize{get; set;}
    private UnityEngine.Vector2 _nozzleEndSize{get; set;}
}
