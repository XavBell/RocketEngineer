using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class saveWorld
{
    public string version = "0.0.4";
    public bool previouslyLoaded = false;
    public double time;

    public int IDMax;

    public float cameraLocX;
    public float cameraLocY;
    public float cameraLocZ;

    public float cameraRotX;
    public float cameraRotY;
    public float cameraRotZ;

    public List<string> partType = new List<string>();
    public List<string> partName = new List<string>();
    public List<int> count = new List<int>();

    public List<string> turbineUnlocked = new List<string>();
    public List<string> pumpUnlocked = new List<string>();
    public List<string> nozzleUnlocked = new List<string>();
    public List<string> tvcUnlocked = new List<string>();

    public List<string> tankMaterialUnlocked = new List<string>();

    public float maxTankBuildSizeX;
    public float maxTankBuildSizeY;

    public float maxRocketBuildSizeX;
    public float maxRocketBuildSizeY;
    public List<string> nodeUnlocked = new List<string>();

    public float nPoints;

    //SolarSystem
    //Earth
    public float earthLocX;
    public float earthLocY;
    public float earthLocZ;

    public float earthVX;
    public float earthVY;

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

    public List<Guid> containerGuid = new List<Guid>();

    public List<Guid> padFuelGuid = new List<Guid>();
    public List<Guid> padOxidizerGuid = new List<Guid>();

    public List<Guid> staticFireFuelGuid = new List<Guid>();
    public List<Guid> staticFireOxidizerGuid = new List<Guid>();

    public List<Guid> standGuid = new List<Guid>();

    public List<Guid> originGuid = new List<Guid>();
    public List<Guid> destinationGuid = new List<Guid>();


    //Moon
    public float moonLocX;
    public float moonLocY;
    public float moonLocZ;
    public float moonVX;
    public float moonVY;

    public List<int> childrenNumber = new List<int>();
    public List<string> types = new List<string>();

    public List<saveWorldRocket> rockets = new List<saveWorldRocket>();
}

