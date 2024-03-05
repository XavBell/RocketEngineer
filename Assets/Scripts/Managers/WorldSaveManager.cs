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
using UnityEditor.SceneManagement;
using UnityEditor.Tilemaps;

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
    public List<Rocket> rocketToExclude = new List<Rocket>();

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
            loadRocket(worldToLoad);
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
            saveWorld.partName = MasterManager.GetComponent<MasterManager>().partName;
            saveWorld.count = MasterManager.GetComponent<MasterManager>().count;
            saveWorld.partType = MasterManager.GetComponent<MasterManager>().partType;

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
                        rocketToExclude.Add(building.GetComponent<launchPadManager>().ConnectedRocket.GetComponent<Rocket>());
                    }


                    saveWorld.padFuelGuid.Add(building.GetComponent<flowControllerForLaunchPads>().fuelGuid);

                    saveWorld.padOxidizerGuid.Add(building.GetComponent<flowControllerForLaunchPads>().oxidizerGuid);
                }

                if (building.GetComponent<buildingType>().type == "staticFireStand")
                {

                    saveWorld.staticFireFuelGuid.Add(building.GetComponent<flowControllerStaticFire>().fuelGuid);

                    saveWorld.staticFireOxidizerGuid.Add(building.GetComponent<flowControllerStaticFire>().oxidizerGuid);
                }

                if (building.GetComponent<buildingType>().type == "standTank")
                {
                    saveWorld.standGuid.Add(building.GetComponent<flowControllerForTankStand>().originGuid);

                }
            }

            Rocket[] Rockets = FindObjectsOfType<Rocket>();
            foreach (Rocket rocket in Rockets)
            {
                if (!rocketToExclude.Contains(rocket))
                {
                    print("saving rocket");
                    saveRocket(rocket, saveWorld);
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
            FindObjectOfType<StageViewer>().Stop();
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
                earth.GetComponent<BodyPath>().KeplerParams = loadedWorld.earthK;
                moon.GetComponent<BodyPath>().KeplerParams = loadedWorld.moonK;
                earth.GetComponent<BodyPath>().bypass = true;
                moon.GetComponent<BodyPath>().bypass = true;

                MasterManager.GetComponent<pointManager>().nPoints = loadedWorld.nPoints;
                MasterManager.GetComponent<MasterManager>().partName = loadedWorld.partName;
                MasterManager.GetComponent<MasterManager>().partType = loadedWorld.partType;
                MasterManager.GetComponent<MasterManager>().count = loadedWorld.count;

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
            flowControllerForLaunchPads[] flowControllersForLaunchPads = FindObjectsOfType<flowControllerForLaunchPads>();
            foreach (flowControllerForLaunchPads flowControllerForLaunchPads in flowControllersForLaunchPads)
            {
                flowControllerForLaunchPads.fuelGuid = loadedWorld.padFuelGuid[padIndex];
                flowControllerForLaunchPads.oxidizerGuid = loadedWorld.padOxidizerGuid[padIndex];

                foreach (container container in containers)
                {
                    if (container.guid == flowControllerForLaunchPads.fuelGuid)
                    {
                        flowControllerForLaunchPads.fuelContainerOrigin = container;
                    }

                    if (container.guid == flowControllerForLaunchPads.oxidizerGuid)
                    {
                        flowControllerForLaunchPads.oxidizerContainerOrigin = container;
                    }
                }

                padIndex++;
            }

            int staticFireStandIndex = 0;
            flowControllerStaticFire[] flowControllersStaticFire = FindObjectsOfType<flowControllerStaticFire>();
            foreach (flowControllerStaticFire flowControllerStaticFire in flowControllersStaticFire)
            {
                flowControllerStaticFire.fuelGuid = loadedWorld.staticFireFuelGuid[staticFireStandIndex];
                flowControllerStaticFire.oxidizerGuid = loadedWorld.staticFireOxidizerGuid[staticFireStandIndex];

                foreach (container container in containers)
                {
                    if (container.guid == flowControllerStaticFire.fuelGuid)
                    {
                        flowControllerStaticFire.fuelContainer = container;
                    }

                    if (container.guid == flowControllerStaticFire.oxidizerGuid)
                    {
                        flowControllerStaticFire.oxidizerContainer = container;
                    }
                }

                staticFireStandIndex++;
            }

            int standIndex = 0;
            flowControllerForTankStand[] flowControllersForTankStand = FindObjectsOfType<flowControllerForTankStand>();
            foreach (flowControllerForTankStand flowControllerForTankStand in flowControllersForTankStand)
            {
                flowControllerForTankStand.originGuid = loadedWorld.standGuid[standIndex];
                foreach (container container in containers)
                {
                    if (container.guid == flowControllerForTankStand.originGuid)
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
                        if (container.guid == flowController.originGuid)
                        {
                            flowController.origin = container;
                        }

                        if (container.guid == flowController.destinationGuid)
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

    public void saveRocket(Rocket rocket, saveWorld save)
    {
        saveWorldRocket saveWorldRocket = new saveWorldRocket();
        saveWorldRocket.v_x.Add(rocket.GetComponent<Rigidbody2D>().velocity.x);
        saveWorldRocket.v_y.Add(rocket.GetComponent<Rigidbody2D>().velocity.y);
        saveWorldRocket.state.Add(rocket.GetComponent<RocketStateManager>().state);
        saveWorldRocket.x_pos.Add(rocket.GetComponent<DoubleTransform>().x_pos);
        saveWorldRocket.y_pos.Add(rocket.GetComponent<DoubleTransform>().y_pos);
        if (rocket.GetComponent<RocketStateManager>().state == "landed")
        {
            saveWorldRocket.x_pos.Add(rocket.transform.position.x);
            saveWorldRocket.y_pos.Add(rocket.transform.position.y);
        }
        else
        {
            saveWorldRocket.x_pos.Add(rocket.GetComponent<DoubleTransform>().x_pos);
            saveWorldRocket.y_pos.Add(rocket.GetComponent<DoubleTransform>().y_pos);
        }
        saveWorldRocket.curr_X.Add(rocket.GetComponent<RocketStateManager>().curr_X);
        saveWorldRocket.curr_Y.Add(rocket.GetComponent<RocketStateManager>().curr_Y);
        saveWorldRocket.prev_X.Add(rocket.GetComponent<RocketStateManager>().previous_X);
        saveWorldRocket.prev_Y.Add(rocket.GetComponent<RocketStateManager>().previous_Y);
        saveWorldRocket.keplerParams.Add(rocket.GetComponent<RocketPath>().KeplerParams);
        saveWorldRocket.Ho = rocket.GetComponent<RocketPath>().Ho;
        saveWorldRocket.H = rocket.GetComponent<RocketPath>().H;
        saveWorldRocket.Mo = rocket.GetComponent<RocketPath>().Mo;
        saveWorldRocket.n = rocket.GetComponent<RocketPath>().n;
        saveWorldRocket.e = rocket.GetComponent<RocketPath>().e;
        saveWorldRocket.a = rocket.GetComponent<RocketPath>().a;
        saveWorldRocket.i = rocket.GetComponent<RocketPath>().i;
        saveWorldRocket.lastUpdatedTime = rocket.GetComponent<RocketPath>().lastUpdatedTime;
        saveWorldRocket.startTime = rocket.GetComponent<RocketPath>().startTime;
        saveWorldRocket.planetName.Add(rocket.GetComponent<PlanetGravity>().getPlanet().GetComponent<TypeScript>().type);

        saveWorldRocket.coreID = rocket.GetComponent<Rocket>().core.GetComponent<RocketPart>()._partID;
        foreach (Stages stage in rocket.Stages)
        {
            saveStage saveStage = new saveStage();
            foreach (RocketPart part in stage.Parts)
            {
                savePart savePart = new savePart();
                savePart.type = part._partType;
                savePart.name = part._partName;
                savePart.path = part._path;
                savePart.guid = part._partID;
                savePart.mass = part._partMass;
                savePart.posX = part.transform.localPosition.x;
                savePart.posY = part.transform.localPosition.y;
                savePart.cost = part._partCost;
                if (part._attachBottom != null)
                {
                    if (part._attachBottom.GetComponent<AttachPointScript>().attachedBody != null)
                    {
                        savePart.guidBottom = part._attachBottom.GetComponent<AttachPointScript>().attachedBody.GetComponent<RocketPart>()._partID;
                    }
                }
                else
                {
                    savePart.guidBottom = Guid.Empty;
                }
                if (part._attachTop != null)
                {
                    if (part._attachTop.GetComponent<AttachPointScript>().attachedBody != null)
                    {
                        savePart.guidTop = part._attachTop.GetComponent<AttachPointScript>().attachedBody.GetComponent<RocketPart>()._partID;
                    }
                }
                else
                {
                    savePart.guidTop = Guid.Empty;
                }
                if (part._attachLeft != null)
                {
                    if (part._attachLeft.GetComponent<AttachPointScript>().attachedBody != null)
                    {
                        savePart.guidLeft = part._attachLeft.GetComponent<AttachPointScript>().attachedBody.GetComponent<RocketPart>()._partID;
                    }
                }
                else
                {
                    savePart.guidLeft = Guid.Empty;
                }
                if (part._attachRight != null)
                {
                    if (part._attachRight.GetComponent<AttachPointScript>().attachedBody != null)
                    {
                        savePart.guidRight = part._attachRight.GetComponent<AttachPointScript>().attachedBody.GetComponent<RocketPart>()._partID;
                    }
                }
                else
                {
                    savePart.guidRight = Guid.Empty;
                }


                if (part._partType == "engine")
                {
                    Engine engine = part.gameObject.GetComponent<Engine>();
                    savePart.thrust = engine._thrust;
                    savePart.tvcSpeed = engine._tvcSpeed;
                    savePart.rate = engine._rate;
                    savePart.reliability = engine.reliability;
                    savePart.turbineName = engine._turbineName;
                    savePart.nozzleName = engine._nozzleName;
                    savePart.pumpName = engine._pumpName;
                    savePart.tvcName = engine._tvcName;
                    savePart.maxAngle = engine._maxAngle;
                    savePart.maxTime = engine.maxTime;
                    savePart.willExplode = engine.willExplode;
                    savePart.willFail = engine.willFail;
                    savePart.timeOfFail = engine.timeOfFail;
                    savePart.operational = engine.operational;
                }

                if (part._partType == "tank")
                {
                    Tank tank = part.gameObject.GetComponent<Tank>();
                    savePart._volume = tank._volume;
                    savePart.x_scale = tank.gameObject.GetComponent<SpriteRenderer>().size.x;
                    savePart.y_scale = tank.gameObject.GetComponent<SpriteRenderer>().size.y;
                    savePart.tankMaterial = tank.tankMaterial;
                    savePart.propellantCategory = tank.propellantCategory;
                    savePart.tested = tank.tested;
                    savePart.coolerActive = tank.GetComponent<cooler>().active;
                    savePart.targetTemp = tank.GetComponent<cooler>().targetTemperature;
                    savePart.internalTemp = tank.GetComponent<container>().internalTemperature;
                    savePart.quantity = tank.GetComponent<container>().moles;
                    if(tank.GetComponent<container>().substance != null)
                    {
                        savePart.fuelType = tank.GetComponent<container>().substance.Name;
                    }else{
                        savePart.fuelType = "none";
                    }
                }
                saveStage.Parts.Add(savePart);
            }
            saveWorldRocket.stages.Add(saveStage);
        }
        save.rockets.Add(saveWorldRocket);
    }

    public void loadRocket(saveWorld load)
    {
        int i = 0;
        print(load.rockets.Count);
        foreach (saveWorldRocket saveRocket in load.rockets)
        {
            int stageID = 0;
            savePart core = new savePart();
            //Find core
            foreach (saveStage saveStage in saveRocket.stages)
            {
                foreach (savePart savePart in saveStage.Parts)
                {
                    if (savePart.guid == saveRocket.coreID)
                    {
                        core = savePart;
                    }
                }
                stageID++;
            }


            //Instantiate core
            GameObject root = null;
            if (core.type == "satellite")
            {
                root = Instantiate(Satellite);
                root.AddComponent<Rocket>();

            }

            if (core.type == "engine")
            {
                root = Instantiate(Engine);
                root.AddComponent<Rocket>();
                Engine engine = root.gameObject.GetComponent<Engine>();
                engine._thrust = core.thrust;
                engine._tvcSpeed = core.tvcSpeed;
                engine._rate = core.rate;
                engine.reliability = core.reliability;
                engine._turbineName = core.turbineName;
                engine._nozzleName = core.nozzleName;
                engine._pumpName = core.pumpName;
                engine._tvcName = core.tvcName;
                engine._maxAngle = core.maxAngle;
                engine.maxTime = core.maxTime;
                engine.willExplode = core.willExplode;
                engine.willFail = core.willFail;
                engine.timeOfFail = core.timeOfFail;
                engine.operational = core.operational;
            }

            if (core.type == "tank")
            {
                root = Instantiate(Tank);
                root.AddComponent<Rocket>();
                Tank tank = root.gameObject.GetComponent<Tank>();
                tank._volume = core._volume;
                tank.gameObject.GetComponent<SpriteRenderer>().size = new Vector2(core.x_scale, core.y_scale);
                tank.tankMaterial = core.tankMaterial;
                tank.propellantCategory = core.propellantCategory;
                tank.transform.localScale = new Vector3(1, 1, 0);
                tank.x_scale = core.x_scale;
                tank.y_scale = core.y_scale;
                tank.tested = core.tested;
                tank.GetComponent<cooler>().active = core.coolerActive;
                tank.GetComponent<cooler>().targetTemperature = core.targetTemp;
                tank.GetComponent<container>().internalTemperature = core.internalTemp;
                tank.GetComponent<container>().moles = core.quantity;
                if(core.fuelType == "kerosene")
                {
                    tank.GetComponent<container>().substance = kerosene;
                }
                if(core.fuelType == "LOX")
                {
                    tank.GetComponent<container>().substance = LOX;
                }
            }

            if (core.type == "decoupler")
            {
                root = Instantiate(Decoupler);
                root.AddComponent<Rocket>();
            }

            List<GameObject> parts = new List<GameObject>();
            List<Guid> guidRefTop = new List<Guid>();
            List<Guid> guidRefBottom = new List<Guid>();
            List<Guid> guidRefLeft = new List<Guid>();
            List<Guid> guidRefRight = new List<Guid>();

            if (root != null)
            {
                root.transform.position = new Vector3((float)saveRocket.x_pos[0], (float)saveRocket.y_pos[0], 0);
                root.AddComponent<Rigidbody2D>();
                root.GetComponent<Rigidbody2D>().angularDrag = 0;
                root.GetComponent<Rigidbody2D>().freezeRotation = true;
                root.GetComponent<Rigidbody2D>().gravityScale = 0;
                root.GetComponent<Rigidbody2D>().collisionDetectionMode = CollisionDetectionMode2D.Continuous;
                root.AddComponent<PlanetGravity>();
                root.GetComponent<PlanetGravity>().setCore(root);
                root.GetComponent<Rocket>().core = root;
                root.AddComponent<RocketStateManager>();
                root.GetComponent<RocketStateManager>().state = saveRocket.state[0];
                root.AddComponent<RocketPath>();
                root.AddComponent<BodySwitcher>();

                root.GetComponent<RocketPart>()._partMass = core.mass;
                root.GetComponent<RocketPart>()._partID = core.guid;
                root.GetComponent<RocketPart>()._partCost = core.cost;
                root.GetComponent<RocketPart>()._partName = core.name;
                root.GetComponent<RocketPart>()._path = core.path;

                parts.Add(root);
                guidRefTop.Add(core.guidTop);
                guidRefBottom.Add(core.guidBottom);
                guidRefLeft.Add(core.guidLeft);
                guidRefRight.Add(core.guidRight);
            }

            //Load all parts
            foreach (saveStage saveStage in saveRocket.stages)
            {
                Stages stage = new Stages();
                root.GetComponent<Rocket>().Stages.Add(stage);
                foreach (savePart savePart in saveStage.Parts)
                {
                    GameObject currentPart = null;
                    if (savePart != core)
                    {
                        if (savePart.type == "satellite")
                        {
                            currentPart = Instantiate(Satellite, root.transform);
                        }

                        if (savePart.type == "engine")
                        {
                            currentPart = Instantiate(Engine, root.transform);
                            Engine engine = currentPart.gameObject.GetComponent<Engine>();
                            engine._thrust = savePart.thrust;
                            engine._tvcSpeed = savePart.tvcSpeed;
                            engine._rate = savePart.rate;
                            engine.reliability = savePart.reliability;
                            engine._turbineName = savePart.turbineName;
                            engine._nozzleName = savePart.nozzleName;
                            engine._pumpName = savePart.pumpName;
                            engine._tvcName = savePart.tvcName;
                            engine._maxAngle = savePart.maxAngle;
                            engine.maxTime = savePart.maxTime;
                            engine.willExplode = savePart.willExplode;
                            engine.willFail = savePart.willFail;
                            engine.timeOfFail = savePart.timeOfFail;
                            engine.operational = savePart.operational;
                        }

                        if (savePart.type == "tank")
                        {
                            currentPart = Instantiate(Tank, root.transform);
                            Tank tank = currentPart.gameObject.GetComponent<Tank>();
                            tank._volume = savePart._volume;
                            tank.gameObject.GetComponent<SpriteRenderer>().size = new Vector2(savePart.x_scale, savePart.y_scale);
                            tank.tankMaterial = savePart.tankMaterial;
                            tank.propellantCategory = savePart.propellantCategory;
                            tank.transform.localScale = new Vector3(1, 1, 0);
                            tank.x_scale = savePart.x_scale;
                            tank.y_scale = savePart.y_scale;
                            tank.tested = savePart.tested;
                            tank.GetComponent<cooler>().active = savePart.coolerActive;
                            tank.GetComponent<cooler>().targetTemperature = savePart.targetTemp;
                            tank.GetComponent<container>().internalTemperature = savePart.internalTemp;
                            tank.GetComponent<container>().moles = savePart.quantity;
                            if(savePart.fuelType == "kerosene")
                            {
                                tank.GetComponent<container>().substance = kerosene;
                            }
                            if(savePart.fuelType == "LOX")
                            {
                                tank.GetComponent<container>().substance = LOX;
                            }
                        }

                        if (savePart.type == "decoupler")
                        {
                            currentPart = Instantiate(Decoupler, root.transform);
                        }

                        if (currentPart != null)
                        {
                            currentPart.transform.localPosition = new Vector3(savePart.posX, savePart.posY, 0);
                            currentPart.GetComponent<RocketPart>()._partMass = savePart.mass;
                            currentPart.GetComponent<RocketPart>()._partID = savePart.guid;
                            currentPart.GetComponent<RocketPart>()._partCost = savePart.cost;
                            currentPart.GetComponent<RocketPart>()._partName = savePart.name;
                            currentPart.GetComponent<RocketPart>()._partType = savePart.type;
                            currentPart.GetComponent<RocketPart>()._path = savePart.path;
                            parts.Add(currentPart);
                            guidRefTop.Add(savePart.guidTop);
                            guidRefBottom.Add(savePart.guidBottom);
                            guidRefLeft.Add(savePart.guidLeft);
                            guidRefRight.Add(savePart.guidRight);
                            stage.Parts.Add(currentPart.GetComponent<RocketPart>());
                        }
                    }

                    if (savePart == core)
                    {
                        stage.Parts.Add(root.GetComponent<RocketPart>());
                    }
                }
            }

            //Connect all parts
            int checkID = 0;
            foreach (GameObject part in parts)
            {
                if (guidRefTop[checkID] != Guid.Empty)
                {
                    foreach (GameObject part2 in parts)
                    {
                        if (part2.GetComponent<RocketPart>()._partID == guidRefTop[checkID])
                        {
                            part.GetComponent<RocketPart>()._attachTop.GetComponent<AttachPointScript>().attachedBody = part2;
                        }
                    }
                }

                if (guidRefBottom[checkID] != Guid.Empty)
                {
                    foreach (GameObject part2 in parts)
                    {
                        if (part2.GetComponent<RocketPart>()._partID == guidRefBottom[checkID])
                        {
                            part.GetComponent<RocketPart>()._attachBottom.GetComponent<AttachPointScript>().attachedBody = part2;
                        }
                    }
                }

                if (guidRefLeft[checkID] != Guid.Empty)
                {
                    foreach (GameObject part2 in parts)
                    {
                        if (part2.GetComponent<RocketPart>()._partID == guidRefLeft[checkID])
                        {
                            part.GetComponent<RocketPart>()._attachLeft.GetComponent<AttachPointScript>().attachedBody = part2;
                        }
                    }
                }

                if (guidRefRight[checkID] != Guid.Empty)
                {
                    foreach (GameObject part2 in parts)
                    {
                        if (part2.GetComponent<RocketPart>()._partID == guidRefRight[checkID])
                        {
                            part.GetComponent<RocketPart>()._attachRight.GetComponent<AttachPointScript>().attachedBody = part2;
                        }
                    }
                }

                checkID++;
            }

            if((float)saveRocket.v_x[0] != float.NaN && (float)saveRocket.v_y[0] != float.NaN)
            {
                root.GetComponent<Rigidbody2D>().velocity = new Vector2((float)saveRocket.v_x[0], (float)saveRocket.v_y[0]);
            }
            if (saveRocket.state[0] == "rail")
            {
                //Tricking state manager
                root.GetComponent<RocketStateManager>().state = "simulate";
                root.GetComponent<RocketStateManager>().previousState = "simulate";
                if(saveRocket.planetName[0] == "moon")
                {
                    root.GetComponent<PlanetGravity>().setPlanet(moon);
                }
                if (saveRocket.planetName[0] == "earth")
                {
                    root.GetComponent<PlanetGravity>().setPlanet(earth);
                }
                if(saveRocket.planetName[0] == "sun")
                {
                    root.GetComponent<PlanetGravity>().setPlanet(sun);
                }

                root.GetComponent<RocketPath>().KeplerParams = saveRocket.keplerParams[0];
                root.GetComponent<RocketPath>().Ho = saveRocket.Ho;
                root.GetComponent<RocketPath>().H = saveRocket.H;
                root.GetComponent<RocketPath>().Mo = saveRocket.Mo;
                root.GetComponent<RocketPath>().n = saveRocket.n;
                root.GetComponent<RocketPath>().e = saveRocket.e;
                root.GetComponent<RocketPath>().a = saveRocket.a;
                root.GetComponent<RocketPath>().i = saveRocket.i;
                UnityEngine.Debug.Log(root.GetComponent<RocketPath>().e.ToString() + root.GetComponent<RocketPath>().KeplerParams.eccentricity.ToString());
                root.GetComponent<RocketPath>().lastUpdatedTime = saveRocket.lastUpdatedTime;
                root.GetComponent<RocketPath>().startTime = saveRocket.startTime;

                root.GetComponent<RocketPath>().bypass = true;
                
                root.GetComponent<RocketPath>().planetGravity = root.GetComponent<PlanetGravity>();
                root.GetComponent<RocketPath>().rb = root.GetComponent<Rigidbody2D>();
                root.GetComponent<RocketStateManager>().curr_X = (float)saveRocket.curr_X[0];
                root.GetComponent<RocketStateManager>().curr_Y = (float)saveRocket.curr_Y[0];
                root.GetComponent<RocketStateManager>().previous_X = (float)saveRocket.prev_X[0];
                root.GetComponent<RocketStateManager>().previous_Y = (float)saveRocket.prev_Y[0];
                root.GetComponent<RocketPath>().updatePosition();
                i++;
                continue;
            }

            if (saveRocket.state[0] == "landed")
            {
                print("landed");
                root.GetComponent<RocketStateManager>().curr_X = (float)saveRocket.curr_X[0];
                root.GetComponent<RocketStateManager>().curr_Y = (float)saveRocket.curr_Y[0];
                root.GetComponent<RocketStateManager>().previous_X = (float)saveRocket.prev_X[0];
                root.GetComponent<RocketStateManager>().previous_Y = (float)saveRocket.prev_Y[0];
                root.GetComponent<RocketStateManager>().previousState = "none";
                
                if (saveRocket.planetName[0] == "moon")
                {
                    root.transform.parent = moon.transform;
                    root.transform.localPosition = new Vector3((float)saveRocket.x_pos[0], (float)saveRocket.y_pos[0], 0);
                    root.GetComponent<DoubleTransform>().x_pos = root.transform.position.x;
                    root.GetComponent<DoubleTransform>().y_pos = root.transform.position.y;
                    root.GetComponent<RocketStateManager>().savedPlanet = moon;
                }
                if (saveRocket.planetName[0] == "earth")
                {
                    root.transform.parent = earth.transform;
                    root.transform.localPosition = new Vector3((float)saveRocket.x_pos[0], (float)saveRocket.y_pos[0], 0);
                    root.GetComponent<DoubleTransform>().x_pos = root.transform.position.x;
                    root.GetComponent<DoubleTransform>().y_pos = root.transform.position.y;
                    root.GetComponent<RocketStateManager>().savedPlanet = earth;

                }
                i++;
                continue;
            }

            if (saveRocket.state[0] == "simulate")
            {
                root.GetComponent<RocketStateManager>().state = "simulate";
                root.GetComponent<RocketStateManager>().previousState = "simulate";
                if(saveRocket.planetName[0] == "moon")
                {
                    root.GetComponent<PlanetGravity>().setPlanet(moon);
                }
                if (saveRocket.planetName[0] == "earth")
                {
                    root.GetComponent<PlanetGravity>().setPlanet(earth);
                }
                if(saveRocket.planetName[0] == "sun")
                {
                    root.GetComponent<PlanetGravity>().setPlanet(sun);
                }
                   
                root.GetComponent<RocketPath>().KeplerParams = saveRocket.keplerParams[0];
                root.GetComponent<RocketStateManager>().curr_X = (float)saveRocket.curr_X[0];
                root.GetComponent<RocketStateManager>().curr_Y = (float)saveRocket.curr_Y[0];
                root.GetComponent<RocketStateManager>().previous_X = (float)saveRocket.prev_X[0];
                root.GetComponent<RocketStateManager>().previous_Y = (float)saveRocket.prev_Y[0];
                i++;
                continue;
            }
            i++;
        }

    }

}
