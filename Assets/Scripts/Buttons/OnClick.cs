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
using DG.Tweening;
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

    public container oxidizerInput;
    public container fuelInput;
    public container output;
    public FuelConnectorManager fcm;
    public GameObject inputUI;
    public GameObject outputUI;
    public GameObject selectUI;

    private Substance kerosene;
    private Substance LOX;

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
            core.GetComponent<Rigidbody2D>().collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            core.AddComponent<PlanetGravity>();
            core.GetComponent<PlanetGravity>().setCore(core);
            core.GetComponent<Rocket>().core = core;
            //core.AddComponent<DoubleTransform>();
            core.AddComponent<RocketStateManager>();
            core.AddComponent<RocketPath>();
            core.AddComponent<BodySwitcher>();
            
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
                        part.GetComponent<Tank>()._partName = loadedRocket.tankName[tankID];
                        part.transform.localScale = new UnityEngine.Vector2(loadedRocket.x_scale[tankID], loadedRocket.y_scale[tankID]);
                        part.GetComponent<Tank>()._volume = loadedRocket.volume[tankID];
                        part.GetComponent<Tank>().tankMaterial = loadedRocket.tankMaterial[tankID];
                        part.GetComponent<Tank>().propellantCategory = loadedRocket.propellantType[tankID];
                        part.GetComponent<Tank>().x_scale = loadedRocket.x_scale[tankID];
                        part.GetComponent<Tank>().y_scale = loadedRocket.y_scale[tankID];
                        part.GetComponent<Tank>()._partCost = loadedRocket.tankCost[tankID];
                        tankID++;
                    }

                    if(part.GetComponent<RocketPart>()._partType == "engine")
                    {
                        part.GetComponent<Engine>()._partName = loadedRocket.engineName[engineID];
                        part.GetComponent<Engine>()._thrust = loadedRocket.thrust[engineID];
                        part.GetComponent<Engine>()._rate = loadedRocket.flowRate[engineID];
                        part.GetComponent<Engine>().reliability = loadedRocket.reliability[engineID];
                        part.GetComponent<Engine>().maxTime = loadedRocket.maxTime[engineID];
                        part.GetComponent<Engine>()._partCost = loadedRocket.engineCost[engineID];
                        //part.GetComponent<Engine>().InitializeFail();
                        engineID++;
                    }
                }
                j++;
            }
            spawnedRocket = core;
            filePath = null;
            DestroyImmediate(temp);
            
        }

        if (fileTypePath == savePathRef.engineFolder)
        {
            saveEngine saveEngine = new saveEngine();
            var jsonString = JsonConvert.SerializeObject(saveEngine);
            jsonString = File.ReadAllText(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + fileTypePath + path);
            saveEngine loadedEngine = JsonConvert.DeserializeObject<saveEngine>(jsonString);

            spawnedRocket = Instantiate(Engine, launchPad.transform);

            //Engine engine = spawnedRocket.GetComponent<Engine>();
            spawnedRocket.GetComponent<Engine>()._path = loadedEngine.path;
            spawnedRocket.GetComponent<Engine>()._partName = loadedEngine.engineName;
            spawnedRocket.GetComponent<Engine>()._thrust = loadedEngine.thrust_s;
            spawnedRocket.GetComponent<Engine>()._partMass = loadedEngine.mass_s;
            spawnedRocket.GetComponent<Engine>()._rate = loadedEngine.rate_s;
            spawnedRocket.GetComponent<Engine>()._tvcSpeed = loadedEngine.tvcSpeed_s;
            spawnedRocket.GetComponent<Engine>()._maxAngle = loadedEngine.tvcMaxAngle_s;
            spawnedRocket.GetComponent<Engine>()._tvcName = loadedEngine.tvcName_s;
            spawnedRocket.GetComponent<Engine>()._nozzleName = loadedEngine.nozzleName_s;
            spawnedRocket.GetComponent<Engine>()._turbineName = loadedEngine.turbineName_s;
            spawnedRocket.GetComponent<Engine>()._pumpName = loadedEngine.pumpName_s;
            spawnedRocket.GetComponent<Engine>().reliability = loadedEngine.reliability;
            spawnedRocket.GetComponent<Engine>().maxTime = loadedEngine.maxTime;
            spawnedRocket.GetComponent<Engine>()._partCost = loadedEngine.cost;
            if(launchPad.GetComponent<buildingType>().anchor != null)
            {
                spawnedRocket.transform.position = launchPad.GetComponent<buildingType>().anchor.transform.position;
            }

        }

        if (fileTypePath == savePathRef.tankFolder)
        {
            saveTank saveTank = new saveTank();
            var jsonString = JsonConvert.SerializeObject(saveTank);
            jsonString = File.ReadAllText(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + fileTypePath + path);
            saveTank loadedTank = JsonConvert.DeserializeObject<saveTank>(jsonString);

            spawnedRocket = Instantiate(Tank, launchPad.transform);
            spawnedRocket.GetComponent<DoubleTransform>().x_pos = launchPad.transform.position.x;
            spawnedRocket.GetComponent<DoubleTransform>().y_pos = launchPad.transform.position.y;

            //Engine engine = spawnedRocket.GetComponent<Engine>();
            spawnedRocket.GetComponent<Tank>()._path = loadedTank.path;
            spawnedRocket.GetComponent<Tank>()._partName = loadedTank.tankName;
            spawnedRocket.GetComponent<Tank>()._partMass = loadedTank.mass;
            spawnedRocket.GetComponent<Tank>().x_scale = loadedTank.tankSizeX;
            spawnedRocket.GetComponent<Tank>().y_scale = loadedTank.tankSizeY;
            spawnedRocket.GetComponent<Tank>()._volume = loadedTank.volume;
            spawnedRocket.GetComponent<Tank>().tankMaterial = loadedTank.tankMaterial;
            spawnedRocket.GetComponent<Tank>()._partCost = loadedTank.cost;
            if(launchPad.GetComponent<buildingType>().anchor != null)
            {
                spawnedRocket.transform.position = launchPad.GetComponent<buildingType>().anchor.transform.position;
            }
        }
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
        capsule.GetComponent<PlanetGravity>().rocketMass += currentPrefab.GetComponent<Part>().mass;
    }

    public void AddFuel(GameObject tank)
    {
        tank.GetComponent<outputInputManager>().substance = kerosene;
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
            op.run();
        }
    }

    public void setInput(container input)
    {
        container[] potentialContainer = FindObjectsOfType<container>();
        List<GameObject> actualContainer = new List<GameObject>();
        foreach(container container in potentialContainer)
        {
            if(container.gameObject.GetComponent<buildingType>() != null)
            {
                actualContainer.Add(container.gameObject);
            }
        }

        foreach(GameObject building in actualContainer)
        {
            if(building.GetComponent<buildingType>().type == "GSEtank")
            {
                PanelFadeOut(building.GetComponent<buildingType>().outputUI);
                StartCoroutine(ActiveDeactive(1, building.GetComponent<buildingType>().outputUI, false));
            }

            if(building.GetComponent<buildingType>().type == "launchPad")
            {
                PanelFadeOut(building.GetComponent<buildingType>().inputUI);
                StartCoroutine(ActiveDeactive(1, building.GetComponent<buildingType>().inputUI, false));
            }

            if(building.GetComponent<buildingType>().type == "standTank")
            {
                PanelFadeOut(building.GetComponent<buildingType>().inputUI);
                StartCoroutine(ActiveDeactive(1, building.GetComponent<buildingType>().inputUI, false));
            }

            if(building.GetComponent<buildingType>().type == "staticFireStand")
            {
                PanelFadeOut(building.GetComponent<buildingType>().inputUI);
                StartCoroutine(ActiveDeactive(1, building.GetComponent<buildingType>().inputUI, false));
            }
        }

        fcm = FindObjectOfType<FuelConnectorManager>();
        if(fcm != null)
        {
            fcm.input = input;
        }
        PanelFadeOut(inputUI);
        StartCoroutine(ActiveDeactive(1, inputUI, false));
    }

    public void setOutput(container output)
    {
        container[] potentialContainer = FindObjectsOfType<container>();
        List<GameObject> actualContainer = new List<GameObject>();
        foreach(container container in potentialContainer)
        {
            if(container.gameObject.GetComponent<buildingType>() != null)
            {
                actualContainer.Add(container.gameObject);
            }
        }

        foreach(GameObject building in actualContainer)
        {
            if(building.GetComponent<buildingType>().type == "launchPad")
            {
                building.GetComponent<buildingType>().inputUI.SetActive(true);
                PanelFadeIn(building.GetComponent<buildingType>().inputUI);
            }

            if(building.GetComponent<buildingType>().type == "staticFireStand")
            {
                building.GetComponent<buildingType>().inputUI.SetActive(true);
                PanelFadeIn(building.GetComponent<buildingType>().inputUI);
            }

            if(building.GetComponent<buildingType>().type == "standTank")
            {
                building.GetComponent<buildingType>().inputUI.SetActive(true);
                PanelFadeIn(building.GetComponent<buildingType>().inputUI);
            }

            if(building.GetComponent<buildingType>().type == "GSEtank")
            {
                PanelFadeOut(building.GetComponent<buildingType>().outputUI);
                StartCoroutine(ActiveDeactive(1, building.GetComponent<buildingType>().outputUI, false));
            }
        }
        
        fcm = FindObjectOfType<FuelConnectorManager>();
        FuelConnectorManager[] fcms  = FindObjectsOfType<FuelConnectorManager>();
        if(fcm != null)
        {
            fcm.output = output;
            PanelFadeOut(outputUI);
            StartCoroutine(ActiveDeactive(1, outputUI, false));
        }
    }

    private IEnumerator ActiveDeactive(float waitTime, GameObject panel, bool activated)
    {
        yield return new WaitForSeconds(waitTime);
        panel.SetActive(activated);
    }

    public void PanelFadeIn(GameObject panel)
    {
        panel.transform.localScale = new Vector3(0, 0, 0);
        panel.transform.DOScale(1, 0.1f);
    }

    public void PanelFadeOut(GameObject panel)
    {
        panel.transform.DOScale(0, 0.1f);
        panel.transform.localScale = new Vector3(1, 1, 1);
    }

    public void setDestination(GameObject tank)
    {
        FuelOrderManager fom = FindObjectOfType<FuelOrderManager>();
        if(fom != null)
        {
            fom.selectedDestination = tank;
            fom.addFuel();
            fom.selectedDestination = null;
        }

        buildingType[] tanks = FindObjectsOfType<buildingType>();
        foreach(buildingType gse in tanks)
        {
            if(gse.type == "GSEtank")
            {
                PanelFadeOut(gse.selectUI);
                StartCoroutine(ActiveDeactive(1, gse.selectUI, false));
            }
        }
    }
}
