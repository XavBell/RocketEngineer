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

    public List<string> enginesBuilt = new List<string>();
    public List<string> rocketsBuilt = new List<string>();
    public List<string> tanksBuilt= new List<string>();

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

    //Container
    public List<Guid> containerGuid = new List<Guid>();
    public List<float> quantity = new List<float>();
    public List<bool> coolerActive = new List<bool>();
    public List<float> targetTemp = new List<float>();
    public List<float> internalTemp = new List<float>();
    public List<string> fuelType = new List<string>();

    public List<List<Guid>> containerGuids = new List<List<Guid>>();

    public List<Guid> staticFireFuelGuid = new List<Guid>();
    public List<Guid> staticFireOxidizerGuid = new List<Guid>();

    public List<Guid> standGuid = new List<Guid>();

    public List<Guid> originGuid = new List<Guid>();
    public List<Guid> destinationGuid = new List<Guid>();


    //Moon
    public KeplerParams moonK;
    public KeplerParams earthK;
    
    

    public List<int> childrenNumber = new List<int>();
    public List<string> types = new List<string>();

    public List<RocketData> rockets = new List<RocketData>();
}

