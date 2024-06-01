using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PartData
{
    public string partType;
    public float x_pos;
    public float y_pos;
    
    //Used to load correct prefab profile saved in json
    public string fileName;

    public List<PartData> children = new List<PartData>();
}
