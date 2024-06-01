using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PartData
{
    public string partType;
    public List<PartData> children = new List<PartData>();
    public float x_pos;
    public float y_pos;
}
