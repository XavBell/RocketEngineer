using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using Newtonsoft.Json;

public class rocketSaveManager : MonoBehaviour
{
    public savePath savePathRef = new savePath();
    public MasterManager masterManager;
    public void Start()
    {
        masterManager = FindObjectOfType<MasterManager>();
    }

    public void saveRocket(RocketController rocketController, bool inEditor)
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

        //Get all children
        PartData rootPart = rocketData.rootPart;
        GameObject rootPartObject = originalPart;
        AddChildren(rootPart, rootPartObject);


        //Write file
        string saveUserPath = Application.persistentDataPath + "/rocket.json";
        string rocketData1 = JsonConvert.SerializeObject(rocketData);
        File.WriteAllText(saveUserPath, rocketData1);

    }

    public void loadRocket(RocketController rocketController, bool inEditor)
    {
        string saveUserPath = Application.persistentDataPath + "/rocket.json";
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
        originalPart.transform.localPosition = new Vector2(rocketData.rootPart.x_pos, rocketData.rootPart.y_pos);
        loadPartFromType(rocketData.rootPart.partType, originalPart, rocketData.rootPart, rocketController);
        LoadChildren(rocketData.rootPart, originalPart, rocketController);
    }

    public void LoadChildren(PartData parent, GameObject parentObject, RocketController rocketController)
    {
        foreach (PartData child in parent.children)
        {
            GameObject newPart = Instantiate(Resources.Load<GameObject>("Prefabs/Modules/" + child.partType));
            //Important to change rotation before assigning parent
            newPart.transform.rotation = Quaternion.Euler(0, 0, child.z_rot);
            newPart.transform.parent = parentObject.transform;
            newPart.transform.localPosition = new Vector2(child.x_pos, child.y_pos);
            newPart.GetComponent<PhysicsPart>().guid = child.guid;
            loadPartFromType(child.partType, newPart, child, rocketController);
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
    }

    public void loadDecoupler(GameObject part, PartData partData)
    {
        part.GetComponent<DecouplerComponent>().detachFromParent = partData.detachFromParent;
    }

    public void loadTank(GameObject part, PartData partData, RocketController rocketController)
    {
        part.GetComponent<PhysicsPart>().path = partData.fileName;
        if (part.GetComponent<TankComponent>().lineGuid != Guid.Empty)
        {
            part.GetComponent<TankComponent>().lineGuid = partData.lineGuid;
            part.GetComponent<TankComponent>().lineName = rocketController.lineNames[rocketController.lineGuids.IndexOf(partData.lineGuid)];
        }

        //Load tank from path
        var jsonString = File.ReadAllText(Application.persistentDataPath + savePathRef.worldsFolder + '/' + masterManager.FolderName + savePathRef.tankFolder + partData.fileName);
        saveTank loadedTank = JsonConvert.DeserializeObject<saveTank>(jsonString);
        part.transform.localScale = new Vector3(loadedTank.tankSizeX, loadedTank.tankSizeY, 1);
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
        part.GetComponent<EngineComponent>().InitializeSprite();
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
    }

    public void saveDecoupler(GameObject part, PartData partData)
    {
        partData.detachFromParent = part.GetComponent<DecouplerComponent>().detachFromParent;
    }

    public void saveTank(GameObject part, PartData partData)
    {
        partData.fileName = part.GetComponent<PhysicsPart>().path;
        partData.lineGuid = part.GetComponent<TankComponent>().lineGuid;
    }

    public void saveEngine(GameObject part, PartData partData)
    {
        partData.fileName = part.GetComponent<PhysicsPart>().path;
    }
}
