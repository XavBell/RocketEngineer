using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Part : MonoBehaviour
{
    public string type = "null";
    public AttachPointScript attachTop;
    public AttachPointScript attachBottom;
    public float StageNumber;

    public float maxThrust = 0;
    public float mass = 0;
    public float fuel = 0;
    public float maxFuel = 0;
    public float rate = 0;
    public GameObject referenceDecoupler;

    public GameObject nozzleExit;
    public GameObject nozzleEnd;
    public GameObject turbopump;
    public GameObject tank;
    public string path;
    public string partName;

    public Vector2 boxSize;
    public Vector2 offsets;
}
