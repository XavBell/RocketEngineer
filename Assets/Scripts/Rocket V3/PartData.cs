using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.Rendering;

public class PartData
{
    public string partType;
    public float x_pos;
    public float y_pos;
    public float z_rot;

    //Used to load correct prefab profile saved in json
    public string fileName;

    //Used for decouplers
    public bool detachFromParent;

    public List<PartData> children = new List<PartData>();
}
