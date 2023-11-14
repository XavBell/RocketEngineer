using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stages
{
    [field: SerializeField] public List<System.Guid> PartsID = new List<System.Guid>();
    [field: SerializeField] public List<RocketPart> Parts = new List<RocketPart>();
}
