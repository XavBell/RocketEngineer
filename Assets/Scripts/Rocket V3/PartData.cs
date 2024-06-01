using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;
using System;

public class PartData
{
    public string partType;
    public Guid guid;
    public float x_pos;
    public float y_pos;
    public float z_rot;

    //Used to load correct prefab profile saved in json
    public string fileName;

    //**Put variable necessary for parts here**//
    //Decouplers
    public bool detachFromParent;
    //Engines

    //Tanks
    public Guid lineGuid;

    //Capsules


    //Used to store children parts
    public List<PartData> children = new List<PartData>();
}
