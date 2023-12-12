using System.Runtime.InteropServices.ComTypes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;
using System.Runtime.Serialization;
using System.Xml.Linq;
using System.Text;
using Newtonsoft.Json;
public class OnClick : MonoBehaviour
{
    public Button b1;
    public GameObject tankPrefab;
    public GameObject enginePrefab;
    public GameObject launchPad;

    public GameObject capsulePrefab;
    public GameObject Tank;
    public GameObject Engine;
    public GameObject Decoupler;

    public savePath savePathRef = new savePath();
    public string path;
    public string filePath;
    public GameObject spawnedRocket;
    public operationManager op;
    public GameObject savedLaunchpad;

    public outputInputManager oxidizerInput;
    public outputInputManager fuelInput;
    public outputInputManager output;
    public FuelConnectorManager fcm;
    public GameObject inputUI;
    public GameObject outputUI;
    public GameObject selectUI;

    // Start is called before the first frame update
    void Start()
    {
        GameObject GameManager = GameObject.FindGameObjectWithTag("GameManager");
    }

    // Update is called once per frame
    void Update()
    {
        if(SceneManager.GetActiveScene().name.ToString() == "Menu")
        {
           GameObject MasterManager = GameObject.FindGameObjectWithTag("MasterManager");
           if(b1.GetComponentInChildren<TextMeshProUGUI>().text.ToString() != MasterManager.GetComponent<MasterManager>().FolderName)
           {
                b1.interactable = true;
           }
        }

        if(SceneManager.GetActiveScene().name.ToString() == "Building")
        {
           GameObject GameManager = GameObject.FindGameObjectWithTag("GameManager");
           if("/" + b1.GetComponentInChildren<TextMeshProUGUI>().text != GameManager.GetComponent<GameManager>().path || GameManager.GetComponent<GameManager>().partPath != filePath)
           {
                b1.interactable = true;
           }
        }
    }

    public void clicked()
    {
        GameObject GameManager = GameObject.FindGameObjectWithTag("GameManager");
        GameObject MasterManager = GameObject.FindGameObjectWithTag("MasterManager");
        if (GameManager != null)
        {
            GameManager.GetComponent<GameManager>().path = "/"+ b1.GetComponentInChildren<TextMeshProUGUI>().text;
            if (filePath == savePathRef.engineFolder)
            {
                GameManager.GetComponent<GameManager>().partPath = filePath;
                GameManager.GetComponent<GameManager>().PrefabToConstruct = GameManager.GetComponent<GameManager>().Engine;
                b1.interactable = false;
            }

            if (filePath == savePathRef.rocketFolder)
            {
                GameManager.GetComponent<GameManager>().partPath = filePath;
                b1.interactable = false;
            }

            if (filePath == savePathRef.tankFolder)
            {
                GameManager.GetComponent<GameManager>().partPath = filePath;
                GameManager.GetComponent<GameManager>().PrefabToConstruct = GameManager.GetComponent<GameManager>().Tank;
                b1.interactable = false;
            }

        }

        if(SceneManager.GetActiveScene().name.ToString() == "Menu")
        {
            MasterManager.GetComponent<MasterManager>().FolderName = b1.GetComponentInChildren<TextMeshProUGUI>().text;
            b1.interactable = false;
        }

        if(launchPad != null)
        {
            path = "/"+b1.GetComponentInChildren<TextMeshProUGUI>().text;
            load(filePath);
            if(spawnedRocket != null)
            {
                launchPad.GetComponent<launchPadManager>().ConnectedRocket = spawnedRocket;
            }
            b1.interactable = false;

        }   
        
    }

    public void load(string fileTypePath)
    {
        GameObject MasterManagerGO = GameObject.FindGameObjectWithTag("MasterManager");
        MasterManager MasterManager = MasterManagerGO.GetComponent<MasterManager>();
        savecraft saveObject = new savecraft();

        GameObject core = null;

        if (fileTypePath == savePathRef.rocketFolder)
        {
            var jsonString = JsonConvert.SerializeObject(saveObject);
            jsonString = File.ReadAllText(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + fileTypePath + path);
            savecraft loadedRocket = JsonConvert.DeserializeObject<savecraft>(jsonString);
            GameObject temp = new GameObject();
            Rocket rocket = temp.AddComponent<Rocket>();
            int maxSteps = loadedRocket.PartsID.Count;
            int maxStage = Mathf.Max(loadedRocket.StageNumber.ToArray());

            //Create stages
            for(int i = 0; i <= maxStage; i++)
            {
                Stages stage = new Stages();
                rocket.Stages.Add(stage);
            }

            Vector2 position = new Vector2(launchPad.transform.position.x, launchPad.transform.position.y + 10);

            List<System.Guid> partsGuid = new List<System.Guid>();
            List<System.Guid> topGuid = new List<System.Guid>();
            List<System.Guid> bottomGuid = new List<System.Guid>();
            List<System.Guid> leftGuid = new List<System.Guid>();
            List<System.Guid> rightGuid = new List<System.Guid>();
            List<GameObject> rocketPart = new List<GameObject>();

            //Put parts in stages
            for(int i = 0; i < maxSteps; i++)
            {
                Vector3 rotation = new Vector3(0, 0, loadedRocket.z_rot[i]);
                GameObject currentPart = SpawnPart(position, rotation, loadedRocket.partType[i]);
                RocketPart part = currentPart.GetComponent<RocketPart>();

                part._partID = loadedRocket.PartsID[i];
                part._partMass = loadedRocket.mass[i];
                
                //Add part to stage
                rocket.Stages[loadedRocket.StageNumber[i]].Parts.Add(part);
                rocket.Stages[loadedRocket.StageNumber[i]].PartsID.Add(part._partID);

                if(loadedRocket.coreID == loadedRocket.PartsID[i])
                {
                    currentPart.AddComponent<Rocket>();
                    rocket.core = currentPart;
                    core = currentPart;
                    //newParts.Remove(currentPart);
                }

                topGuid.Add(loadedRocket.attachedTop[i]);
                bottomGuid.Add(loadedRocket.attachedBottom[i]);
                rightGuid.Add(loadedRocket.attachedRight[i]);
                leftGuid.Add(loadedRocket.attachedLeft[i]);
                partsGuid.Add(part._partID);
                rocketPart.Add(part.gameObject);
            }
            
 
            core.AddComponent<Rigidbody2D>();
            core.GetComponent<Rigidbody2D>().angularDrag = 0;
            core.GetComponent<Rigidbody2D>().freezeRotation = true;
            core.GetComponent<Rigidbody2D>().gravityScale = 0;
            core.AddComponent<PlanetGravity>();
            core.GetComponent<PlanetGravity>().core = core;
            core.GetComponent<Rocket>().core = core;
            core.AddComponent<DoubleTransform>();
            core.AddComponent<RocketStateManager>();
            core.AddComponent<RocketPath>();
            
            foreach(Stages stage in rocket.Stages)
            {
                core.GetComponent<Rocket>().Stages.Add(stage);
            }

            linkParts(rocketPart, loadedRocket.attachedTop,loadedRocket.attachedBottom,loadedRocket.attachedRight, loadedRocket.attachedLeft);

            //Parent core and set values to proper parts
            int j = 0;
            int tankID = 0;
            int engineID = 0;
            foreach(GameObject part in rocketPart)
            {
                if(part != core)
                {
                    part.transform.SetParent(core.transform);
                    part.transform.position = new UnityEngine.Vector3(loadedRocket.x_pos[j] + core.transform.position.x, loadedRocket.y_pos[j]+ core.transform.position.y, loadedRocket.z_pos[j]+ core.transform.position.z);

                    if(part.GetComponent<RocketPart>()._partType == "tank")
                    {
                        part.transform.localScale = new UnityEngine.Vector2(loadedRocket.x_scale[tankID], loadedRocket.y_scale[tankID]);
                        part.GetComponent<Tank>()._volume = loadedRocket.volume[tankID];
                        part.GetComponent<Tank>().tankMaterial = loadedRocket.tankMaterial[tankID];
                        part.GetComponent<Tank>().propellantCategory = loadedRocket.propellantType[tankID];
                        tankID++;
                    }

                    if(part.GetComponent<RocketPart>()._partType == "engine")
                    {
                        part.GetComponent<Engine>()._thrust = loadedRocket.thrust[engineID];
                        part.GetComponent<Engine>()._rate = loadedRocket.flowRate[engineID];
                        engineID++;
                    }
                }
                j++;
            }
            DestroyImmediate(temp);
            
        }
        spawnedRocket = core;
        filePath = null;
    }

    public void linkParts(List<GameObject> parts, List<System.Guid> topRef, List<System.Guid> bottomRef, List<System.Guid> rightRef, List<System.Guid> leftRef)
    {
        int i = 0;
        foreach(GameObject part in parts)
        {
            System.Guid partID = part.GetComponent<RocketPart>()._partID;

            bool top = topRef.Contains(partID);
            bool bottom = bottomRef.Contains(partID);
            bool left = leftRef.Contains(partID);
            bool right = rightRef.Contains(partID);

            if(top == true)
            {
                List<int> pos = new List<int>();
                int l = 0;
                foreach(System.Guid id in topRef)
                {
                    if(id == partID)
                    {
                        pos.Add(l);
                    }
                    l++;
                }

                foreach(int position in pos)
                {
                    parts[position].GetComponent<RocketPart>()._attachTop.GetComponent<AttachPointScript>().attachedBody = part;
                }
            }

            if(bottom == true)
            {
                List<int> pos = new List<int>();
                int l = 0;
                foreach(System.Guid id in bottomRef)
                {
                    if(id == partID)
                    {
                        pos.Add(l);
                    }
                    l++;
                }

                foreach(int position in pos)
                {
                    parts[position].GetComponent<RocketPart>()._attachBottom.GetComponent<AttachPointScript>().attachedBody = part;
                }
            }

            if(left == true)
            {
                List<int> pos = new List<int>();
                int l = 0;
                foreach(System.Guid id in leftRef)
                {
                    if(id == partID)
                    {
                        pos.Add(l);
                    }
                    l++;
                }

                foreach(int position in pos)
                {
                    parts[position].GetComponent<RocketPart>()._attachLeft.GetComponent<AttachPointScript>().attachedBody = part;
                }
            }

            if(right == true)
            {
                List<int> pos = new List<int>();
                int l = 0;
                foreach(System.Guid id in rightRef)
                {
                    if(id == partID)
                    {
                        pos.Add(l);
                    }
                    l++;
                }

                foreach(int position in pos)
                {
                    parts[position].GetComponent<RocketPart>()._attachRight.GetComponent<AttachPointScript>().attachedBody = part;
                }
            }
            i++;
        }
    }

    GameObject SpawnPart(UnityEngine.Vector3 position, UnityEngine.Vector3 rotation, string type)
    {
        GameObject go = null;
        if(type == "satellite")
        {
            go = Instantiate(capsulePrefab, position, UnityEngine.Quaternion.Euler(0, 0, rotation.z));
        }
        if(type == "engine")
        {
            go = Instantiate(Engine, position, UnityEngine.Quaternion.Euler(0, 0, rotation.z));
        }
        if(type == "tank")
        {
            go = Instantiate(Tank, position, UnityEngine.Quaternion.Euler(0, 0, rotation.z));

        }if(type == "decoupler")
        {
            go = Instantiate(Decoupler, position, UnityEngine.Quaternion.Euler(0, 0, rotation.z));
        }

        return go;
    }

    public void setRocketValues(AttachPointScript attachPoint, GameObject currentPrefab, GameObject capsule)
    {

        attachPoint.GetComponent<AttachPointScript>().attachedBody = currentPrefab;
        currentPrefab.GetComponent<Part>().attachTop.GetComponent<AttachPointScript>().attachedBody = attachPoint.GetComponent<AttachPointScript>().referenceBody;
        Vector3 difference = currentPrefab.transform.position - currentPrefab.GetComponent<Part>().attachTop.transform.position;
        currentPrefab.transform.position = attachPoint.transform.position + difference;
        currentPrefab.transform.SetParent(capsule.transform);
        if(currentPrefab.GetComponent<Part>().type == "engine")
        {
            capsule.GetComponent<PlanetGravity>().particle.transform.position = currentPrefab.transform.position;
        }
        capsule.GetComponent<PlanetGravity>().rocketMass += currentPrefab.GetComponent<Part>().mass;
    }

    public void AddFuel(GameObject tank)
    {
        tank.GetComponent<outputInputManager>().substance = "kerosene";
        tank.GetComponent<outputInputManager>().moles = 50;
        tank.GetComponent<outputInputManager>().internalTemperature = 298f;
        tank.GetComponent<outputInputManager>().internalPressure = 100f;
    }

    public void OpenValve(GameObject tank)
    {
        if(tank.GetComponent<outputInputManager>().selfRate == 0)
        {
            tank.GetComponent<outputInputManager>().selfRate = 10;
            return;
        }

        if(tank.GetComponent<outputInputManager>().selfRate > 0)
        {
            tank.GetComponent<outputInputManager>().selfRate = 0;
            return;
        }
    }

    public void selectLaunchpad()
    {
        if(op != null)
        {
            op.selectedLaunchPad = savedLaunchpad;
            op.hidePadButtons();
        }
    }

    public void setInput(outputInputManager input)
    {
        outputInputManager[] potentialInputsOutputs = FindObjectsOfType<outputInputManager>();
        List<GameObject> actualOutputInput = new List<GameObject>();
        foreach(outputInputManager outputInput in potentialInputsOutputs)
        {
            if(outputInput.gameObject.GetComponent<buildingType>() != null)
            {
                actualOutputInput.Add(outputInput.gameObject);
            }
        }

        foreach(GameObject building in actualOutputInput)
        {
            if(building.GetComponent<buildingType>().type == "GSEtank")
            {
                building.GetComponent<buildingType>().outputUI.SetActive(false);
            }
        }

        fcm = FindObjectOfType<FuelConnectorManager>();
        if(fcm != null)
        {
            fcm.input = input;
        }
        inputUI.SetActive(false);
    }

    public void setOutput(outputInputManager output)
    {
        outputInputManager[] potentialInputsOutputs = FindObjectsOfType<outputInputManager>();
        List<GameObject> actualOutputInput = new List<GameObject>();
        foreach(outputInputManager outputInput in potentialInputsOutputs)
        {
            if(outputInput.gameObject.GetComponent<buildingType>() != null)
            {
                actualOutputInput.Add(outputInput.gameObject);
            }
        }

        foreach(GameObject building in actualOutputInput)
        {
            if(building.GetComponent<buildingType>().type == "launchPad")
            {
                building.GetComponent<buildingType>().inputUI.SetActive(false);
            }
        }
        
        fcm = FindObjectOfType<FuelConnectorManager>();
        if(fcm != null)
        {
            fcm.output = output;
        }
        outputUI.SetActive(false);
    }

    public void setDestination(GameObject tank)
    {
        FuelOrderManager fom = FindObjectOfType<FuelOrderManager>();
        if(fom != null)
        {
            fom.selectedDestination = tank;
        }

        buildingType[] tanks = FindObjectsOfType<buildingType>();
        foreach(buildingType gse in tanks)
        {
            if(gse.type == "GSEtank")
            {
                gse.selectUI.SetActive(false);
            }
        }
    }
}
