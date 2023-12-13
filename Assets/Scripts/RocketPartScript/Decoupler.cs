using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Decoupler : RocketPart
{
    public bool activated = false;
    public void activate()
    {
        activated = true;
    }
   
}
