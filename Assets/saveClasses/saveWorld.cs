using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class saveWorld
{
    public string version = "0.0.4";
    public bool previouslyLoaded = false;

    public float cameraLocX;
    public float cameraLocY;
    public float cameraLocZ;

    public float cameraRotX;
    public float cameraRotY;
    public float cameraRotZ;

    //SolarSystem
    //Earth
    public float earthLocX;
    public float earthLocY;
    public float earthLocZ;

    public float earthRotX;
    public float earthRotY;
    public float earthRotZ;

    //Earth childs
    public List<string> buildingTypes = new List<string>();
    public List<float> buildingLocX = new List<float>();
    public List<float> buildingLocY = new List<float>();
    public List<float> buildingLocZ = new List<float>();

    public List<float> buildingScaleX = new List<float>();
    public List<float> buildingScaleY = new List<float>();

    public List<float> buildingRotX = new List<float>();
    public List<float> buildingRotY = new List<float>();
    public List<float> buildingRotZ = new List<float>();

    public List<float> inputLocX = new List<float>();
    public List<float> inputLocY = new List<float>();
    public List<float> inputLocZ = new List<float>();

    public List<float> outputLocX = new List<float>();
    public List<float> outputLocY = new List<float>();
    public List<float> outputLocZ = new List<float>();    

    //Moon
    public float moonLocX;
    public float moonLocY;
    public float moonLocZ;



    public List<int> childrenNumber = new List<int>();
    public List<string> types = new List<string>();

    //Capsule
    public List<float> capsuleScaleX = new List<float>();
    public List<float> capsuleScaleY = new List<float>();
    public List<float> capsuleScaleZ = new List<float>();

    public List<float> capsuleLocX = new List<float>();
    public List<float> capsuleLocY = new List<float>();
    public List<float> capsuleLocZ = new List<float>();

    public List<float> capsuleRotX = new List<float>();
    public List<float> capsuleRotY = new List<float>();
    public List<float> capsuleRotZ = new List<float>();

    public List<float> capsuleSpeedX = new List<float>();
    public List<float> capsuleSpeedY = new List<float>();
    

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

    //Engine values
    public List<float> engineFuel = new List<float>();
    public List<float> engineRate = new List<float>();
    public List<float> engineMaxThrust = new List<float>();

    //Decoupler values
    public List<float> decouplerLocX = new List<float>();
    public List<float> decouplerLocY = new List<float>();
    public List<float> decouplerLocZ = new List<float>();

    //Rocket values for state
    public List<float> rocketMass = new List<float>();
    public List<float> currentFuel = new List<float>();
    public List<float> maxFuel = new List<float>();
    public List<bool> stageUpdated = new List<bool>();
}

