using System.Xml;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;
using System.Runtime.Serialization;
using System.Xml.Linq;
using System.Text;
using Newtonsoft.Json;

public class WorldSaveManager : MonoBehaviour
{
    public GameObject capsulePrefab;
    public GameObject tankPrefab;
    public GameObject enginePrefab;
    public GameObject decouplerPrefab;

    public GameObject designerPrefab;
    public GameObject fuelTankPrefab;
    public GameObject launchPadPrefab;
    public GameObject VABPrefab;
    public GameObject commandCenterPrefab;

    public savePath savePathRef = new savePath();

    public GameObject earth;
    public GameObject moon;

    public GameObject worldCamera;

    public bool loaded = false;

    public GameObject MasterManager;
    public GameObject BuildingManager;
    public launchsiteManager launchsiteManager;

    // Start is called before the first frame update
    void Start()
    {
        MasterManager = GameObject.FindGameObjectWithTag("MasterManager");
        loadWorld(); 
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.J))
        {
            saveTheWorld();
        }

        if(Input.GetKey(KeyCode.H) && loaded == false)
        {
            loadWorld();
        }
    }


    public void saveTheWorld()
    {
        saveWorld saveWorld = new saveWorld();

        saveWorld.cameraLocX = worldCamera.transform.localPosition.x;
        saveWorld.cameraLocY = worldCamera.transform.localPosition.y;
        saveWorld.cameraLocZ = worldCamera.transform.localPosition.z;

        saveWorld.cameraRotX = worldCamera.transform.eulerAngles.x;
        saveWorld.cameraRotY = worldCamera.transform.eulerAngles.y;
        saveWorld.cameraRotZ = worldCamera.transform.eulerAngles.z;

        saveWorld.earthLocX = earth.transform.localPosition.x;
        saveWorld.earthLocY = earth.transform.localPosition.y;
        saveWorld.earthLocZ = earth.transform.localPosition.z;

        saveWorld.earthRotX = earth.transform.eulerAngles.x;
        saveWorld.earthRotY = earth.transform.eulerAngles.y;
        saveWorld.earthRotZ = earth.transform.eulerAngles.z;

        saveWorld.moonLocX = moon.transform.localPosition.x;
        saveWorld.moonLocY = moon.transform.localPosition.y;
        saveWorld.moonLocZ = moon.transform.localPosition.z;

        saveWorld.IDMax = BuildingManager.GetComponent<BuildingManager>().IDMax;

        saveWorld.previouslyLoaded = true;

        GameObject[] buildings = GameObject.FindGameObjectsWithTag("building");
        foreach(GameObject building in buildings)
        {
            saveWorld.buildingTypes.Add(building.GetComponent<buildingType>().type);
            saveWorld.buildingIDs.Add(building.GetComponent<buildingType>().buildingID);
            saveWorld.buildingLocX.Add(building.transform.localPosition.x);
            saveWorld.buildingLocY.Add(building.transform.localPosition.y);
            saveWorld.buildingLocZ.Add(building.transform.localPosition.z);

            saveWorld.buildingRotX.Add(building.transform.eulerAngles.x);
            saveWorld.buildingRotY.Add(building.transform.eulerAngles.y);
            saveWorld.buildingRotZ.Add(building.transform.eulerAngles.z);

            if(building.GetComponent<buildingType>().type == "designer")
            {
                saveWorld.inputIDs.Add(0);
                saveWorld.outputIDs.Add(0);
            }

            if(building.GetComponent<buildingType>().type == "commandCenter")
            {
                saveWorld.inputIDs.Add(0);
                saveWorld.outputIDs.Add(0);
            }

            if(building.GetComponent<buildingType>().type == "VAB")
            {
                saveWorld.inputIDs.Add(0);
                saveWorld.outputIDs.Add(0);
            }

            if(building.GetComponent<buildingType>().type == "GSEtank")
            {
                saveWorld.inputIDs.Add(building.GetComponent<outputInputManager>().inputParentID);
                saveWorld.outputIDs.Add(building.GetComponent<outputInputManager>().outputParentID);
            }
            
            if(building.GetComponent<buildingType>().type == "launchPad")
            {
                saveWorld.inputIDs.Add(building.GetComponent<outputInputManager>().inputParentID);
                saveWorld.outputIDs.Add(building.GetComponent<outputInputManager>().outputParentID);
            }
        }

       

        var jsonString = JsonConvert.SerializeObject(saveWorld);
        System.IO.File.WriteAllText(MasterManager.GetComponent<MasterManager>().worldPath, jsonString);
    }

    public void loadWorld()
    {
        saveWorld saveWorld = new saveWorld();
        var jsonString = JsonConvert.SerializeObject(saveWorld);
        jsonString = File.ReadAllText(MasterManager.GetComponent<MasterManager>().worldPath);
        saveWorld loadedWorld = JsonConvert.DeserializeObject<saveWorld>(jsonString);
        FileVersionManger version = new FileVersionManger();
        if(loadedWorld.version == version.currentVersion){
        int alreadyUsed = 0;
        int capsuleID = 0;

        int engineCount = 0;
        int tankCount = 0;
        int decouplerCount = 0;

        if(loadedWorld.previouslyLoaded == true)
        {
            earth.transform.localPosition = new Vector3(loadedWorld.earthLocX, loadedWorld.earthLocY, loadedWorld.earthLocZ);
            worldCamera.transform.localPosition = new Vector3(loadedWorld.cameraLocX, loadedWorld.cameraLocY, loadedWorld.cameraLocZ);
            earth.transform.eulerAngles = new Vector3(loadedWorld.earthRotX, loadedWorld.earthRotY, loadedWorld.earthRotZ);
            worldCamera.transform.eulerAngles = new Vector3(loadedWorld.cameraRotX, loadedWorld.cameraRotY, loadedWorld.cameraRotZ);
            moon.transform.localPosition = new Vector3(loadedWorld.moonLocX, loadedWorld.moonLocY, loadedWorld.moonLocZ);
        }
        int count = 0;

        BuildingManager.GetComponent<BuildingManager>().IDMax = loadedWorld.IDMax;

        foreach(string buildingType in loadedWorld.buildingTypes)
        {
            Vector3 position = new Vector3(loadedWorld.buildingLocX[count], loadedWorld.buildingLocY[count], loadedWorld.buildingLocZ[count]);
            Vector3 rotation = new Vector3(loadedWorld.buildingRotX[count], loadedWorld.buildingRotY[count], loadedWorld.buildingRotZ[count]);

            if(buildingType == "designer")
            {
                GameObject current = Instantiate(designerPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                current.transform.SetParent(earth.transform);
                current.transform.localPosition = position;
                current.transform.eulerAngles = rotation;
                current.GetComponent<buildingType>().buildingID = loadedWorld.buildingIDs[count];
                launchsiteManager.designer = current;
            }

            if(buildingType == "GSEtank")
            {
                GameObject current = Instantiate(fuelTankPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                current.transform.SetParent(earth.transform);
                current.transform.localPosition = position;
                current.transform.eulerAngles = rotation;
                current.GetComponent<buildingType>().buildingID = loadedWorld.buildingIDs[count];
                current.GetComponent<outputInputManager>().inputParentID = loadedWorld.inputIDs[count];
                current.GetComponent<outputInputManager>().outputParentID = loadedWorld.outputIDs[count];
            }

            if(buildingType == "launchPad")
            {
                GameObject current = Instantiate(launchPadPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                current.transform.SetParent(earth.transform);
                current.transform.localPosition = position;
                current.transform.eulerAngles = rotation;
                current.GetComponent<buildingType>().buildingID = loadedWorld.buildingIDs[count];
                current.GetComponent<outputInputManager>().inputParentID = loadedWorld.inputIDs[count];
                current.GetComponent<outputInputManager>().outputParentID = loadedWorld.outputIDs[count];
            }

            if(buildingType == "VAB")
            {
                GameObject current = Instantiate(VABPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                current.transform.SetParent(earth.transform);
                current.transform.localPosition = position;
                current.transform.eulerAngles = rotation;
                current.GetComponent<buildingType>().buildingID = loadedWorld.buildingIDs[count];
            }

            if(buildingType == "commandCenter")
            {
                GameObject current = Instantiate(commandCenterPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                current.transform.SetParent(earth.transform);
                current.transform.localPosition = position;
                current.transform.eulerAngles = rotation;
                current.GetComponent<buildingType>().buildingID = loadedWorld.buildingIDs[count];
                launchsiteManager.commandCenter = current;
            }
            count++;
        }

        launchsiteManager.updateVisibleButtons();

        

        
        earth.GetComponent<EarthScript>().InitializeEarth();
        moon.GetComponent<MoonScript>().InitializeMoon();
        
        loaded = true;
        } else if(loadedWorld.version != version.currentVersion)
        {
           UnityEngine.Debug.Log("File version not compatible");
        }
    }

    public void setPosition(float x, float y, float z, GameObject current)
    {
        current.transform.localPosition = new Vector3(x, y, z);
    }


}
