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
    public GameObject pipePrefab;
    public GameObject fuelTankPrefab;
    public GameObject launchPadPrefab;

    public savePath savePathRef = new savePath();

    public GameObject earth;
    public GameObject moon;

    public GameObject worldCamera;

    public bool loaded = false;

    public GameObject MasterManager;
    public GameObject BuildingManager;

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

            if(building.GetComponent<buildingType>().type == "pipe")
            {
                saveWorld.buildingScaleX.Add(building.GetComponent<SpriteRenderer>().size.x);
                saveWorld.buildingScaleY.Add(building.GetComponent<SpriteRenderer>().size.y);

                saveWorld.inputLocX.Add(building.GetComponent<outputInputManager>().input.transform.position.x);
                saveWorld.inputLocY.Add(building.GetComponent<outputInputManager>().input.transform.position.y);
                saveWorld.inputLocZ.Add(building.GetComponent<outputInputManager>().input.transform.position.z);

                saveWorld.outputLocX.Add(building.GetComponent<outputInputManager>().output.transform.position.x);
                saveWorld.outputLocY.Add(building.GetComponent<outputInputManager>().output.transform.position.y);
                saveWorld.outputLocZ.Add(building.GetComponent<outputInputManager>().output.transform.position.z);

                saveWorld.inputIDs.Add(building.GetComponent<outputInputManager>().inputParentID);
                saveWorld.outputIDs.Add(building.GetComponent<outputInputManager>().outputParentID);
            }
        }

        GameObject[] rockets = GameObject.FindGameObjectsWithTag("capsule");
        int i = 0;
        foreach(GameObject rocket in rockets)
        {
            saveWorld.childrenNumber.Add(0);
            
            //Save capsule propreties
            saveWorld.capsuleType = rocket.GetComponent<Part>().type;
            saveWorld.capsuleInputParentIDs.Add(rocket.GetComponent<outputInputManager>().inputParentID);
            saveWorld.connectedAsRocket.Add(rocket.GetComponent<outputInputManager>().connectedAsRocket);
            saveWorld.capsuleVolume.Add(rocket.GetComponent<outputInputManager>().volume);
            saveWorld.capsuleMoles.Add(rocket.GetComponent<outputInputManager>().moles);
            saveWorld.capsuleLocX.Add(rocket.transform.position.x);
            saveWorld.capsuleLocY.Add(rocket.transform.position.y);
            saveWorld.capsuleLocZ.Add(rocket.transform.position.z);

            saveWorld.capsuleScaleX.Add(rocket.transform.localScale.x);
            saveWorld.capsuleScaleY.Add(rocket.transform.localScale.y);
            saveWorld.capsuleScaleZ.Add(rocket.transform.localScale.z);

            saveWorld.capsuleSpeedX.Add(rocket.GetComponent<PlanetGravity>().rb.velocity.x);
            saveWorld.capsuleSpeedY.Add(rocket.GetComponent<PlanetGravity>().rb.velocity.y);

            saveWorld.capsuleRotX.Add(rocket.transform.eulerAngles.x);
            saveWorld.capsuleRotY.Add(rocket.transform.eulerAngles.y);
            saveWorld.capsuleRotZ.Add(rocket.transform.eulerAngles.z);

            saveWorld.rocketMass.Add(rocket.GetComponent<PlanetGravity>().rocketMass);
            saveWorld.maxFuel.Add(rocket.GetComponent<PlanetGravity>().maxFuel);
            saveWorld.currentFuel.Add(rocket.GetComponent<PlanetGravity>().currentFuel);

            saveWorld.stageUpdated.Add(rocket.GetComponent<PlanetGravity>().stageUpdated);

            if(rocket.GetComponent<Part>().attachBottom.GetComponent<AttachPointScript>().attachedBody != null)
            {
                GameObject referenceBody = rocket;
                while(referenceBody.GetComponent<Part>().attachBottom.GetComponent<AttachPointScript>().attachedBody != null)
                {
                    string currentType = referenceBody.GetComponent<Part>().attachBottom.GetComponent<AttachPointScript>().attachedBody.GetComponent<Part>().type;
                    saveWorld.types.Add(currentType);
                    GameObject currentPrefab = referenceBody.GetComponent<Part>().attachBottom.GetComponent<AttachPointScript>().attachedBody;
                    if(currentType == "tank")
                    {
                        saveWorld.tankLocX.Add(currentPrefab.transform.localPosition.x);
                        saveWorld.tankLocY.Add(currentPrefab.transform.localPosition.y);
                        saveWorld.tankLocZ.Add(currentPrefab.transform.localPosition.z);

                        saveWorld.tankScaleX.Add(currentPrefab.GetComponent<Part>().tank.GetComponent<SpriteRenderer>().size.x);
                        saveWorld.tankScaleY.Add(currentPrefab.GetComponent<Part>().tank.GetComponent<SpriteRenderer>().size.y);
                        saveWorld.tankScaleZ.Add(currentPrefab.GetComponent<Part>().tank.GetComponent<SpriteRenderer>().transform.localScale.z);

                        GameObject attachTopObj = currentPrefab.gameObject.transform.GetChild(2).gameObject;
                        saveWorld.tankAttachTopLocX.Add(attachTopObj.transform.localPosition.x);
                        saveWorld.tankAttachTopLocY.Add(attachTopObj.transform.localPosition.y);
                        saveWorld.tankAttachTopLocZ.Add(attachTopObj.transform.localPosition.z);

                        GameObject attachBottomObj = currentPrefab.gameObject.transform.GetChild(1).gameObject;
                        saveWorld.tankAttachBottomLocX.Add(attachBottomObj.transform.localPosition.x);
                        saveWorld.tankAttachBottomLocY.Add(attachBottomObj.transform.localPosition.y);
                        saveWorld.tankAttachBottomLocZ.Add(attachBottomObj.transform.localPosition.z);
                    }

                    if(currentType == "engine")
                    {
                        saveWorld.engineLocX.Add(currentPrefab.transform.localPosition.x);
                        saveWorld.engineLocY.Add(currentPrefab.transform.localPosition.y);
                        saveWorld.engineLocZ.Add(currentPrefab.transform.localPosition.z);

                        saveWorld.engineScaleX.Add(currentPrefab.transform.localScale.x);
                        saveWorld.engineScaleY.Add(currentPrefab.transform.localScale.y);
                        saveWorld.engineScaleZ.Add(currentPrefab.transform.localScale.z);

                        saveWorld.engineFuel.Add(currentPrefab.GetComponent<Part>().fuel);
                        saveWorld.engineRate.Add(currentPrefab.GetComponent<Part>().rate);
                        saveWorld.engineMaxThrust.Add(currentPrefab.GetComponent<Part>().maxThrust);

                        GameObject attachTopObj = currentPrefab.gameObject.transform.GetChild(0).gameObject;
                        saveWorld.engineAttachTopLocX.Add(attachTopObj.transform.localPosition.x);
                        saveWorld.engineAttachTopLocY.Add(attachTopObj.transform.localPosition.y);
                        saveWorld.engineAttachTopLocZ.Add(attachTopObj.transform.localPosition.z);

                        GameObject attachBottomObj = currentPrefab.gameObject.transform.GetChild(1).gameObject;
                        saveWorld.engineAttachBottomLocX.Add(attachBottomObj.transform.localPosition.x);
                        saveWorld.engineAttachBottomLocY.Add(attachBottomObj.transform.localPosition.y);
                        saveWorld.engineAttachBottomLocZ.Add(attachBottomObj.transform.localPosition.z); 

                        GameObject nozzleExitRef = currentPrefab.GetComponent<Part>().nozzleExit;
                        saveWorld.nozzleExitSizeX.Add(nozzleExitRef.GetComponent<SpriteRenderer>().transform.localScale.x);
                        saveWorld.nozzleExitSizeY.Add(nozzleExitRef.GetComponent<SpriteRenderer>().transform.localScale.y);
                        saveWorld.nozzleExitLocY.Add(nozzleExitRef.transform.localPosition.y);

                        GameObject nozzleEndRef = currentPrefab.GetComponent<Part>().nozzleEnd;
                        saveWorld.nozzleEndSizeX.Add(nozzleEndRef.GetComponent<SpriteRenderer>().transform.localScale.x);

                        GameObject turbopump = currentPrefab.GetComponent<Part>().turbopump;
                        saveWorld.turbopumpSizeX.Add(turbopump.GetComponent<SpriteRenderer>().transform.localScale.x);
                    }

                    if(currentType == "decoupler")
                    {
                        saveWorld.decouplerLocX.Add(currentPrefab.transform.localPosition.x);
                        saveWorld.decouplerLocY.Add(currentPrefab.transform.localPosition.y);
                        saveWorld.decouplerLocZ.Add(currentPrefab.transform.localPosition.z);
                    }

                    referenceBody = referenceBody.GetComponent<Part>().attachBottom.GetComponent<AttachPointScript>().attachedBody;
                    saveWorld.childrenNumber[i]++; 
                }
            }
            i++;
        }

        var jsonString = JsonConvert.SerializeObject(saveWorld);
        System.IO.File.WriteAllText(MasterManager.GetComponent<MasterManager>().worldPath, jsonString);
        UnityEngine.Debug.Log(MasterManager.GetComponent<MasterManager>().worldPath);
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

        int pipeCount = 0;
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

            if(buildingType == "pipe")
            {
                GameObject current = Instantiate(pipePrefab, new Vector3(0, 0, 0), Quaternion.identity);
                current.transform.SetParent(earth.transform);
                current.transform.localPosition = position;
                current.transform.eulerAngles = rotation;
                current.GetComponent<buildingType>().buildingID = loadedWorld.buildingIDs[count];
                current.GetComponent<outputInputManager>().inputParentID = loadedWorld.inputIDs[count];
                current.GetComponent<outputInputManager>().outputParentID = loadedWorld.outputIDs[count];

                Vector2 size = new Vector2(loadedWorld.buildingScaleX[pipeCount], loadedWorld.buildingScaleY[pipeCount]);
                current.GetComponent<SpriteRenderer>().size = size;

                Vector3 inputPos = new Vector3(loadedWorld.inputLocX[pipeCount], loadedWorld.inputLocY[pipeCount], loadedWorld.inputLocZ[pipeCount]);
                Vector3 outputPos = new Vector3(loadedWorld.outputLocX[pipeCount], loadedWorld.outputLocY[pipeCount], loadedWorld.outputLocZ[pipeCount]);
                current.GetComponent<outputInputManager>().input.transform.position = inputPos;
                current.GetComponent<outputInputManager>().output.transform.position = outputPos;
                pipeCount++;
            }

            count++;
        }

        

        foreach(int rocket in loadedWorld.childrenNumber)
        {
            GameObject capsule = null;
            int childrenNumber = rocket;
            if(loadedWorld.capsuleType == "capsule")
            {
                capsule = Instantiate(capsulePrefab, Vector3.zero, Quaternion.identity);
            }
            setPosition(loadedWorld.capsuleLocX[capsuleID], loadedWorld.capsuleLocY[capsuleID], loadedWorld.capsuleLocZ[capsuleID], capsule);
            capsule.transform.rotation = Quaternion.Euler(loadedWorld.capsuleRotX[capsuleID], loadedWorld.capsuleRotY[capsuleID], loadedWorld.capsuleRotZ[capsuleID]);
            GameObject currentPrefab = capsule;
            currentPrefab.GetComponent<PlanetGravity>().posUpdated = true;
            currentPrefab.GetComponent<PlanetGravity>().stageUpdated = loadedWorld.stageUpdated[capsuleID];
            currentPrefab.GetComponent<PlanetGravity>().rb = currentPrefab.GetComponent<Rigidbody2D>();

            currentPrefab.GetComponent<PlanetGravity>().rocketMass = loadedWorld.rocketMass[capsuleID];
            currentPrefab.GetComponent<PlanetGravity>().currentFuel = loadedWorld.currentFuel[capsuleID];
            currentPrefab.GetComponent<PlanetGravity>().maxFuel = loadedWorld.maxFuel[capsuleID];

            currentPrefab.GetComponent<PlanetGravity>().rb.velocity = new Vector2(loadedWorld.capsuleSpeedX[capsuleID], loadedWorld.capsuleSpeedY[capsuleID]);
            
            currentPrefab.GetComponent<outputInputManager>().inputParentID = loadedWorld.capsuleInputParentIDs[capsuleID];
            currentPrefab.GetComponent<outputInputManager>().connectedAsRocket = loadedWorld.connectedAsRocket[capsuleID];
            currentPrefab.GetComponent<outputInputManager>().volume = loadedWorld.capsuleVolume[capsuleID];
            currentPrefab.GetComponent<outputInputManager>().moles = loadedWorld.capsuleMoles[capsuleID];

            bool decouplerPresent = false;
            int i = 0;
            while(i < childrenNumber)
            {
                GameObject previousPrefab = currentPrefab;
                if(loadedWorld.types[i + alreadyUsed] == "tank")
                {
                    currentPrefab = Instantiate(tankPrefab, previousPrefab.GetComponent<Part>().attachBottom.transform.position, Quaternion.identity);
                    currentPrefab.transform.SetParent(capsule.transform);
                    currentPrefab.transform.localRotation = Quaternion.Euler(0, 0, 0);
                    previousPrefab.GetComponent<Part>().attachBottom.GetComponent<AttachPointScript>().attachedBody = currentPrefab;
                    currentPrefab.GetComponent<Part>().attachTop.GetComponent<AttachPointScript>().attachedBody = previousPrefab;
                    setPosition(loadedWorld.tankLocX[tankCount], loadedWorld.tankLocY[tankCount], loadedWorld.tankLocZ[tankCount], currentPrefab);
                    currentPrefab.GetComponent<Part>().tank.GetComponent<SpriteRenderer>().size = new Vector2(loadedWorld.tankScaleX[tankCount], loadedWorld.tankScaleY[tankCount]);

                    GameObject attachTopObj = currentPrefab.gameObject.transform.GetChild(2).gameObject;
                    setPosition(loadedWorld.tankAttachTopLocX[tankCount], loadedWorld.tankAttachTopLocY[tankCount], loadedWorld.tankAttachTopLocZ[tankCount], attachTopObj);

                    GameObject attachBottomObj = currentPrefab.gameObject.transform.GetChild(1).gameObject;
                    setPosition(loadedWorld.tankAttachBottomLocX[tankCount], loadedWorld.tankAttachBottomLocY[tankCount], loadedWorld.tankAttachBottomLocZ[tankCount], attachBottomObj);

                    GameObject newPrefabDetach = currentPrefab;
                    if (decouplerPresent == true)
                    {
                        if (currentPrefab.GetComponent<Part>().attachTop.GetComponent<AttachPointScript>().attachedBody.GetComponent<Part>().type.ToString() == "decoupler")
                        {
                            currentPrefab.GetComponent<Part>().referenceDecoupler = newPrefabDetach.GetComponent<Part>().attachTop.GetComponent<AttachPointScript>().attachedBody;
                        }

                        if (currentPrefab.GetComponent<Part>().attachTop.GetComponent<AttachPointScript>().attachedBody.GetComponent<Part>().type.ToString() != "decoupler")
                        {
                            while (currentPrefab.GetComponent<Part>().referenceDecoupler == null)
                            {
                                newPrefabDetach = newPrefabDetach.GetComponent<Part>().attachTop.GetComponent<AttachPointScript>().attachedBody;
                                if (newPrefabDetach.GetComponent<Part>().type.ToString() == "decoupler")
                                {
                                    currentPrefab.GetComponent<Part>().referenceDecoupler = newPrefabDetach;
                                }
                            }

                        }

                    }

                    tankCount++;
                }

                if(loadedWorld.types[i + alreadyUsed] == "engine")
                {
                    currentPrefab = Instantiate(enginePrefab, previousPrefab.GetComponent<Part>().attachBottom.transform.position, Quaternion.identity);
                    currentPrefab.transform.SetParent(capsule.transform);
                    currentPrefab.transform.localRotation = Quaternion.Euler(0, 0, 0);
                    previousPrefab.GetComponent<Part>().attachBottom.GetComponent<AttachPointScript>().attachedBody = currentPrefab;
                    currentPrefab.GetComponent<Part>().attachTop.GetComponent<AttachPointScript>().attachedBody = previousPrefab;

                    currentPrefab.GetComponent<Part>().fuel = loadedWorld.engineFuel[engineCount];
                    currentPrefab.GetComponent<Part>().maxThrust = loadedWorld.engineMaxThrust[engineCount];
                    currentPrefab.GetComponent<Part>().rate = loadedWorld.engineRate[engineCount];

                    setPosition(loadedWorld.engineLocX[engineCount], loadedWorld.engineLocY[engineCount], loadedWorld.engineLocZ[engineCount], currentPrefab);

                    GameObject attachTopObj = currentPrefab.gameObject.transform.GetChild(0).gameObject;
                    setPosition(loadedWorld.engineAttachTopLocX[engineCount], loadedWorld.engineAttachTopLocY[engineCount], loadedWorld.engineAttachTopLocZ[engineCount], attachTopObj);

                    GameObject attachBottomObj = currentPrefab.gameObject.transform.GetChild(1).gameObject;
                    setPosition(loadedWorld.engineAttachBottomLocX[engineCount], loadedWorld.engineAttachBottomLocY[engineCount], loadedWorld.engineAttachBottomLocZ[engineCount], attachBottomObj);

                    GameObject nozzleExitRef = currentPrefab.GetComponent<Part>().nozzleExit;
                    nozzleExitRef.GetComponent<SpriteRenderer>().transform.localScale = new Vector2(loadedWorld.nozzleExitSizeX[engineCount], loadedWorld.nozzleExitSizeY[engineCount]);
                    nozzleExitRef.transform.localPosition = new Vector2(nozzleExitRef.transform.localPosition.x, loadedWorld.nozzleExitLocY[engineCount]);

                    GameObject nozzleEndRef = currentPrefab.GetComponent<Part>().nozzleEnd;
                    nozzleEndRef.GetComponent<SpriteRenderer>().transform.localScale = new Vector2(loadedWorld.nozzleEndSizeX[engineCount], nozzleEndRef.GetComponent<SpriteRenderer>().transform.localScale.y);

                    GameObject turbopump = currentPrefab.GetComponent<Part>().turbopump;
                    turbopump.GetComponent<SpriteRenderer>().transform.localScale = new Vector2(loadedWorld.turbopumpSizeX[engineCount], turbopump.GetComponent<SpriteRenderer>().transform.localScale.y);

                    capsule.GetComponent<PlanetGravity>().activeEngine = currentPrefab;


                    GameObject newPrefabDetach = currentPrefab;
                    if (decouplerPresent == true)
                    {
                        if (currentPrefab.GetComponent<Part>().attachTop.GetComponent<AttachPointScript>().attachedBody.GetComponent<Part>().type.ToString() == "decoupler")
                        {
                            currentPrefab.GetComponent<Part>().referenceDecoupler = newPrefabDetach.GetComponent<Part>().attachTop.GetComponent<AttachPointScript>().attachedBody;
                        }

                        if (currentPrefab.GetComponent<Part>().attachTop.GetComponent<AttachPointScript>().attachedBody.GetComponent<Part>().type.ToString() != "decoupler")
                        {
                            while (currentPrefab.GetComponent<Part>().referenceDecoupler == null)
                            {
                                newPrefabDetach = newPrefabDetach.GetComponent<Part>().attachTop.GetComponent<AttachPointScript>().attachedBody;
                                if (newPrefabDetach.GetComponent<Part>().type.ToString() == "decoupler")
                                {
                                    currentPrefab.GetComponent<Part>().referenceDecoupler = newPrefabDetach;
                                }
                            }
                        }
                    }
                    engineCount++;
                }

                if(loadedWorld.types[i + alreadyUsed] == "decoupler")
                {
                    currentPrefab = Instantiate(decouplerPrefab, previousPrefab.GetComponent<Part>().attachBottom.transform.position, Quaternion.identity);
                    currentPrefab.transform.SetParent(capsule.transform);
                    currentPrefab.transform.localRotation = Quaternion.Euler(0, 0, 0);
                    previousPrefab.GetComponent<Part>().attachBottom.GetComponent<AttachPointScript>().attachedBody = currentPrefab;
                    currentPrefab.GetComponent<Part>().attachTop.GetComponent<AttachPointScript>().attachedBody = previousPrefab;
                    setPosition(loadedWorld.decouplerLocX[decouplerCount], loadedWorld.decouplerLocY[decouplerCount], loadedWorld.decouplerLocZ[decouplerCount], currentPrefab);

                    decouplerPresent = true;
                    decouplerCount++;
                }
                i++;
            }
            capsule.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            capsuleID++;
            alreadyUsed += childrenNumber;
        }
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
