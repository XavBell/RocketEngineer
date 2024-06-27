using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using Newtonsoft.Json;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEditorInternal;
using UnityEngine.Animations;

public class rocketSaveManager : MonoBehaviour
{
    public savePath savePathRef = new savePath();
    public MasterManager masterManager;
    public GameObject StageEditor;
    public void Start()
    {
        masterManager = FindObjectOfType<MasterManager>();
    }

    //Editor Related and launchpad spawn
    public void saveRocket(RocketController rocketController, string rocketName)
    {
        RocketData rocketData = new RocketData();
        rocketData.rocketName = rocketController.rocketName;
        rocketData.lineNames = rocketController.lineNames;
        rocketData.lineGuids = rocketController.lineGuids;
        rocketData.x_pos = rocketController.transform.position.x;
        rocketData.y_pos = rocketController.transform.position.y;
        rocketData.rootPart = new PartData();
        GameObject originalPart = rocketController.transform.GetChild(0).gameObject;
        rocketData.rootPart.partType = originalPart.GetComponent<PhysicsPart>().type;
        rocketData.rootPart.x_pos = originalPart.transform.localPosition.x;
        rocketData.rootPart.y_pos = originalPart.transform.localPosition.y;
        rocketData.rootPart.z_rot = originalPart.transform.eulerAngles.z;
        rocketData.rootPart.guid = originalPart.GetComponent<PhysicsPart>().guid;
        savePartFromType(rocketData.rootPart.partType, originalPart, rocketData.rootPart);
        saveStage(rocketData);

        //Get all children
        PartData rootPart = rocketData.rootPart;
        GameObject rootPartObject = originalPart;
        AddChildren(rootPart, rootPartObject);


        //Write file
        string saveUserPath = Application.persistentDataPath + savePathRef.worldsFolder + '/' + masterManager.FolderName + savePathRef.rocketFolder + "/" + rocketName + ".json";
        string rocketData1 = JsonConvert.SerializeObject(rocketData);
        File.WriteAllText(saveUserPath, rocketData1);

    }

    //Editor Related and LaunchPad spawn
    public void loadRocket(RocketController rocketController, string rocketName)
    {
        string saveUserPath = Application.persistentDataPath + savePathRef.worldsFolder + '/' + masterManager.FolderName + savePathRef.rocketFolder + "/" + rocketName + ".json";
        string rocketData1 = File.ReadAllText(saveUserPath);
        RocketData rocketData = JsonConvert.DeserializeObject<RocketData>(rocketData1);

        //Load rocket info
        rocketController.lineNames = rocketData.lineNames;
        rocketController.lineGuids = rocketData.lineGuids;
        rocketController.rocketName = rocketData.rocketName;
        rocketController.transform.position = new Vector2(rocketData.x_pos, rocketData.y_pos);

        //Load parts
        GameObject newPart = Instantiate(Resources.Load<GameObject>("Prefabs/Modules/" + rocketData.rootPart.partType));
        GameObject originalPart = newPart;
        originalPart.transform.rotation = Quaternion.Euler(0, 0, rocketData.rootPart.z_rot);
        originalPart.GetComponent<PhysicsPart>().guid = rocketData.rootPart.guid;
        originalPart.transform.parent = rocketController.transform;
        originalPart.transform.localPosition = new Vector2(0, 0);
        loadPartFromType(rocketData.rootPart.partType, originalPart, rocketData.rootPart, rocketController);
        LoadChildren(rocketData.rootPart, originalPart, rocketController);
        loadStages(rocketData, rocketController);
    }

    public void LoadChildren(PartData parent, GameObject parentObject, RocketController rocketController)
    {
        foreach (PartData child in parent.children)
        {
            GameObject newPart = Instantiate(Resources.Load<GameObject>("Prefabs/Modules/" + child.partType));
            //Important to change rotation before assigning parent
            newPart.transform.rotation = Quaternion.Euler(0, 0, child.z_rot);
            newPart.GetComponent<PhysicsPart>().guid = child.guid;
            loadPartFromType(child.partType, newPart, child, rocketController);
            newPart.transform.parent = parentObject.transform;
            newPart.transform.localPosition = new Vector2(child.x_pos, child.y_pos);
            LoadChildren(child, newPart, rocketController);
        }
    }

    public void loadPartFromType(string type, GameObject part, PartData partData, RocketController rocketController)
    {
        if (type == "decoupler")
        {
            loadDecoupler(part, partData);
        }

        if (type == "tank")
        {
            loadTank(part, partData, rocketController);
        }

        if (type == "engine")
        {
            loadEngine(part, partData);
        }

        if (type == "capsule")
        {
            loadCapsule(part, partData);
        }
    }

    public void loadDecoupler(GameObject part, PartData partData)
    {
        part.GetComponent<DecouplerComponent>().detachFromParent = partData.detachFromParent;
    }

    public void loadTank(GameObject part, PartData partData, RocketController rocketController)
    {
        part.GetComponent<PhysicsPart>().path = partData.fileName;
        if (partData.lineGuid != Guid.Empty)
        {
            part.GetComponent<TankComponent>().lineGuid = partData.lineGuid;
            part.GetComponent<TankComponent>().lineName = rocketController.lineNames[rocketController.lineGuids.IndexOf(partData.lineGuid)];
        }

        //Load tank from path
        var jsonString = File.ReadAllText(Application.persistentDataPath + savePathRef.worldsFolder + '/' + masterManager.FolderName + savePathRef.tankFolder + partData.fileName);
        saveTank loadedTank = JsonConvert.DeserializeObject<saveTank>(jsonString);
        part.transform.localScale = new Vector3(loadedTank.tankSizeX, loadedTank.tankSizeY, 1);
        part.GetComponent<TankComponent>()._volume = loadedTank.volume;
        part.GetComponent<TankComponent>().x_scale = loadedTank.tankSizeX;
        part.GetComponent<TankComponent>().y_scale = loadedTank.tankSizeY;
        part.GetComponent<TankComponent>().conductivity = loadedTank.thermalConductivity;
        part.GetComponent<PhysicsPart>().mass = loadedTank.mass;
        part.GetComponent<PhysicsPart>().cost = loadedTank.cost;
        part.GetComponent<TankComponent>().tested = loadedTank.tested;
    }

    public void loadEngine(GameObject part, PartData partData)
    {
        part.GetComponent<PhysicsPart>().path = partData.fileName;

        //Load engine from path
        var jsonString = File.ReadAllText(Application.persistentDataPath + savePathRef.worldsFolder + '/' + masterManager.FolderName + savePathRef.engineFolder + partData.fileName);
        saveEngine loadedEngine = JsonConvert.DeserializeObject<saveEngine>(jsonString);
        part.GetComponent<EngineComponent>()._nozzleName = loadedEngine.nozzleName_s;
        part.GetComponent<EngineComponent>()._pumpName = loadedEngine.pumpName_s;
        part.GetComponent<EngineComponent>()._turbineName = loadedEngine.turbineName_s;
        part.GetComponent<EngineComponent>().maxThrust = loadedEngine.thrust_s;
        part.GetComponent<EngineComponent>().maxFuelFlow = loadedEngine.rate_s;
        part.GetComponent<EngineComponent>().reliability = loadedEngine.reliability;
        part.GetComponent<PhysicsPart>().mass = loadedEngine.mass_s;
        part.GetComponent<PhysicsPart>().cost = loadedEngine.cost;
        part.GetComponent<EngineComponent>().InitializeSprite();
    }

    public void loadCapsule(GameObject part, PartData partData)
    {
        part.GetComponent<PhysicsPart>().path = partData.fileName;

        //Load capsule from path
        var jsonString = File.ReadAllText(Application.persistentDataPath + savePathRef.worldsFolder + '/' + masterManager.FolderName + savePathRef.capsuleFolder + partData.fileName);
        saveCapsule loadedCapsule = JsonConvert.DeserializeObject<saveCapsule>(jsonString);
        foreach(string module in loadedCapsule.modules)
        {
            if(module != "")
            {
                GameObject newPart = Instantiate(Resources.Load<GameObject>("Prefabs/Modules/CapsuleModules/" + module));
                newPart.transform.parent = part.transform;
                newPart.transform.localPosition = new Vector3(loadedCapsule.modulePositionsX[loadedCapsule.modules.IndexOf(module)], loadedCapsule.modulePositionsY[loadedCapsule.modules.IndexOf(module)], 0);
                newPart.transform.eulerAngles = new Vector3(newPart.transform.eulerAngles.x, loadedCapsule.moduleRotationsY[loadedCapsule.modules.IndexOf(module)], loadedCapsule.moduleRotationsZ[loadedCapsule.modules.IndexOf(module)]);
                newPart.transform.parent = part.GetComponent<CapsuleComponent>().modules[loadedCapsule.modules.IndexOf(module)].transform;
            }
        }
    }

    public void AddChildren(PartData parent, GameObject parentObject)
    {
        foreach (Transform child in parentObject.transform)
        {
            if (child.GetComponent<PhysicsPart>() == null)
            {
                continue;
            }
            PartData newPart = new PartData();
            newPart.partType = child.GetComponent<PhysicsPart>().type;
            newPart.x_pos = child.localPosition.x;
            newPart.y_pos = child.localPosition.y;
            newPart.z_rot = child.eulerAngles.z;
            newPart.guid = child.GetComponent<PhysicsPart>().guid;
            newPart.partPath = child.GetComponent<PhysicsPart>().path;
            savePartFromType(newPart.partType, child.gameObject, newPart);
            parent.children.Add(newPart);
            AddChildren(newPart, child.gameObject);
        }
    }

    public void savePartFromType(string type, GameObject part, PartData partData)
    {
        if (type == "decoupler")
        {
            saveDecoupler(part, partData);
        }

        if (type == "tank")
        {
            saveTank(part, partData);
        }

        if (type == "engine")
        {
            saveEngine(part, partData);
        }

        if (type == "capsule")
        {
            saveCapsule(part, partData);
        }
    }

    public void saveDecoupler(GameObject part, PartData partData)
    {
        partData.detachFromParent = part.GetComponent<DecouplerComponent>().detachFromParent;
    }

    public void saveTank(GameObject part, PartData partData)
    {
        partData.fileName = part.GetComponent<PhysicsPart>().path;
        partData.lineGuid = part.GetComponent<TankComponent>().lineGuid;
        partData._volume = part.GetComponent<TankComponent>()._volume;
        partData.x_scale = part.GetComponent<TankComponent>().x_scale;
        partData.y_scale = part.GetComponent<TankComponent>().y_scale;
        partData.conductivity = part.GetComponent<TankComponent>().conductivity;
        partData.tested = part.GetComponent<TankComponent>().tested;

    }

    public void saveEngine(GameObject part, PartData partData)
    {
        partData.fileName = part.GetComponent<PhysicsPart>().path;
        partData.thrust = part.GetComponent<EngineComponent>().maxThrust;
        partData.massFlowRate = part.GetComponent<EngineComponent>().maxFuelFlow;
        partData.reliability = part.GetComponent<EngineComponent>().reliability;
        partData.mass = part.GetComponent<PhysicsPart>().mass;
    }

    public void saveCapsule(GameObject part, PartData partData)
    {
        partData.fileName = part.GetComponent<PhysicsPart>().path;
        partData.modules = new List<string>();
        partData.modulePositionsX = new List<float>();
        partData.modulePositionsY = new List<float>();
        partData.moduleRotationsY = new List<float>();
        partData.moduleRotationsZ = new List<float>();
        CapsuleComponent capsule = part.GetComponent<CapsuleComponent>();
        foreach (CapsuleModuleComponent module in capsule.modules)
        {
            partData.modules.Add(module.moduleName);
            partData.modulePositionsX.Add(module.transform.localPosition.x);
            partData.modulePositionsY.Add(module.transform.localPosition.y);
            partData.moduleRotationsY.Add(module.transform.eulerAngles.y);
            partData.moduleRotationsZ.Add(module.transform.eulerAngles.z);
        }
    }

    public void saveStage(RocketData rocketData)
    {
        if (StageEditor != null)
        {
            stageContainer[] stageContainers = StageEditor.GetComponent<StageEditor>().container.GetComponentsInChildren<stageContainer>();
            stageData stageData = new stageData();
            foreach (stageContainer container in stageContainers)
            {
                List<Guid> partIDs = new List<Guid>();
                foreach (Transform part in container.transform)
                {
                    if (part.GetComponent<partRef>() != null)
                    {
                        partIDs.Add(part.GetComponent<partRef>().refObj.GetComponent<PhysicsPart>().guid);
                    }
                }
                stageData.partIDs.Add(partIDs);
            }
            rocketData.stageData = stageData;
        }
    }

    public void loadStages(RocketData rocketData, RocketController rocketController)
    {
        if (StageEditor != null)
        {
            if (StageEditor.activeSelf == false)
            {
                StageEditor.SetActive(true);
            }

            //Find all physics parts
            PhysicsPart[] parts = rocketController.GetComponentsInChildren<PhysicsPart>();
            //Load stages buttons
            foreach (List<Guid> partIDs in rocketData.stageData.partIDs)
            {
                GameObject StageContainer = Instantiate(StageEditor.GetComponent<StageEditor>().stageContainer, StageEditor.GetComponent<StageEditor>().container.transform);
                foreach (Guid guid in partIDs)
                {
                    foreach (PhysicsPart part in parts)
                    {
                        if (part.guid == guid && guid != Guid.Empty)
                        {
                            if (part.type == "engine")
                            {
                                GameObject EngineButton = Instantiate(StageEditor.GetComponent<StageEditor>().engineBtn);
                                GameObject child = EngineButton.GetComponentInChildren<Button>().gameObject;
                                StageContainer.GetComponent<stageContainer>().stageEditor = StageEditor.GetComponent<StageEditor>();
                                StageContainer.GetComponent<stageContainer>().stageEditor.stageContainers.Add(StageContainer.GetComponent<stageContainer>());
                                StageContainer.GetComponent<stageContainer>().container = StageContainer;
                                child.transform.SetParent(StageContainer.GetComponent<stageContainer>().container.gameObject.transform);
                                child.GetComponentInChildren<partRef>().refObj = part.gameObject;
                                child.GetComponentInChildren<partRef>().initializeEngineColors();
                                DestroyImmediate(EngineButton);
                            }

                            if (part.type == "decoupler")
                            {
                                GameObject DecouplerButton = Instantiate(StageEditor.GetComponent<StageEditor>().decouplerBtn);
                                GameObject child = DecouplerButton.GetComponentInChildren<Button>().gameObject;
                                StageContainer.GetComponent<stageContainer>().stageEditor = StageEditor.GetComponent<StageEditor>();
                                StageContainer.GetComponent<stageContainer>().stageEditor.stageContainers.Add(StageContainer.GetComponent<stageContainer>());
                                StageContainer.GetComponent<stageContainer>().container = StageContainer;
                                child.transform.SetParent(StageContainer.GetComponent<stageContainer>().container.gameObject.transform);
                                child.GetComponentInChildren<partRef>().refObj = part.gameObject;
                                child.GetComponentInChildren<partRef>().initializeDecouplerColor();
                                DestroyImmediate(DecouplerButton);
                            }
                        }
                    }
                }
            }
        }
    }

    public void saveWorldRocket(RocketController rocketController, out RocketData rocketData)
    {
        rocketData = new RocketData();
        rocketData.lineGuids = rocketController.lineGuids;
        rocketData.lineNames = rocketController.lineNames;
        rocketData.rocketName = rocketController.rocketName;
        rocketData.z_rot = rocketController.transform.eulerAngles.z;
        rocketData.state = rocketController.GetComponent<RocketStateManager>().state;
        rocketData.x_pos = rocketController.transform.position.x;
        rocketData.y_pos = rocketController.transform.position.y;
        rocketData.v_x = rocketController.GetComponent<Rigidbody2D>().velocity.x;
        rocketData.v_y = rocketController.GetComponent<Rigidbody2D>().velocity.y;
        if (rocketController.GetComponent<RocketStateManager>().state == "landed")
        {
            rocketData.x_pos = rocketController.transform.position.x;
            rocketData.y_pos = rocketController.transform.position.y;
        }
        else
        {
            //Should be converted to dounle
            rocketData.x_pos = rocketController.transform.position.x;
            rocketData.y_pos = rocketController.transform.position.y;
        }
        rocketData.curr_X = rocketController.GetComponent<RocketStateManager>().curr_X;
        rocketData.curr_Y = rocketController.GetComponent<RocketStateManager>().curr_Y;
        rocketData.prev_X = rocketController.GetComponent<RocketStateManager>().previous_X;
        rocketData.prev_Y = rocketController.GetComponent<RocketStateManager>().previous_Y;
        rocketData.keplerParams = rocketController.GetComponent<RocketPath>().KeplerParams;
        rocketData.Ho = rocketController.GetComponent<RocketPath>().Ho;
        rocketData.H = rocketController.GetComponent<RocketPath>().H;
        rocketData.Mo = rocketController.GetComponent<RocketPath>().Mo;
        rocketData.n = rocketController.GetComponent<RocketPath>().n;
        rocketData.e = rocketController.GetComponent<RocketPath>().e;
        rocketData.a = rocketController.GetComponent<RocketPath>().a;
        rocketData.i = rocketController.GetComponent<RocketPath>().i;
        rocketData.lastUpdatedTime = rocketController.GetComponent<RocketPath>().lastUpdatedTime;
        rocketData.startTime = rocketController.GetComponent<RocketPath>().startTime;
        rocketData.planetName = rocketController.GetComponent<PlanetGravity>().getPlanet().GetComponent<TypeScript>().type;


        rocketData.rootPart = new PartData();
        rocketData.rootPart.partType = rocketController.transform.GetChild(0).GetComponent<PhysicsPart>().type;
        rocketData.rootPart.fileName = rocketController.transform.GetChild(0).GetComponent<PhysicsPart>().path;
        rocketData.rootPart.x_pos = rocketController.transform.GetChild(0).localPosition.x;
        rocketData.rootPart.y_pos = rocketController.transform.GetChild(0).localPosition.y;
        rocketData.rootPart.z_rot = rocketController.transform.GetChild(0).eulerAngles.z;
        rocketData.rootPart.guid = rocketController.transform.GetChild(0).GetComponent<PhysicsPart>().guid;
        rocketData.rootPart.mass = rocketController.transform.GetChild(0).GetComponent<PhysicsPart>().mass;
        if (rocketController.transform.GetChild(0).GetComponent<PhysicsPart>().type == "tank")
        {
            rocketData.rootPart.x_scale = rocketController.transform.GetChild(0).GetComponent<TankComponent>().x_scale;
            rocketData.rootPart.y_scale = rocketController.transform.GetChild(0).GetComponent<TankComponent>().y_scale;
            rocketData.rootPart.conductivity = rocketController.transform.GetChild(0).GetComponent<TankComponent>().conductivity;
            rocketData.rootPart._volume = rocketController.transform.GetChild(0).GetComponent<TankComponent>()._volume;
            rocketData.rootPart.lineGuid = rocketController.transform.GetChild(0).GetComponent<TankComponent>().lineGuid;
            rocketData.rootPart.tested = rocketController.transform.GetChild(0).GetComponent<TankComponent>().tested;
        }
        if (rocketController.transform.GetChild(0).GetComponent<PhysicsPart>().type == "engine")
        {
            rocketData.rootPart.thrust = rocketController.transform.GetChild(0).GetComponent<EngineComponent>().maxThrust;
            rocketData.rootPart.massFlowRate = rocketController.transform.GetChild(0).GetComponent<EngineComponent>().maxFuelFlow;
            rocketData.rootPart.reliability = rocketController.transform.GetChild(0).GetComponent<EngineComponent>().reliability;

        }
        if(rocketController.transform.GetChild(0).GetComponent<PhysicsPart>().type == "capsule")
        {
            rocketData.rootPart.modules = new List<string>();
            rocketData.rootPart.modulePositionsX = new List<float>();
            rocketData.rootPart.modulePositionsY = new List<float>();
            rocketData.rootPart.moduleRotationsY = new List<float>();
            rocketData.rootPart.moduleRotationsZ = new List<float>();
            CapsuleComponent capsule = rocketController.transform.GetChild(0).GetComponent<CapsuleComponent>();
            foreach (CapsuleModuleComponent module in capsule.modules)
            {
                rocketData.rootPart.modules.Add(module.moduleName);
                rocketData.rootPart.modulePositionsX.Add(module.transform.localPosition.x);
                rocketData.rootPart.modulePositionsY.Add(module.transform.localPosition.y);
                rocketData.rootPart.moduleRotationsY.Add(module.transform.eulerAngles.y);
                rocketData.rootPart.moduleRotationsZ.Add(module.transform.eulerAngles.z);
            }
        }
        saveWorldChildren(rocketData.rootPart, rocketController.transform.GetChild(0).gameObject);

    }

    public void saveWorldChildren(PartData parent, GameObject parentObject)
    {
        foreach (Transform child in parentObject.transform)
        {
            if (child.GetComponent<PhysicsPart>() == null)
            {
                continue;
            }
            PartData newPart = new PartData();
            newPart.partType = child.GetComponent<PhysicsPart>().type;
            newPart.fileName = child.GetComponent<PhysicsPart>().path;
            newPart.x_pos = child.localPosition.x;
            newPart.y_pos = child.localPosition.y;
            newPart.z_rot = child.eulerAngles.z;
            newPart.guid = child.GetComponent<PhysicsPart>().guid;
            newPart.mass = child.GetComponent<PhysicsPart>().mass;
            if (child.GetComponent<PhysicsPart>().type == "tank")
            {
                newPart.x_scale = child.GetComponent<TankComponent>().x_scale;
                newPart.y_scale = child.GetComponent<TankComponent>().y_scale;
                newPart.conductivity = child.GetComponent<TankComponent>().conductivity;
                newPart._volume = child.GetComponent<TankComponent>()._volume;
                newPart.lineGuid = child.GetComponent<TankComponent>().lineGuid;
                newPart.tested = child.GetComponent<TankComponent>().tested;
            }
            if (child.GetComponent<PhysicsPart>().type == "engine")
            {
                newPart.thrust = child.GetComponent<EngineComponent>().maxThrust;
                newPart.massFlowRate = child.GetComponent<EngineComponent>().maxFuelFlow;
                newPart.reliability = child.GetComponent<EngineComponent>().reliability;
            }
            if(child.GetComponent<PhysicsPart>().type == "capsule")
            {
                newPart.modules = new List<string>();
                newPart.modulePositionsX = new List<float>();
                newPart.modulePositionsY = new List<float>();
                newPart.moduleRotationsY = new List<float>();
                newPart.moduleRotationsZ = new List<float>();
                CapsuleComponent capsule = child.GetComponent<CapsuleComponent>();
                foreach (CapsuleModuleComponent module in capsule.modules)
                {
                    newPart.modules.Add(module.moduleName);
                    newPart.modulePositionsX.Add(module.transform.localPosition.x);
                    newPart.modulePositionsY.Add(module.transform.localPosition.y);
                    newPart.moduleRotationsY.Add(module.transform.eulerAngles.y);
                    newPart.moduleRotationsZ.Add(module.transform.eulerAngles.z);
                }
            }
            parent.children.Add(newPart);
            saveWorldChildren(newPart, child.gameObject);
        }
    }

    public void loadWorldRocket(RocketData rocketData)
    {
        GameObject rocket = Instantiate(Resources.Load<GameObject>("Prefabs/RocketController"));
        RocketController rocketController = rocket.GetComponent<RocketController>();
        rocketController.InitializeComponents();
        GameObject moon = FindObjectOfType<MoonScript>().gameObject;
        GameObject earth = FindObjectOfType<EarthScript>().gameObject;
        GameObject sun = FindObjectOfType<SunScript>().gameObject;
        rocketController.rocketName = rocketData.rocketName;
        rocketController.lineNames = rocketData.lineNames;
        rocketController.lineGuids = rocketData.lineGuids;
        rocketController.transform.position = new Vector2(rocketData.x_pos, rocketData.y_pos);
        if ((float)rocketData.v_x != float.NaN && (float)rocketData.v_y != float.NaN)
        {
            rocketController.GetComponent<Rigidbody2D>().velocity = new Vector2((float)rocketData.v_x, (float)rocketData.v_y);
        }
        if (rocketData.state == "rail")
        {
            //Tricking state manager
            rocketController.GetComponent<RocketStateManager>().state = "simulate";
            rocketController.GetComponent<RocketStateManager>().previousState = "simulate";
            if (rocketData.planetName == "moon")
            {
                rocketController.GetComponent<PlanetGravity>().setPlanet(moon);
            }
            if (rocketData.planetName == "earth")
            {
                rocketController.GetComponent<PlanetGravity>().setPlanet(earth);
            }
            if (rocketData.planetName == "sun")
            {
                rocketController.GetComponent<PlanetGravity>().setPlanet(sun);
            }

            rocketController.GetComponent<RocketPath>().KeplerParams = rocketData.keplerParams;
            rocketController.GetComponent<RocketPath>().Ho = rocketData.Ho;
            rocketController.GetComponent<RocketPath>().H = rocketData.H;
            rocketController.GetComponent<RocketPath>().Mo = rocketData.Mo;
            rocketController.GetComponent<RocketPath>().n = rocketData.n;
            rocketController.GetComponent<RocketPath>().e = rocketData.e;
            rocketController.GetComponent<RocketPath>().a = rocketData.a;
            rocketController.GetComponent<RocketPath>().i = rocketData.i;
            UnityEngine.Debug.Log(rocketController.GetComponent<RocketPath>().e.ToString() + rocketController.GetComponent<RocketPath>().KeplerParams.eccentricity.ToString());
            rocketController.GetComponent<RocketPath>().lastUpdatedTime = rocketData.lastUpdatedTime;
            rocketController.GetComponent<RocketPath>().startTime = rocketData.startTime;

            rocketController.GetComponent<RocketPath>().bypass = true;

            rocketController.GetComponent<RocketPath>().planetGravity = rocketController.GetComponent<PlanetGravity>();
            rocketController.GetComponent<RocketPath>().rb = rocketController.GetComponent<Rigidbody2D>();
            rocketController.GetComponent<RocketStateManager>().curr_X = (float)rocketData.curr_X;
            rocketController.GetComponent<RocketStateManager>().curr_Y = (float)rocketData.curr_Y;
            rocketController.GetComponent<RocketStateManager>().previous_X = (float)rocketData.prev_X;
            rocketController.GetComponent<RocketStateManager>().previous_Y = (float)rocketData.prev_Y;
            rocketController.GetComponent<RocketPath>().updatePosition();
        }

        if (rocketData.state == "landed")
        {
            print("landed");
            rocketController.GetComponent<RocketStateManager>().curr_X = (float)rocketData.curr_X;
            rocketController.GetComponent<RocketStateManager>().curr_Y = (float)rocketData.curr_Y;
            rocketController.GetComponent<RocketStateManager>().previous_X = (float)rocketData.prev_X;
            rocketController.GetComponent<RocketStateManager>().previous_Y = (float)rocketData.prev_Y;
            rocketController.GetComponent<RocketStateManager>().previousState = "none";

            if (rocketData.planetName == "moon")
            {
                rocketController.transform.parent = moon.transform;
                rocketController.transform.localPosition = new Vector3((float)rocketData.x_pos, (float)rocketData.y_pos, 0);
                rocketController.GetComponent<DoubleTransform>().x_pos = rocketController.transform.position.x;
                rocketController.GetComponent<DoubleTransform>().y_pos = rocketController.transform.position.y;
                rocketController.GetComponent<RocketStateManager>().savedPlanet = moon;
            }
            if (rocketData.planetName == "earth")
            {
                rocketController.transform.parent = earth.transform;
                rocketController.transform.localPosition = new Vector3((float)rocketData.x_pos, (float)rocketData.y_pos, 0);
                rocketController.GetComponent<DoubleTransform>().x_pos = rocketController.transform.position.x;
                rocketController.GetComponent<DoubleTransform>().y_pos = rocketController.transform.position.y;
                rocketController.GetComponent<RocketStateManager>().savedPlanet = earth;

            }
        }

        if (rocketData.state == "simulate")
        {
            rocketController.GetComponent<RocketStateManager>().state = "simulate";
            rocketController.GetComponent<RocketStateManager>().previousState = "simulate";
            if (rocketData.planetName == "moon")
            {
                rocketController.GetComponent<PlanetGravity>().setPlanet(moon);
            }
            if (rocketData.planetName == "earth")
            {
                rocketController.GetComponent<PlanetGravity>().setPlanet(earth);
            }
            if (rocketData.planetName == "sun")
            {
                rocketController.GetComponent<PlanetGravity>().setPlanet(sun);
            }

            rocketController.GetComponent<RocketPath>().KeplerParams = rocketData.keplerParams;
            rocketController.GetComponent<RocketStateManager>().curr_X = (float)rocketData.curr_X;
            rocketController.GetComponent<RocketStateManager>().curr_Y = (float)rocketData.curr_Y;
            rocketController.GetComponent<RocketStateManager>().previous_X = (float)rocketData.prev_X;
            rocketController.GetComponent<RocketStateManager>().previous_Y = (float)rocketData.prev_Y;


        }
        GameObject newPart = Instantiate(Resources.Load<GameObject>("Prefabs/Modules/" + rocketData.rootPart.partType));
        if (rocketData.rootPart.partType == "tank")
        {
            newPart.GetComponent<PhysicsPart>().path = rocketData.rootPart.fileName;
            newPart.transform.localScale = new Vector3(rocketData.rootPart.x_scale, rocketData.rootPart.y_scale, 1);
            newPart.GetComponent<TankComponent>()._volume = rocketData.rootPart._volume;
            newPart.GetComponent<TankComponent>().x_scale = rocketData.rootPart.x_scale;
            newPart.GetComponent<TankComponent>().y_scale = rocketData.rootPart.y_scale;
            newPart.GetComponent<TankComponent>().conductivity = rocketData.rootPart.conductivity;
            newPart.GetComponent<PhysicsPart>().mass = rocketData.rootPart.mass;
            newPart.GetComponent<TankComponent>().lineGuid = rocketData.rootPart.lineGuid;
            newPart.GetComponent<TankComponent>().tested = rocketData.rootPart.tested;
        }
        if (rocketData.rootPart.partType == "engine")
        {
            newPart.GetComponent<PhysicsPart>().path = rocketData.rootPart.fileName;
            newPart.GetComponent<PhysicsPart>().mass = rocketData.rootPart.mass;
            newPart.GetComponent<EngineComponent>().maxThrust = rocketData.rootPart.thrust;
            newPart.GetComponent<EngineComponent>().maxFuelFlow = rocketData.rootPart.massFlowRate;
            newPart.GetComponent<EngineComponent>().reliability = rocketData.rootPart.reliability;

            var jsonString = File.ReadAllText(Application.persistentDataPath + savePathRef.worldsFolder + '/' + masterManager.FolderName + savePathRef.engineFolder + rocketData.rootPart.fileName);
            saveEngine loadedEngine = JsonConvert.DeserializeObject<saveEngine>(jsonString);
            newPart.GetComponent<EngineComponent>()._nozzleName = loadedEngine.nozzleName_s;
            newPart.GetComponent<EngineComponent>()._pumpName = loadedEngine.pumpName_s;
            newPart.GetComponent<EngineComponent>()._turbineName = loadedEngine.turbineName_s;

            newPart.GetComponent<EngineComponent>().InitializeSprite();
        }
        if(rocketData.rootPart.partType == "capsule")
        {
            newPart.GetComponent<PhysicsPart>().path = rocketData.rootPart.fileName;
            newPart.GetComponent<PhysicsPart>().mass = rocketData.rootPart.mass;
            newPart.GetComponent<CapsuleComponent>().modules = new List<CapsuleModuleComponent>();
            foreach (string module in rocketData.rootPart.modules)
            {
                if(module != "")
                {
                    GameObject newModule = Instantiate(Resources.Load<GameObject>("Prefabs/Modules/CapsuleModules/" + module));
                    newModule.transform.parent = newPart.transform;
                    newModule.transform.localPosition = new Vector3(rocketData.rootPart.modulePositionsX[rocketData.rootPart.modules.IndexOf(module)], rocketData.rootPart.modulePositionsY[rocketData.rootPart.modules.IndexOf(module)], 0);
                    newModule.transform.eulerAngles = new Vector3(newModule.transform.eulerAngles.x, rocketData.rootPart.moduleRotationsY[rocketData.rootPart.modules.IndexOf(module)], rocketData.rootPart.moduleRotationsZ[rocketData.rootPart.modules.IndexOf(module)]);
                    newModule.transform.parent = newPart.GetComponent<CapsuleComponent>().modules[rocketData.rootPart.modules.IndexOf(module)].transform;
                }
            }
        }
        newPart.transform.rotation = Quaternion.Euler(0, 0, rocketData.rootPart.z_rot);
        newPart.GetComponent<PhysicsPart>().guid = rocketData.rootPart.guid;
        newPart.transform.parent = rocketController.transform;
        newPart.transform.localPosition = new Vector2(0, 0);
        loadWorldChildren(rocketData.rootPart, newPart);
        rocketController.transform.rotation = Quaternion.Euler(0, 0, rocketData.z_rot);
    }

    public void loadWorldChildren(PartData parent, GameObject parentObject)
    {
        foreach (PartData child in parent.children)
        {
            GameObject newPart = Instantiate(Resources.Load<GameObject>("Prefabs/Modules/" + child.partType));
            if (child.partType == "tank")
            {
                newPart.GetComponent<PhysicsPart>().path = child.fileName;
                newPart.transform.localScale = new Vector3(child.x_scale, child.y_scale, 1);
                newPart.GetComponent<TankComponent>()._volume = child._volume;
                newPart.GetComponent<TankComponent>().x_scale = child.x_scale;
                newPart.GetComponent<TankComponent>().y_scale = child.y_scale;
                newPart.GetComponent<TankComponent>().conductivity = child.conductivity;
                newPart.GetComponent<PhysicsPart>().mass = child.mass;
                newPart.GetComponent<TankComponent>().lineGuid = child.lineGuid;
                newPart.GetComponent<TankComponent>().tested = child.tested;
            }
            if (child.partType == "engine")
            {
                newPart.GetComponent<PhysicsPart>().path = child.fileName;
                newPart.GetComponent<PhysicsPart>().mass = child.mass;
                newPart.GetComponent<EngineComponent>().maxThrust = child.thrust;
                newPart.GetComponent<EngineComponent>().maxFuelFlow = child.massFlowRate;
                newPart.GetComponent<EngineComponent>().reliability = child.reliability;

                var jsonString = File.ReadAllText(Application.persistentDataPath + savePathRef.worldsFolder + '/' + masterManager.FolderName + savePathRef.engineFolder + child.fileName);
                saveEngine loadedEngine = JsonConvert.DeserializeObject<saveEngine>(jsonString);
                newPart.GetComponent<EngineComponent>()._nozzleName = loadedEngine.nozzleName_s;
                newPart.GetComponent<EngineComponent>()._pumpName = loadedEngine.pumpName_s;
                newPart.GetComponent<EngineComponent>()._turbineName = loadedEngine.turbineName_s;

                newPart.GetComponent<EngineComponent>().InitializeSprite();
            }
            if(child.partType == "capsule")
            {
                newPart.GetComponent<PhysicsPart>().path = child.fileName;
                newPart.GetComponent<PhysicsPart>().mass = child.mass;
                newPart.GetComponent<CapsuleComponent>().modules = new List<CapsuleModuleComponent>();
                foreach (string module in child.modules)
                {
                    if(module != "")
                    {
                        GameObject newModule = Instantiate(Resources.Load<GameObject>("Prefabs/Modules/CapsuleModules/" + module));
                        newModule.transform.parent = newPart.transform;
                        newModule.transform.localPosition = new Vector3(child.modulePositionsX[child.modules.IndexOf(module)], child.modulePositionsY[child.modules.IndexOf(module)], 0);
                        newModule.transform.eulerAngles = new Vector3(newModule.transform.eulerAngles.x, child.moduleRotationsY[child.modules.IndexOf(module)], child.moduleRotationsZ[child.modules.IndexOf(module)]);
                        newModule.transform.parent = newPart.GetComponent<CapsuleComponent>().modules[child.modules.IndexOf(module)].transform;
                    }
                }
            }
            newPart.transform.rotation = Quaternion.Euler(0, 0, child.z_rot);
            newPart.GetComponent<PhysicsPart>().guid = child.guid;
            newPart.transform.parent = parentObject.transform;
            newPart.transform.localPosition = new Vector2(child.x_pos, child.y_pos);
            loadWorldChildren(child, newPart);
        }
    }
}
