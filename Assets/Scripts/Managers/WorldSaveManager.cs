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


    // Start is called before the first frame update
    void Start()
    {
        MasterManager = GameObject.FindGameObjectWithTag("MasterManager");
        loadWorld();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.J))
        {
            saveTheWorld();
        }

        if (Input.GetKey(KeyCode.H) && loaded == false)
        {
            loadWorld();
        }
    }


    public void saveTheWorld()
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

        saveWorld.earthLocX = (float)earth.GetComponent<DoubleTransform>().x_pos;
        saveWorld.earthLocY = (float)earth.GetComponent<DoubleTransform>().y_pos;
        saveWorld.earthLocZ = earth.transform.localPosition.z;

        saveWorld.earthRotX = earth.transform.eulerAngles.x;
        saveWorld.earthRotY = earth.transform.eulerAngles.y;
        saveWorld.earthRotZ = earth.transform.eulerAngles.z;

        saveWorld.moonLocX = (float)moon.GetComponent<DoubleTransform>().x_pos;
        saveWorld.moonLocY = (float)moon.GetComponent<DoubleTransform>().y_pos;
        saveWorld.moonLocZ = moon.transform.localPosition.z;
        
        saveWorld.sunLocX = (float)sun.GetComponent<DoubleTransform>().x_pos;
        saveWorld.sunLocY = (float)sun.GetComponent<DoubleTransform>().y_pos;

        saveWorld.moonVX = moon.GetComponent<BodyPath>().GetVelocityAtTime(time).x;
        saveWorld.moonVY = moon.GetComponent<BodyPath>().GetVelocityAtTime(time).y;
        saveWorld.earthVX = earth.GetComponent<BodyPath>().GetVelocityAtTime(time).x;
        saveWorld.earthVY = earth.GetComponent<BodyPath>().GetVelocityAtTime(time).y;

        saveWorld.nPoints = MasterManager.GetComponent<pointManager>().nPoints;
        saveWorld.partName = MasterManager.GetComponent<MasterManager>().partName;
        saveWorld.count = MasterManager.GetComponent<MasterManager>().count;
        saveWorld.partType = MasterManager.GetComponent<MasterManager>().partType;

        foreach(Turbine turbine in MasterManager.GetComponent<MasterManager>().turbineUnlocked)
        {
            saveWorld.turbineUnlocked.Add(turbine.turbineName);
        }

        foreach(Nozzle nozzle in MasterManager.GetComponent<MasterManager>().nozzleUnlocked)
        {
            saveWorld.nozzleUnlocked.Add(nozzle.nozzleName);
        }

        foreach(Pump pump in MasterManager.GetComponent<MasterManager>().pumpUnlocked)
        {
            saveWorld.pumpUnlocked.Add(pump.pumpName);
        }

        foreach(TVC tvc in MasterManager.GetComponent<MasterManager>().tvcUnlocked)
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
                saveRocket(rocket, saveWorld);
            }

        }

        container[] containers = FindObjectsOfType<container>();
        foreach(container container in containers)
        {
            if(container.GetComponent<buildingType>())
            {
                saveWorld.containerGuid.Add(container.guid);
            }
        }

        flowController[] flowControllers = FindObjectsOfType<flowController>();
        foreach(flowController flowController in flowControllers)
        {
            if(flowController.GetComponent<buildingType>())
            {
                saveWorld.destinationGuid.Add(flowController.destinationGuid);
                saveWorld.originGuid.Add(flowController.originGuid);
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
        if (loadedWorld.version == version.currentVersion)
        {
            int alreadyUsed = 0;
            int capsuleID = 0;

            int engineCount = 0;
            int tankCount = 0;
            int decouplerCount = 0;

            if (loadedWorld.previouslyLoaded == true)
            {
                FindObjectOfType<TimeManager>().time = loadedWorld.time;
                FindObjectOfType<TimeManager>().bypass = true;
                earth.transform.position = new Vector3(loadedWorld.earthLocX, loadedWorld.earthLocY, loadedWorld.earthLocZ);
                moon.transform.position = new Vector3(loadedWorld.moonLocX, loadedWorld.moonLocY, loadedWorld.moonLocZ);
                sun.transform.position = new Vector3(loadedWorld.sunLocX, loadedWorld.sunLocY, 0);

                earth.GetComponent<PhysicsStats>().x_vel = loadedWorld.earthVX;
                earth.GetComponent<PhysicsStats>().y_vel = loadedWorld.earthVY;

                moon.GetComponent<PhysicsStats>().x_vel = loadedWorld.moonVX;
                moon.GetComponent<PhysicsStats>().y_vel = loadedWorld.moonVY;

                MasterManager.GetComponent<pointManager>().nPoints = loadedWorld.nPoints;
                MasterManager.GetComponent<MasterManager>().partName = loadedWorld.partName;
                MasterManager.GetComponent<MasterManager>().partType = loadedWorld.partType;
                MasterManager.GetComponent<MasterManager>().count = loadedWorld.count;

                if (loadedWorld.nodeUnlocked.Count > MasterManager.GetComponent<MasterManager>().nodeUnlocked.Count)
                {
                    foreach(Nozzle nozzle in nozzleReferences)
                    {
                        foreach(string nozzle1 in loadedWorld.nozzleUnlocked)
                        {
                            if(nozzle1 == nozzle.nozzleName && !MasterManager.GetComponent<MasterManager>().nozzleUnlocked.Contains(nozzle))
                            {
                                MasterManager.GetComponent<MasterManager>().nozzleUnlocked.Add(nozzle);
                            }
                        }
                    }

                    foreach(Turbine turbine in turbineReferences)
                    {
                        foreach(string turbine1 in loadedWorld.turbineUnlocked)
                        {
                            if(turbine1 == turbine.turbineName && !MasterManager.GetComponent<MasterManager>().turbineUnlocked.Contains(turbine)) 
                            {
                                MasterManager.GetComponent<MasterManager>().turbineUnlocked.Add(turbine);
                            }
                        }
                    }

                    foreach(Pump pump in pumpReferences)
                    {
                        foreach(string pump1 in loadedWorld.pumpUnlocked)
                        {
                            if(pump1 == pump.pumpName && !MasterManager.GetComponent<MasterManager>().pumpUnlocked.Contains(pump))
                            {
                                MasterManager.GetComponent<MasterManager>().pumpUnlocked.Add(pump);
                            }
                        }
                    }

                    foreach(TVC tvc in tvcReferences)
                    {
                        foreach(string tvc1 in loadedWorld.tvcUnlocked)
                        {
                            if(tvc1 == tvc.TVCName && !MasterManager.GetComponent<MasterManager>().tvcUnlocked.Contains(tvc))
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

            //loadRocket(loadedWorld);

            int containerIndex = 0;
            container[] containers = FindObjectsOfType<container>();
            foreach(container container in containers)
            {
                if(container.GetComponent<buildingType>())
                {
                    container.guid = loadedWorld.containerGuid[containerIndex];
                    containerIndex++;
                }
            }

            int padIndex = 0;
            flowControllerForLaunchPads[] flowControllersForLaunchPads = FindObjectsOfType<flowControllerForLaunchPads>();
            foreach(flowControllerForLaunchPads flowControllerForLaunchPads in flowControllersForLaunchPads)
            {
                flowControllerForLaunchPads.fuelGuid = loadedWorld.padFuelGuid[padIndex];
                flowControllerForLaunchPads.oxidizerGuid = loadedWorld.padOxidizerGuid[padIndex];

                foreach(container container in containers)
                {
                    if(container.guid == flowControllerForLaunchPads.fuelGuid)
                    {
                        flowControllerForLaunchPads.fuelContainerOrigin = container;
                    }

                    if(container.guid == flowControllerForLaunchPads.oxidizerGuid)
                    {
                        flowControllerForLaunchPads.oxidizerContainerOrigin = container;
                    }
                }

                padIndex++;
            }

            int staticFireStandIndex = 0;
            flowControllerStaticFire[] flowControllersStaticFire = FindObjectsOfType<flowControllerStaticFire>();
            foreach(flowControllerStaticFire flowControllerStaticFire in flowControllersStaticFire)
            {
                flowControllerStaticFire.fuelGuid = loadedWorld.staticFireFuelGuid[staticFireStandIndex];
                flowControllerStaticFire.oxidizerGuid = loadedWorld.staticFireOxidizerGuid[staticFireStandIndex];

                foreach(container container in containers)
                {
                    if(container.guid == flowControllerStaticFire.fuelGuid)
                    {
                        flowControllerStaticFire.fuelContainer = container;
                    }

                    if(container.guid == flowControllerStaticFire.oxidizerGuid)
                    {
                        flowControllerStaticFire.oxidizerContainer = container;
                    }
                }

                staticFireStandIndex++;
            }

            int standIndex = 0;
            flowControllerForTankStand[] flowControllersForTankStand = FindObjectsOfType<flowControllerForTankStand>();
            foreach(flowControllerForTankStand flowControllerForTankStand in flowControllersForTankStand)
            {
                flowControllerForTankStand.originGuid = loadedWorld.standGuid[standIndex];
                foreach(container container in containers)
                {
                    if(container.guid == flowControllerForTankStand.originGuid)
                    {
                        flowControllerForTankStand.origin = container;
                    }
                }
                standIndex++;
            }

            int flowControllerID = 0;
            flowController[] flowControllers = FindObjectsOfType<flowController>();
            foreach(flowController flowController in flowControllers)
            {
                if(flowController.GetComponent<buildingType>())
                {
                    flowController.originGuid = loadedWorld.originGuid[flowControllerID];
                    flowController.destinationGuid = loadedWorld.destinationGuid[flowControllerID];
                    foreach(container container in containers)
                    {
                        if(container.guid == flowController.originGuid)
                        {
                            flowController.origin = container;
                        }

                        if(container.guid == flowController.destinationGuid)
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
        saveWorldRocket.x_pos.Add(rocket.GetComponent<DoubleTransform>().x_pos);
        saveWorldRocket.y_pos.Add(rocket.GetComponent<DoubleTransform>().y_pos);
        saveWorldRocket.v_x.Add(rocket.GetComponent<Rigidbody2D>().velocity.x);
        saveWorldRocket.v_y.Add(rocket.GetComponent<Rigidbody2D>().velocity.y);
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
        foreach (saveWorldRocket saveRocket in load.rockets)
        {
            int stageID = 0;
            int partID = 0;
            int corePosStage = 0;
            int corePosPart = 0;
            savePart core = new savePart();
            //Find core
            foreach (saveStage saveStage in saveRocket.stages)
            {
                partID = 0;
                foreach (savePart savePart in saveStage.Parts)
                {
                    if (savePart.guid == saveRocket.coreID)
                    {
                        core = savePart;
                    }
                    partID++;
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

            if (root != null)
            {
                root.transform.position = new Vector3((float)saveRocket.x_pos[i], (float)saveRocket.y_pos[i], 0);
                root.AddComponent<Rigidbody2D>();
                root.GetComponent<Rigidbody2D>().angularDrag = 0;
                root.GetComponent<Rigidbody2D>().freezeRotation = true;
                root.GetComponent<Rigidbody2D>().gravityScale = 0;
                root.GetComponent<Rigidbody2D>().collisionDetectionMode = CollisionDetectionMode2D.Continuous;
                root.AddComponent<PlanetGravity>();
                root.GetComponent<PlanetGravity>().setCore(root);
                root.GetComponent<Rocket>().core = root;
                root.AddComponent<RocketStateManager>();
                root.AddComponent<RocketPath>();
                root.AddComponent<BodySwitcher>();
            }

            //Load all parts
            foreach (saveStage saveStage in saveRocket.stages)
            {
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
                        }
                    }
                }
            }

            i++;
        }
    }

}
