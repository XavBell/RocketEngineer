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
    public List<float> mass = new List<float>();
    public List<float> x_pos = new List<float>();
    public List<float> y_pos = new List<float>();
    public List<float> z_pos = new List<float>();
    public List<float> z_rot = new List<float>();

    //AttachPoints
    public List<System.Guid> attachedTop = new List<System.Guid>();
    public List<System.Guid> attachedBottom = new List<System.Guid>();
    public List<System.Guid> attachedRight = new List<System.Guid>();
    public List<System.Guid> attachedLeft = new List<System.Guid>();


    //Tank variables
    public List<string> tankName = new List<string>();
    public List<float> tankCost = new List<float>();
    public List<float> x_scale = new List<float>();
    public List<float> y_scale = new List<float>();
    public List<string> propellantType = new List<string>();
    public List<string> tankMaterial = new List<string>();
    public List<float> volume = new List<float>();

    //Engine variables
    public List<string> engineName = new List<string>();
    public List<float> engineCost = new List<float>();
    public List<float> thrust = new List<float>();
    public List<float> flowRate = new List<float>();
    public List<float> maxTime = new List<float>();
    public List<float> reliability = new List<float>();
}
