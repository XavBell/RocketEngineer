using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class saveWorld
{
    public string version = "0.0.4";
    public bool previouslyLoaded = false;

    public int IDMax;

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

    //Buildings
    public List<string> buildingTypes = new List<string>();
    public List<int> buildingIDs = new List<int>();
    public List<float> buildingLocX = new List<float>();
    public List<float> buildingLocY = new List<float>();
    public List<float> buildingLocZ = new List<float>();

    public List<float> buildingScaleX = new List<float>();
    public List<float> buildingScaleY = new List<float>();

    public List<float> buildingRotX = new List<float>();
    public List<float> buildingRotY = new List<float>();
    public List<float> buildingRotZ = new List<float>();

    public List<int> outputIDs = new List<int>();
    public List<int> inputIDs = new List<int>();

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
}

