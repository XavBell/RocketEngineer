using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketData
{
    public string rocketName;
    public List<string> lineNames = new List<string>();
    public List<Guid> lineGuids = new List<Guid>();
    public PartData rootPart;
}
