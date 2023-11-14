using System.Security.AccessControl;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class savecraft
{
    //Base values for all parts
    public List<System.Guid> PartsID = new List<System.Guid>();
    public System.Guid coreID;
    public List<int> StageNumber = new List<int>();
    public List<string> partType = new List<string>();
    public List<float> x_pos = new List<float>();
    public List<float> y_pos = new List<float>();
    public List<float> z_pos = new List<float>();
}
