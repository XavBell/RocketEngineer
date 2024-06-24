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
using UnityEngine.SceneManagement;

public class WorldSaveManager : MonoBehaviour
{
    public GameObject capsulePrefab;
    public GameObject tankPrefab;
    public GameObject enginePrefab;
    public GameObject decouplerPrefab;

    public GameObject designerPrefab;
    public GameObject fuelTankPrefab;
    public GameObject launchPadPrefab;
    public GameObject standTankPrefab;
    public GameObject staticFireStandPrefab;
    public GameObject VABPrefab;
    public GameObject commandCenterPrefab;

    public savePath savePathRef = new savePath();

    public GameObject earth;
    public GameObject moon;
    public GameObject sun;

    public GameObject worldCamera;

    public bool loaded = false;

    public GameObject MasterManager;
    public GameObject BuildingManager;
    public launchsiteManager launchsiteManager;

    public GameObject Satellite;
    public GameObject Engine;
    public GameObject Decoupler;
    public GameObject Tank;
    public List<RocketController> rocketToExclude = new List<RocketController>();

    public List<Nozzle> nozzleReferences;
    public List<Turbine> turbineReferences;
    public List<Pump> pumpReferences;
    public List<TVC> tvcReferences;
    public saveWorld worldToLoad = null;
    public bool rocketloaded = false;
    public bool pendingStopDelayed = false;

    public Substance kerosene;
    public Substance LOX;


    // Start is called before the first frame update
    void Start()
    {
        MasterManager = GameObject.FindGameObjectWithTag("MasterManager");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (loaded == true && rocketloaded == false)
        {
            
            rocketSaveManager rocketSaveManager = FindObjectOfType<rocketSaveManager>();
            foreach(RocketData rocketData in worldToLoad.rockets)
            {
                rocketSaveManager.loadWorldRocket(rocketData);
            }
            rocketloaded = true;
        }
        if (loaded == false)
        {
            loadWorld();
            loaded = true;
        }
        if (Input.GetKey(KeyCode.J))
        {
            saveTheWorld();
        }

        if (Input.GetKey(KeyCode.H) && loaded == false)
        {
            loadWorld();
        }

        if(pendingStopDelayed == true)
        {
            if(MasterManager.GetComponent<MasterManager>().ActiveRocket == null)
            {
                saveTheWorld();
                pendingStopDelayed = false;
            }
        }
    }


    public void saveTheWorld()
    {
        if (MasterManager.GetComponent<MasterManager>().ActiveRocket == null)
        {
            saveWorld saveWorld = new saveWorld();

            saveWorld.time = FindObjectOfType<TimeManager>().time;
            double time = FindObjectOfType<TimeManager>().time;

            saveWorld.cameraLocX = worldCamera.transform.localPosition.x;
            saveWorld.cameraLocY = worldCamera.transform.localPosition.y;
            saveWorld.cameraLocZ = worldCamera.transform.localPosition.z;

            saveWorld.cameraRotX = worldCamera.transform.eulerAngles.x;
            saveWorld.cameraRotY = worldCamera.transform.eulerAngles.y;
            saveWorld.cameraRotZ = worldCamera.transform.eulerAngles.z;

            saveWorld.earthK = earth.GetComponent<BodyPath>().KeplerParams;
            saveWorld.moonK = moon.GetComponent<BodyPath>().KeplerParams;
            
            saveWorld.nPoints = MasterManager.GetComponent<pointManager>().nPoints;
            saveWorld.enginesBuilt = MasterManager.GetComponent<MasterManager>().engines;
            saveWorld.tanksBuilt = MasterManager.GetComponent<MasterManager>().tanks;
            saveWorld.rocketsBuilt = MasterManager.GetComponent<MasterManager>().rockets;

            foreach (Turbine turbine in MasterManager.GetComponent<MasterManager>().turbineUnlocked)
            {
                saveWorld.turbineUnlocked.Add(turbine.turbineName);
            }

            foreach (Nozzle nozzle in MasterManager.GetComponent<MasterManager>().nozzleUnlocked)
            {
                saveWorld.nozzleUnlocked.Add(nozzle.nozzleName);
            }

            foreach (Pump pump in MasterManager.GetComponent<MasterManager>().pumpUnlocked)
            {
                saveWorld.pumpUnlocked.Add(pump.pumpName);
            }

            foreach (TVC tvc in MasterManager.GetComponent<MasterManager>().tvcUnlocked)
            {
                saveWorld.tvcUnlocked.Add(tvc.TVCName);
            }

            saveWorld.tankMaterialUnlocked = MasterManager.GetComponent<MasterManager>().tankMaterialUnlocked;

            saveWorld.maxRocketBuildSizeX = MasterManager.GetComponent<MasterManager>().maxRocketBuildSizeX;
            saveWorld.maxRocketBuildSizeY = MasterManager.GetComponent<MasterManager>().maxRocketBuildSizeY;
            saveWorld.maxTankBuildSizeX = MasterManager.GetComponent<MasterManager>().maxTankBuildSizeX;
            saveWorld.maxTankBuildSizeY = MasterManager.GetComponent<MasterManager>().maxTankBuildSizeY;
            saveWorld.nodeUnlocked = MasterManager.GetComponent<MasterManager>().nodeUnlocked;


            saveWorld.IDMax = BuildingManager.GetComponent<BuildingManager>().IDMax;

            saveWorld.previouslyLoaded = true;

            GameObject[] buildings = GameObject.FindGameObjectsWithTag("building");
            foreach (GameObject building in buildings)
            {
                saveWorld.buildingTypes.Add(building.GetComponent<buildingType>().type);
                saveWorld.buildingIDs.Add(building.GetComponent<buildingType>().buildingID);
                saveWorld.buildingLocX.Add(building.transform.localPosition.x);
                saveWorld.buildingLocY.Add(building.transform.localPosition.y);
                saveWorld.buildingLocZ.Add(building.transform.localPosition.z);

                saveWorld.buildingRotX.Add(building.transform.eulerAngles.x);
                saveWorld.buildingRotY.Add(building.transform.eulerAngles.y);
                saveWorld.buildingRotZ.Add(building.transform.eulerAngles.z);

                if (building.GetComponent<buildingType>().type == "launchPad")
                {
                    if (building.GetComponent<launchPadManager>().ConnectedRocket != null)
                    {
                        rocketToExclude.Add(building.GetComponent<launchPadManager>().ConnectedRocket.GetComponent<RocketController>());
                    }
                    saveWorld.containerGuids.Add(building.GetComponent<launchPadManager>().connectedContainersPerLine);
                }

                if (building.GetComponent<buildingType>().type == "standTank")
                {
                    saveWorld.standGuid.Add(building.GetComponent<flowControllerForTankStand>().originGuid);

                }
            }

            RocketController[] Rockets = FindObjectsOfType<RocketController>();
            foreach (RocketController rocket in Rockets)
            {
                if (!rocketToExclude.Contains(rocket))
                {
                    print("saving rocket");
                    RocketData rocketData = new RocketData();
                    rocketSaveManager rocketSaveManager = FindObjectOfType<rocketSaveManager>();
                    rocketSaveManager.saveWorldRocket(rocket, out rocketData);
                    saveWorld.rockets.Add(rocketData);
                }

            }

            container[] containers = FindObjectsOfType<container>();
            foreach (container container in containers)
            {
                if (container.GetComponent<buildingType>())
                {
                    saveWorld.containerGuid.Add(container.guid);
                    saveWorld.quantity.Add(container.moles);
                    if(container.GetComponent<cooler>() != null)
                    {
                        saveWorld.coolerActive.Add(container.GetComponent<cooler>().active);
                        saveWorld.targetTemp.Add(container.GetComponent<cooler>().targetTemperature);
                    }else{
                        saveWorld.coolerActive.Add(false);
                        saveWorld.targetTemp.Add(0);
                    }
                    saveWorld.internalTemp.Add(container.internalTemperature);
                    if(container.substance != null)
                    {
                        saveWorld.fuelType.Add(container.substance.Name);
                    }else{
                        saveWorld.fuelType.Add("none");
                    }
                }
            }

            flowController[] flowControllers = FindObjectsOfType<flowController>();
            foreach (flowController flowController in flowControllers)
            {
                if (flowController.GetComponent<buildingType>())
                {
                    saveWorld.destinationGuid.Add(flowController.destinationGuid);
                    saveWorld.originGuid.Add(flowController.originGuid);
                }
            }

            var jsonString = JsonConvert.SerializeObject(saveWorld);
            System.IO.File.WriteAllText(MasterManager.GetComponent<MasterManager>().worldPath, jsonString);
        }
        else{
            FindObjectOfType<StageEditor>().Stop();
            pendingStopDelayed = true;
        }
    }

    public void loadWorld()
    {
        saveWorld saveWorld = new saveWorld();
        var jsonString = JsonConvert.SerializeObject(saveWorld);
        jsonString = File.ReadAllText(MasterManager.GetComponent<MasterManager>().worldPath);
        saveWorld loadedWorld = JsonConvert.DeserializeObject<saveWorld>(jsonString);
        FileVersionManger version = new FileVersionManger();
        if (loadedWorld.version == version.currentVersion)
        {

            if (loadedWorld.previouslyLoaded == true)
            {
                FindObjectOfType<TimeManager>().time = loadedWorld.time;
                FindObjectOfType<TimeManager>().bypass = true;
                

                MasterManager.GetComponent<pointManager>().nPoints = loadedWorld.nPoints;
                MasterManager.GetComponent<MasterManager>().tanks = loadedWorld.tanksBuilt;
                MasterManager.GetComponent<MasterManager>().rockets = loadedWorld.rocketsBuilt;
                MasterManager.GetComponent<MasterManager>().engines = loadedWorld.enginesBuilt;

                if (loadedWorld.nodeUnlocked.Count > MasterManager.GetComponent<MasterManager>().nodeUnlocked.Count)
                {
                    foreach (Nozzle nozzle in nozzleReferences)
                    {
                        foreach (string nozzle1 in loadedWorld.nozzleUnlocked)
                        {
                            if (nozzle1 == nozzle.nozzleName && !MasterManager.GetComponent<MasterManager>().nozzleUnlocked.Contains(nozzle))
                            {
                                MasterManager.GetComponent<MasterManager>().nozzleUnlocked.Add(nozzle);
                            }
                        }
                    }

                    foreach (Turbine turbine in turbineReferences)
                    {
                        foreach (string turbine1 in loadedWorld.turbineUnlocked)
                        {
                            if (turbine1 == turbine.turbineName && !MasterManager.GetComponent<MasterManager>().turbineUnlocked.Contains(turbine))
                            {
                                MasterManager.GetComponent<MasterManager>().turbineUnlocked.Add(turbine);
                            }
                        }
                    }

                    foreach (Pump pump in pumpReferences)
                    {
                        foreach (string pump1 in loadedWorld.pumpUnlocked)
                        {
                            if (pump1 == pump.pumpName && !MasterManager.GetComponent<MasterManager>().pumpUnlocked.Contains(pump))
                            {
                                MasterManager.GetComponent<MasterManager>().pumpUnlocked.Add(pump);
                            }
                        }
                    }

                    foreach (TVC tvc in tvcReferences)
                    {
                        foreach (string tvc1 in loadedWorld.tvcUnlocked)
                        {
                            if (tvc1 == tvc.TVCName && !MasterManager.GetComponent<MasterManager>().tvcUnlocked.Contains(tvc))
                            {
                                MasterManager.GetComponent<MasterManager>().tvcUnlocked.Add(tvc);
                            }
                        }
                    }

                    MasterManager.GetComponent<MasterManager>().tankMaterialUnlocked = loadedWorld.tankMaterialUnlocked;

                    MasterManager.GetComponent<MasterManager>().maxRocketBuildSizeX = loadedWorld.maxRocketBuildSizeX;
                    MasterManager.GetComponent<MasterManager>().maxRocketBuildSizeY = loadedWorld.maxRocketBuildSizeY;
                    MasterManager.GetComponent<MasterManager>().maxTankBuildSizeX = loadedWorld.maxTankBuildSizeX;
                    MasterManager.GetComponent<MasterManager>().maxTankBuildSizeY = loadedWorld.maxTankBuildSizeY;
                    MasterManager.GetComponent<MasterManager>().nodeUnlocked = loadedWorld.nodeUnlocked;
                }

            }



            int count = 0;


            BuildingManager.GetComponent<BuildingManager>().IDMax = loadedWorld.IDMax;
            foreach (string buildingType in loadedWorld.buildingTypes)
            {
                Vector3 position = new Vector3(loadedWorld.buildingLocX[count], loadedWorld.buildingLocY[count], loadedWorld.buildingLocZ[count]);
                Vector3 rotation = new Vector3(loadedWorld.buildingRotX[count], loadedWorld.buildingRotY[count], loadedWorld.buildingRotZ[count]);

                if (buildingType == "designer")
                {
                    GameObject current = Instantiate(designerPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                    current.transform.SetParent(earth.transform);
                    current.transform.localPosition = position;
                    current.transform.eulerAngles = rotation;
                    current.GetComponent<buildingType>().buildingID = loadedWorld.buildingIDs[count];
                    launchsiteManager.designer = current;
                }

                if (buildingType == "GSEtank")
                {
                    GameObject current = Instantiate(fuelTankPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                    current.transform.SetParent(earth.transform);
                    current.transform.localPosition = position;
                    current.transform.eulerAngles = rotation;
                    current.GetComponent<buildingType>().buildingID = loadedWorld.buildingIDs[count];
                }

                if (buildingType == "launchPad")
                {
                    GameObject current = Instantiate(launchPadPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                    current.transform.SetParent(earth.transform);
                    current.transform.localPosition = position;
                    current.transform.eulerAngles = rotation;
                    current.GetComponent<buildingType>().buildingID = loadedWorld.buildingIDs[count];
                }

                if (buildingType == "VAB")
                {
                    GameObject current = Instantiate(VABPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                    current.transform.SetParent(earth.transform);
                    current.transform.localPosition = position;
                    current.transform.eulerAngles = rotation;
                    current.GetComponent<buildingType>().buildingID = loadedWorld.buildingIDs[count];
                }

                if (buildingType == "commandCenter")
                {
                    GameObject current = Instantiate(commandCenterPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                    current.transform.SetParent(earth.transform);
                    current.transform.localPosition = position;
                    current.transform.eulerAngles = rotation;
                    current.GetComponent<buildingType>().buildingID = loadedWorld.buildingIDs[count];
                    launchsiteManager.commandCenter = current;
                }

                if (buildingType == "standTank")
                {
                    GameObject current = Instantiate(standTankPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                    current.transform.SetParent(earth.transform);
                    current.transform.localPosition = position;
                    current.transform.eulerAngles = rotation;
                    current.GetComponent<buildingType>().buildingID = loadedWorld.buildingIDs[count];
                }

                if (buildingType == "staticFireStand")
                {
                    GameObject current = Instantiate(staticFireStandPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                    current.transform.SetParent(earth.transform);
                    current.transform.localPosition = position;
                    current.transform.eulerAngles = rotation;
                    current.GetComponent<buildingType>().buildingID = loadedWorld.buildingIDs[count];
                }
                count++;
            }

            int containerIndex = 0;
            container[] containers = FindObjectsOfType<container>();
            foreach (container container in containers)
            {
                if (container.GetComponent<buildingType>())
                {
                    container.guid = loadedWorld.containerGuid[containerIndex];
                    container.moles = loadedWorld.quantity[containerIndex];
                    if(container.GetComponent<cooler>() != null)
                    {
                        container.GetComponent<cooler>().active = loadedWorld.coolerActive[containerIndex];
                        container.GetComponent<cooler>().targetTemperature = loadedWorld.targetTemp[containerIndex];
                    }
                    container.internalTemperature = loadedWorld.internalTemp[containerIndex];
                    if(loadedWorld.fuelType[containerIndex] == "kerosene")
                    {
                        container.substance = kerosene;
                    }
                    if(loadedWorld.fuelType[containerIndex] == "LOX")
                    {
                        container.substance = LOX;
                    }
                    containerIndex++;
                }
            }

            int padIndex = 0;
            launchPadManager[] launchPadManagers = FindObjectsOfType<launchPadManager>();
            foreach (launchPadManager launchPadManager in launchPadManagers)
            {
                launchPadManager.connectedContainersPerLine = loadedWorld.containerGuids[padIndex];
                padIndex++;
            }

            int standIndex = 0;
            flowControllerForTankStand[] flowControllersForTankStand = FindObjectsOfType<flowControllerForTankStand>();
            foreach (flowControllerForTankStand flowControllerForTankStand in flowControllersForTankStand)
            {
                flowControllerForTankStand.originGuid = loadedWorld.standGuid[standIndex];
                foreach (container container in containers)
                {
                    if (container.guid == flowControllerForTankStand.originGuid && flowControllerForTankStand.originGuid != Guid.Empty) 
                    {
                        flowControllerForTankStand.origin = container;
                    }
                }
                standIndex++;
            }

            int flowControllerID = 0;
            flowController[] flowControllers = FindObjectsOfType<flowController>();
            foreach (flowController flowController in flowControllers)
            {
                if (flowController.GetComponent<buildingType>())
                {
                    flowController.originGuid = loadedWorld.originGuid[flowControllerID];
                    flowController.destinationGuid = loadedWorld.destinationGuid[flowControllerID];
                    foreach (container container in containers)
                    {
                        if (container.guid == flowController.originGuid && flowController.originGuid != Guid.Empty)
                        {
                            flowController.origin = container;
                        }

                        if (container.guid == flowController.destinationGuid && flowController.destinationGuid != Guid.Empty)
                        {
                            flowController.destination = container;
                        }
                    }
                    flowControllerID++;
                }
            }



            launchsiteManager.updateVisibleButtons();
            if (launchsiteManager.commandCenter != null)
            {
                worldCamera.transform.position = launchsiteManager.commandCenter.transform.position;
                earth.GetComponent<BodyPath>().KeplerParams = loadedWorld.earthK;
                moon.GetComponent<BodyPath>().KeplerParams = loadedWorld.moonK;
                earth.GetComponent<BodyPath>().bypass = true;
                moon.GetComponent<BodyPath>().bypass = true;
            }
            sun.GetComponent<SunScript>().InitializeSun();
            earth.GetComponent<EarthScript>().InitializeEarth();
            moon.GetComponent<MoonScript>().InitializeMoon();
            worldToLoad = loadedWorld;
            loaded = true;
        }
        else if (loadedWorld.version != version.currentVersion)
        {
            UnityEngine.Debug.Log("File version not compatible");
        }
    }


}
