using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Satellite : RocketPart
{
    public UnityEngine.Vector2 _size{get; private set;}
    public bool chuteDeployed;
    public GameObject chute;
}
