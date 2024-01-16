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

        double time = FindObjectOfType<TimeManager>().time;
        saveWorld.moonVX = moon.GetComponent<BodyPath>().GetVelocityAtTime(time).x;
        saveWorld.moonVY = moon.GetComponent<BodyPath>().GetVelocityAtTime(time).y;
        saveWorld.earthVX = earth.GetComponent<BodyPath>().GetVelocityAtTime(time).x;
        saveWorld.earthVY = earth.GetComponent<BodyPath>().GetVelocityAtTime(time).y;

        saveWorld.nPoints = MasterManager.GetComponent<pointManager>().nPoints;
        saveWorld.partName = MasterManager.GetComponent<MasterManager>().partName;
        saveWorld.count = MasterManager.GetComponent<MasterManager>().count;
        saveWorld.partType = MasterManager.GetComponent<MasterManager>().partType;

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

            if (building.GetComponent<buildingType>().type == "designer")
            {
                saveWorld.inputIDs.Add(0);
                saveWorld.outputIDs.Add(0);
            }

            if (building.GetComponent<buildingType>().type == "commandCenter")
            {
                saveWorld.inputIDs.Add(0);
                saveWorld.outputIDs.Add(0);
            }

            if (building.GetComponent<buildingType>().type == "VAB")
            {
                saveWorld.inputIDs.Add(0);
                saveWorld.outputIDs.Add(0);
            }

            if (building.GetComponent<buildingType>().type == "GSEtank")
            {

                saveWorld.inputIDs.Add(building.GetComponent<outputInputManager>().inputParentID);
                saveWorld.outputIDs.Add(building.GetComponent<outputInputManager>().outputParentID);
            }

            if (building.GetComponent<buildingType>().type == "launchPad")
            {
                outputInputManager[] outputInputManagers1 = building.GetComponents<outputInputManager>();
                foreach (outputInputManager outputInputManager in outputInputManagers1)
                {
                    saveWorld.inputIDs.Add(outputInputManager.inputParentID);
                    saveWorld.outputIDs.Add(outputInputManager.outputParentID);
                }
            }

            if (building.GetComponent<buildingType>().type == "staticFireStand")
            {
                outputInputManager[] outputInputManagers1 = building.GetComponents<outputInputManager>();
                foreach (outputInputManager outputInputManager in outputInputManagers1)
                {
                    saveWorld.inputIDs.Add(outputInputManager.inputParentID);
                    saveWorld.outputIDs.Add(outputInputManager.outputParentID);
                }
            }

            if (building.GetComponent<buildingType>().type == "standTank")
            {
                outputInputManager[] outputInputManagers1 = building.GetComponents<outputInputManager>();
                foreach (outputInputManager outputInputManager in outputInputManagers1)
                {
                    saveWorld.inputIDs.Add(outputInputManager.inputParentID);
                    saveWorld.outputIDs.Add(outputInputManager.outputParentID);
                }
            }
        }

        outputInputManager[] outputInputManagers = FindObjectsOfType<outputInputManager>();
        foreach (outputInputManager outputInputManager in outputInputManagers)
        {
            saveWorld.selfGuid.Add(outputInputManager.guid);
            saveWorld.InputGuid.Add(outputInputManager.inputGuid);
            saveWorld.OutputGuid.Add(outputInputManager.outputGuid);
        }

        Rocket[] Rockets = FindObjectsOfType<Rocket>();
        foreach (Rocket rocket in Rockets)
        {
            saveRocket(rocket, saveWorld);
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

                earth.GetComponent<PhysicsStats>().x_vel = loadedWorld.earthVX;
                earth.GetComponent<PhysicsStats>().y_vel = loadedWorld.earthVY;

                moon.GetComponent<PhysicsStats>().x_vel = loadedWorld.moonVX;
                moon.GetComponent<PhysicsStats>().y_vel = loadedWorld.moonVY;

                MasterManager.GetComponent<pointManager>().nPoints = loadedWorld.nPoints;
                MasterManager.GetComponent<MasterManager>().partName = loadedWorld.partName;
                MasterManager.GetComponent<MasterManager>().partType = loadedWorld.partType;
                MasterManager.GetComponent<MasterManager>().count = loadedWorld.count;
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
                    current.GetComponent<outputInputManager>().inputParentID = loadedWorld.inputIDs[count];
                    current.GetComponent<outputInputManager>().outputParentID = loadedWorld.outputIDs[count];
                }

                if (buildingType == "launchPad")
                {
                    GameObject current = Instantiate(launchPadPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                    current.transform.SetParent(earth.transform);
                    current.transform.localPosition = position;
                    current.transform.eulerAngles = rotation;
                    current.GetComponent<buildingType>().buildingID = loadedWorld.buildingIDs[count];
                    outputInputManager[] outputInputManagers1 = current.GetComponents<outputInputManager>();
                    foreach (outputInputManager outputInputManager in outputInputManagers1)
                    {
                        outputInputManager.inputParentID = loadedWorld.inputIDs[count];
                        outputInputManager.outputParentID = loadedWorld.outputIDs[count];
                    }
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
                    outputInputManager[] outputInputManagers1 = current.GetComponents<outputInputManager>();
                    foreach (outputInputManager outputInputManager in outputInputManagers1)
                    {
                        outputInputManager.inputParentID = loadedWorld.inputIDs[count];
                        outputInputManager.outputParentID = loadedWorld.outputIDs[count];
                    }
                }

                if (buildingType == "staticFireStand")
                {
                    GameObject current = Instantiate(staticFireStandPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                    current.transform.SetParent(earth.transform);
                    current.transform.localPosition = position;
                    current.transform.eulerAngles = rotation;
                    current.GetComponent<buildingType>().buildingID = loadedWorld.buildingIDs[count];
                    outputInputManager[] outputInputManagers1 = current.GetComponents<outputInputManager>();
                    foreach (outputInputManager outputInputManager in outputInputManagers1)
                    {
                        outputInputManager.inputParentID = loadedWorld.inputIDs[count];
                        outputInputManager.outputParentID = loadedWorld.outputIDs[count];
                    }
                }
                count++;
            }

            loadRocket(loadedWorld);

            outputInputManager[] outputInputManagers = FindObjectsOfType<outputInputManager>();
            int x = 0;
            foreach (outputInputManager outputInputManager in outputInputManagers)
            {
                outputInputManager.guid = loadedWorld.selfGuid[x];
                outputInputManager.inputGuid = loadedWorld.InputGuid[x];
                outputInputManager.outputGuid = loadedWorld.OutputGuid[x];
                x++;
            }

            foreach (outputInputManager outputInputManager1 in outputInputManagers)
            {
                foreach (outputInputManager outputInputManager2 in outputInputManagers)
                {
                    if (outputInputManager1.guid == outputInputManager2.outputGuid)
                    {
                        outputInputManager2.outputParent = outputInputManager1;
                        outputInputManager1.inputParent = outputInputManager2;
                    }

                    if (outputInputManager2.guid == outputInputManager1.outputGuid)
                    {
                        outputInputManager1.outputParent = outputInputManager2;
                        outputInputManager2.inputParent = outputInputManager1;
                    }
                }
            }



            launchsiteManager.updateVisibleButtons();
            if (launchsiteManager.commandCenter != null)
            {
                worldCamera.transform.position = launchsiteManager.commandCenter.transform.position;
            }




            earth.GetComponent<EarthScript>().InitializeEarth();
            moon.GetComponent<MoonScript>().InitializeMoon();
            sun.GetComponent<SunScript>().InitializeSun();

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
                root.GetComponent<PlanetGravity>().core = root;
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









    //TO DELETE
    public void save(Rocket Rocket)
    {
        if (!Directory.Exists(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.GetComponent<MasterManager>().FolderName + savePathRef.rocketFolder))
        {
            Directory.CreateDirectory(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.GetComponent<MasterManager>().FolderName + savePathRef.rocketFolder);
        }

        if (!File.Exists(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.GetComponent<MasterManager>().FolderName + savePathRef.rocketFolder + Rocket.name + ".json"))
        {
            savecraft saveRocket = new savecraft();
            int i = 0;
            saveRocket.coreID = Rocket.GetComponent<Rocket>().core.GetComponent<RocketPart>()._partID;
            foreach (Stages stage in Rocket.GetComponent<Rocket>().Stages)
            {
                foreach (RocketPart part in stage.Parts)
                {

                    //Set tank
                    if (part._partType == "tank")
                    {
                        saveRocket.x_scale.Add(part.GetComponent<BoxCollider2D>().size.x);
                        saveRocket.y_scale.Add(part.GetComponent<BoxCollider2D>().size.y);
                        saveRocket.volume.Add(part.GetComponent<Tank>()._volume);
                        saveRocket.propellantType.Add(part.GetComponent<Tank>().propellantCategory);
                        saveRocket.tankMaterial.Add(part.GetComponent<Tank>().tankMaterial);
                    }

                    //Set Engine
                    if (part._partType == "engine")
                    {
                        saveRocket.engineName.Add(part.GetComponent<Engine>()._partName);
                        saveRocket.thrust.Add(part.GetComponent<Engine>()._thrust);
                        saveRocket.flowRate.Add(part.GetComponent<Engine>()._rate);
                    }
                }

                i++;
            }

            var jsonString = JsonConvert.SerializeObject(saveRocket);
            System.IO.File.WriteAllText(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.GetComponent<MasterManager>().FolderName + savePathRef.rocketFolder + Rocket.name + ".json", jsonString);

        }
        else
        {
            //cry
        }
    }

    public void setPosition(float x, float y, float z, GameObject current)
    {
        current.transform.localPosition = new Vector3(x, y, z);
    }


}
