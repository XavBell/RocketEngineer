using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class saveWorld
{
    public List<int> childrenNumber = new List<int>();
    public List<string> types = new List<string>();

    //Capsule
    public List<float> capsuleScaleX = new List<float>();
    public List<float> capsuleScaleY = new List<float>();
    public List<float> capsuleScaleZ = new List<float>();

    public List<float> capsuleLocX = new List<float>();
    public List<float> capsuleLocY = new List<float>();
    public List<float> capsuleLocZ = new List<float>();

    //Tank
    public List<float> tankScaleX = new List<float>();
    public List<float> tankScaleY = new List<float>();
    public List<float> tankScaleZ = new List<float>();

    public List<float> tankLocX = new List<float>();
    public List<float> tankLocY = new List<float>();
    public List<float> tankLocZ = new List<float>();

    //Tank attach position 
    public List<float> tankAttachTopLocX = new List<float>();
    public List<float> tankAttachTopLocY = new List<float>();
    public List<float> tankAttachTopLocZ = new List<float>();

    public List<float> tankAttachBottomLocX = new List<float>();
    public List<float> tankAttachBottomLocY = new List<float>();
    public List<float> tankAttachBottomLocZ = new List<float>();

    //Engine
    public List<float> engineScaleX = new List<float>();
    public List<float> engineScaleY = new List<float>();
    public List<float> engineScaleZ = new List<float>();

    public List<float> engineLocX = new List<float>();
    public List<float> engineLocY = new List<float>();
    public List<float> engineLocZ = new List<float>();

    //Engine children values
    public List<float> nozzleExitSizeX = new List<float>();
    public List<float> nozzleExitSizeY = new List<float>();
    public List<float> nozzleExitLocY = new List<float>();
    public List<float> nozzleEndSizeX = new List<float>();
    public List<float> turbopumpSizeX = new List<float>();

    //Engine attach position
    public List<float> engineAttachTopLocX = new List<float>();
    public List<float> engineAttachTopLocY = new List<float>();
    public List<float> engineAttachTopLocZ = new List<float>();

    public List<float> engineAttachBottomLocX = new List<float>();
    public List<float> engineAttachBottomLocY = new List<float>();
    public List<float> engineAttachBottomLocZ = new List<float>();

    //Decoupler values
    public List<float> decouplerLocX = new List<float>();
    public List<float> decouplerLocY = new List<float>();
    public List<float> decouplerLocZ = new List<float>();




    //Rocket values for state
    public List<float> rocketMass = new List<float>();
    public List<float> currentFuel = new List<float>();
    public List<float> maxFuel = new List<float>();
}
