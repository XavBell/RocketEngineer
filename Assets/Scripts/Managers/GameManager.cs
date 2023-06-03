using System.Diagnostics;
using System.Numerics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;
using System.Runtime.Serialization;
using System.Xml.Linq;
using System.Text;
using Newtonsoft.Json;

public class GameManager : MonoBehaviour
{

    private GameObject partToConstruct;
    private GameObject capsule;
    private Part[] parts;
    private GameObject attachPoint;
    private AttachPointScript[] attachPoints;
    public int currentStage;
    public CustomCursor customCursor;
    public bool decouplerPresent = false;
    public bool capsuleBuilt = false;
    public bool tankBuilt = false;
    public bool engineBuilt = false;
    public TMP_InputField saveName;
    public Button CapsuleButton;
    public Button TankButton;
    public Button EngineButton;
    public Button DecouplerButton;
    public GameObject scroll;
    public GameObject scrollEngine;
    public GameObject scrollTank;
    public GameObject scrollBox;
    public string path = "rocket";
    public string savePath = "rocket";
    public string filePath;

    public GameObject enginePrefab;
    public GameObject tankPrefab;

    public GameObject Capsule;
    public GameObject Tank;
    public GameObject Engine;
    public GameObject Decoupler;

    public GameObject PrefabToConstruct;
    public GameObject EngineManager;

    public GameObject buttonPrefab;
    public GameObject engineButtonPrefab;
    public GameObject tankButtonPrefab;

    public UnityEngine.Vector2 engineBox = new UnityEngine.Vector2(0, 0);
    public UnityEngine.Vector2 tankBox = new UnityEngine.Vector2(0, 0);
    public UnityEngine.Vector2 engineOffset = new UnityEngine.Vector2(0, 0);
    public UnityEngine.Vector2 tankOffset = new UnityEngine.Vector2(0, 0);
    public UnityEngine.Vector2 decouplerBox = new UnityEngine.Vector2(0, 0);
    public UnityEngine.Vector2 decouplerOffset = new UnityEngine.Vector2(0, 0);

    public float capsuleInitialSizeX;
    public savePath savePathRef = new savePath();
    public string partPath;

    public GameObject popUp;
    public GameObject popUpPart;

    public GameObject panel;

    public MasterManager MasterManager = new MasterManager();
    
    public bool delayStarted = false;

    public string partType;
    public GameObject CursorGameObject;
    public GameObject Rocket;

    public List<GameObject> DebugList = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        if(SceneManager.GetActiveScene().name.ToString() == "Building")
        {
            GameObject GMM = GameObject.FindGameObjectWithTag("MasterManager");
            MasterManager = GMM.GetComponent<MasterManager>();

            retrieveEngineSaved();
            retrieveTankSaved();
            retrieveRocketSaved();
        }
    }

    // Update is called once per frame
    void Update()
    {
        Rotate();
        updateSaveName();
        if(Input.GetMouseButtonDown(0) && partToConstruct != null)
        {
            ResetCursorGameObject();
            GameObject currentPrefab = null;
            if (partToConstruct.GetComponent<RocketPart>()._partType == "satellite" && capsuleBuilt == false && Cursor.visible == false)
            {
                UnityEngine.Vector2 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                currentPrefab = Instantiate(partToConstruct, position, UnityEngine.Quaternion.Euler(customCursor.transform.eulerAngles));
                //if (partPath != null)
                //{
                //    load(partPath, currentPrefab);
                //}
                UnityEngine.Vector2 prefabPos = new UnityEngine.Vector2(currentPrefab.transform.position.x, currentPrefab.transform.position.y);
                setPosition(prefabPos, currentPrefab);
            }

            if (partToConstruct.GetComponent<RocketPart>()._partType == "tank" && Cursor.visible == false)
            {
                UnityEngine.Vector2 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                currentPrefab = Instantiate(partToConstruct, position, UnityEngine.Quaternion.Euler(customCursor.transform.eulerAngles));
                if (partPath != null)
                {
                    load(partPath, currentPrefab);
                }
                UnityEngine.Vector2 prefabPos = new UnityEngine.Vector2(currentPrefab.transform.position.x, currentPrefab.transform.position.y);
                setPosition(prefabPos, currentPrefab);
            }

            if (partToConstruct.GetComponent<RocketPart>()._partType == "decoupler" && Cursor.visible == false)
            {
                UnityEngine.Vector2 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                currentPrefab = Instantiate(partToConstruct, position, UnityEngine.Quaternion.Euler(customCursor.transform.eulerAngles));
                setPosition(currentPrefab.transform.position, currentPrefab);
            }

            if (partToConstruct.GetComponent<RocketPart>()._partType.ToString() == "engine" && Cursor.visible == false)
            {
                UnityEngine.Vector2 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                currentPrefab = Instantiate(partToConstruct, position, UnityEngine.Quaternion.Euler(customCursor.transform.eulerAngles));
                if (partPath != null)
                {
                    load(partPath, currentPrefab);
                }
                UnityEngine.Vector2 enginePosition = new UnityEngine.Vector2(currentPrefab.transform.position.x, currentPrefab.transform.position.y);
                setPosition(enginePosition, currentPrefab);
            }
            
            if(currentPrefab != null && Rocket.GetComponent<Rocket>().core == null)
            {
                Rocket.GetComponent<Rocket>().core = currentPrefab;
            }

            partToConstruct = null;
            partPath = null;
            customCursor.GetComponent<SpriteRenderer>().sprite = null;
            Cursor.visible = true;
            Rocket.GetComponent<Rocket>().scanRocket();
        }

        if (Input.GetKey(KeyCode.Escape))
        {
            Application.Quit();
 
        }
    }

    public void Launch()
    {
        
        SceneManager.LoadScene("SampleScene");
    }

    public void Clear()
    {
        if(capsule != null)
        {
            Destroy(capsule);
        }
        SceneManager.LoadScene("Building");
    }

    public void ResetCursorGameObject()
    {
        DestroyImmediate(customCursor.transform.GetChild(0).gameObject);
    }

    public void ConstructPart(GameObject part)
    {
        if(Cursor.visible == true)
        {
            customCursor.gameObject.SetActive(true);
            UnityEngine.Vector2 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            CursorGameObject = Instantiate(part, position, UnityEngine.Quaternion.Euler(customCursor.transform.eulerAngles));
            CursorGameObject.transform.SetParent(customCursor.transform);

            if(partPath != null)
            {
                load(partPath, CursorGameObject);
            }
            Cursor.visible = false;
            partToConstruct = part;
        }
    }

    public void Rotate()
    {
        if(Cursor.visible == false && delayStarted == false)
        {
            if(Input.GetKey(KeyCode.R))
            {
                customCursor.transform.Rotate(0, 0, 90);
                StartCoroutine(Delay());
            }
        }
    }

    IEnumerator Delay() 
    {
        delayStarted = true;
        yield return new WaitForSeconds(0.3f);
        delayStarted = false;
    }

    public void updateSaveName()
    {
        savePath = "/" + saveName.text;
    }

    public void load(string fileTypePath, GameObject prefab)
    {
        saveRocket saveObject = new saveRocket();
        saveEngine saveEngine = new saveEngine();
        saveTank saveTank = new saveTank();

        if (fileTypePath == savePathRef.rocketFolder)
        {
            var jsonString = JsonConvert.SerializeObject(saveObject);
            jsonString = File.ReadAllText(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + fileTypePath + path);
            saveRocket loadedRocket = JsonConvert.DeserializeObject<saveRocket>(jsonString);
            int engineCount = 0;
            int tankCount = 0;
            if (capsule != null)
            {
                Destroy(capsule);
                tankBuilt = false;
                engineBuilt = false;
                decouplerPresent = false;
            }

            UnityEngine.Vector2 position = new UnityEngine.Vector2(420, 264);
            capsule = Instantiate(Capsule, position, UnityEngine.Quaternion.identity);

            AttachPointScript attachPoint = capsule.GetComponent<Part>().attachBottom;
            GameObject currentPrefab = null;
            GameObject lastPrefab = capsule;
            capsuleBuilt = true;
            foreach (string type in loadedRocket.attachedBodies)
            {
                if (type == "tank")
                {
                    UnityEngine.Vector2 attachPosition = attachPoint.transform.position;
                    currentPrefab = Instantiate(Tank, position, UnityEngine.Quaternion.identity);

                    tankPrefab = currentPrefab;
                    string TypePath = loadedRocket.tankPaths[tankCount];
                    path = loadedRocket.tankNames[tankCount] + ".json";
                    load(TypePath, tankPrefab);
                    tankCount++;

                    GameObject newPrefabDetach = currentPrefab;
                    if (decouplerPresent == true)
                    {
                        if (currentPrefab.GetComponent<Part>().attachTop.GetComponent<AttachPointScript>().attachedBody.GetComponent<Part>().type.ToString() == "decoupler")
                        {
                            currentPrefab.GetComponent<Part>().referenceDecoupler = currentPrefab.GetComponent<Part>().attachTop.GetComponent<AttachPointScript>().attachedBody;
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

                    tankBuilt = true;

                    lastPrefab = currentPrefab;
                }

                if (type == "engine")
                {
                
                    UnityEngine.Vector2 attachPosition = attachPoint.transform.position;
                    currentPrefab = Instantiate(Engine, position, UnityEngine.Quaternion.identity);

                    enginePrefab = currentPrefab;
                    string TypePath = loadedRocket.enginePaths[engineCount];
                    path = loadedRocket.engineNames[engineCount] + ".json";
                    load(TypePath, enginePrefab);
                    engineCount++;

                    if (attachPoint.GetComponent<AttachPointScript>().referenceBody.GetComponent<Part>().type.ToString() == "tank")
                    {
                        currentPrefab.GetComponent<Part>().StageNumber = attachPoint.GetComponent<AttachPointScript>().referenceBody.GetComponent<Part>().StageNumber;

                    }


                    AttachPointScript currentAttach = currentPrefab.GetComponent<Part>().attachTop;
                    while (currentAttach.attachedBody.GetComponent<Part>().type.ToString() == "tank")
                    {
                        currentPrefab.GetComponent<Part>().maxFuel += currentAttach.attachedBody.GetComponent<Part>().maxFuel;
                        currentAttach = currentAttach.attachedBody.GetComponent<Part>().attachTop;
                    }



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
                        engineBuilt = true;
                        lastPrefab = currentPrefab;
                }

                if (type == "decoupler")
                {
                    UnityEngine.Vector2 attachPosition = attachPoint.transform.position;
                    currentPrefab = Instantiate(Decoupler, position, UnityEngine.Quaternion.identity);
                    decouplerPresent = true;
                    lastPrefab = currentPrefab;
                }

                attachPoint = lastPrefab.GetComponent<Part>().attachBottom;
            }

            if (capsuleBuilt == false)
            {
                DecouplerButton.interactable = false;
            }

            if (capsuleBuilt == true)
            {
                CapsuleButton.interactable = false;
                DecouplerButton.interactable = false;
            }

            if (engineBuilt == true)
            {
                DecouplerButton.interactable = true;
            }
        }

        if(fileTypePath == savePathRef.engineFolder)
        {
            var jsonString = JsonConvert.SerializeObject(saveEngine);
            jsonString = File.ReadAllText(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + fileTypePath + path);
            saveEngine loadedEngine = JsonConvert.DeserializeObject<saveEngine>(jsonString);

            prefab.GetComponent<RocketPart>()._path = loadedEngine.path;
            prefab.GetComponent<RocketPart>()._partName = loadedEngine.engineName;
            prefab.GetComponent<Engine>()._thrust = loadedEngine.thrust_s;
            prefab.GetComponent<Engine>()._rate = loadedEngine.rate_s;
            prefab.GetComponent<RocketPart>()._partMass = loadedEngine.mass_s;

            prefab.GetComponent<Engine>()._nozzleEnd.transform.localScale = new UnityEngine.Vector2(loadedEngine.nozzleExitSize_s, loadedEngine.verticalSize_s);
            prefab.GetComponent<Engine>()._nozzleEnd.transform.localPosition = new UnityEngine.Vector2(prefab.GetComponent<Engine>()._nozzleEnd.transform.localPosition.x, loadedEngine.verticalPos);
            prefab.GetComponent<Engine>()._attachBottom.transform.localPosition = (new UnityEngine.Vector3(0, loadedEngine.attachBottomPos, 0));
            prefab.GetComponent<Engine>()._nozzleStart.transform.localScale = new UnityEngine.Vector2(loadedEngine.nozzleEndSize_s, prefab.GetComponent<Engine>()._nozzleStart.GetComponent<SpriteRenderer>().transform.localScale.y);
            prefab.GetComponent<Engine>()._turbopump.transform.localScale = new UnityEngine.Vector2(loadedEngine.turbopumpSize_s, prefab.GetComponent<Engine>()._turbopump.GetComponent<SpriteRenderer>().transform.localScale.y);
        }

        if (fileTypePath == savePathRef.tankFolder)
        {
            var jsonString = JsonConvert.SerializeObject(saveTank);
            jsonString = File.ReadAllText(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + fileTypePath + path);
            saveTank loadedTank = JsonConvert.DeserializeObject<saveTank>(jsonString);

            prefab.GetComponent<RocketPart>()._path = loadedTank.path;
            prefab.GetComponent<RocketPart>()._partName = loadedTank.tankName;

            prefab.GetComponent<Tank>()._volume = loadedTank.volume;
            prefab.GetComponent<RocketPart>()._partMass = loadedTank.mass;
            prefab.GetComponent<SpriteRenderer>().size = new UnityEngine.Vector2(loadedTank.tankSizeX, loadedTank.tankSizeY);
            prefab.GetComponent<RocketPart>()._attachTop.transform.localPosition = (new UnityEngine.Vector3(0, loadedTank.attachTopPos, 0));
            prefab.GetComponent<RocketPart>()._attachBottom.transform.localPosition = (new UnityEngine.Vector3(0, loadedTank.attachBottomPos, 0));
            prefab.GetComponent<RocketPart>()._attachRight.transform.localPosition = (new UnityEngine.Vector3(loadedTank.attachRightPos, 0, 0));
            prefab.GetComponent<RocketPart>()._attachLeft.transform.localPosition = (new UnityEngine.Vector3(loadedTank.attachLeftPos, 0, 0));
        }
        filePath = null;
    }

    public void save()
    {
        saveRocket saveObject = new saveRocket();
        GameObject attachedBody;
        if(capsule != null)
        {
           if(capsule.GetComponent<Part>().attachBottom.GetComponent<AttachPointScript>().attachedBody != null)
           {
            var jsonString = JsonConvert.SerializeObject(saveObject);
            if (!Directory.Exists(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + savePathRef.rocketFolder))
            {
                Directory.CreateDirectory(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + savePathRef.rocketFolder);
            }

            if(!File.Exists(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + savePathRef.rocketFolder + savePath + ".json"))
            {
                attachedBody = capsule.GetComponent<Part>().attachBottom.GetComponent<AttachPointScript>().attachedBody;

                while (attachedBody.GetComponent<Part>().attachBottom.GetComponent<AttachPointScript>().attachedBody != null)
                {
                    saveObject.attachedBodies.Add(attachedBody.GetComponent<Part>().type.ToString());
                    if(attachedBody.GetComponent<Part>().type == "engine")
                    {
                        saveObject.enginePaths.Add(attachedBody.GetComponent<Part>().path);
                        saveObject.engineNames.Add(attachedBody.GetComponent<Part>().partName);

                        //Add 1 to usedNum
                        saveEngine saveEngine = new saveEngine();
                        var jsonString1 = JsonConvert.SerializeObject(saveEngine);
                        jsonString1 = File.ReadAllText(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + attachedBody.GetComponent<Part>().path + attachedBody.GetComponent<Part>().partName + ".json");
                        saveEngine loadedEngine = JsonConvert.DeserializeObject<saveEngine>(jsonString1);
                        loadedEngine.usedNum += 1;
                        jsonString1 = JsonConvert.SerializeObject(loadedEngine);
                        System.IO.File.WriteAllText(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + attachedBody.GetComponent<Part>().path + attachedBody.GetComponent<Part>().partName + ".json", jsonString1);
                    }

                    if (attachedBody.GetComponent<Part>().type == "tank")
                    {
                        saveObject.tankPaths.Add(attachedBody.GetComponent<Part>().path);
                        saveObject.tankNames.Add(attachedBody.GetComponent<Part>().partName);

                        //Add 1 to usedNum
                        saveTank saveTank = new saveTank();
                        var jsonString1 = JsonConvert.SerializeObject(saveTank);
                        jsonString1 = File.ReadAllText(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + attachedBody.GetComponent<Part>().path + attachedBody.GetComponent<Part>().partName + ".json");
                        saveTank loadedTank = JsonConvert.DeserializeObject<saveTank>(jsonString1);
                        loadedTank.usedNum += 1;
                        jsonString1 = JsonConvert.SerializeObject(loadedTank);
                        System.IO.File.WriteAllText(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + attachedBody.GetComponent<Part>().path + attachedBody.GetComponent<Part>().partName + ".json", jsonString1);
                    }
                    attachedBody = attachedBody.GetComponent<Part>().attachBottom.GetComponent<AttachPointScript>().attachedBody;
                }
            
                if(attachedBody.GetComponent<Part>().attachBottom.GetComponent<AttachPointScript>().attachedBody == null)
                {
                    saveObject.attachedBodies.Add(attachedBody.GetComponent<Part>().type.ToString());
                    if (attachedBody.GetComponent<Part>().type == "engine")
                    {
                        saveObject.enginePaths.Add(attachedBody.GetComponent<Part>().path);
                        saveObject.engineNames.Add(attachedBody.GetComponent<Part>().partName);

                        //Add 1 to usedNum
                        saveEngine saveEngine = new saveEngine();
                        var jsonString1 = JsonConvert.SerializeObject(saveEngine);
                        jsonString1 = File.ReadAllText(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + attachedBody.GetComponent<Part>().path + attachedBody.GetComponent<Part>().partName + ".json");
                        saveEngine loadedEngine = JsonConvert.DeserializeObject<saveEngine>(jsonString1);
                        loadedEngine.usedNum = 1.0f + loadedEngine.usedNum;
                        jsonString1 = JsonConvert.SerializeObject(loadedEngine);
                        System.IO.File.WriteAllText(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + attachedBody.GetComponent<Part>().path + attachedBody.GetComponent<Part>().partName + ".json", jsonString1);
                    }
                    if (attachedBody.GetComponent<Part>().type == "tank")
                    {
                        saveObject.tankPaths.Add(attachedBody.GetComponent<Part>().path);
                        saveObject.tankNames.Add(attachedBody.GetComponent<Part>().partName);
                        
                        //Add 1 to usedNum
                        saveTank saveTank = new saveTank();
                        var jsonString1 = JsonConvert.SerializeObject(saveTank);
                        jsonString1 = File.ReadAllText(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + attachedBody.GetComponent<Part>().path + attachedBody.GetComponent<Part>().partName + ".json");
                        saveTank loadedTank = JsonConvert.DeserializeObject<saveTank>(jsonString1);
                        loadedTank.usedNum += 1;
                        jsonString1 = JsonConvert.SerializeObject(loadedTank);
                        System.IO.File.WriteAllText(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + attachedBody.GetComponent<Part>().path + attachedBody.GetComponent<Part>().partName + ".json", jsonString1);
                    }
                }
                jsonString = JsonConvert.SerializeObject(saveObject);
                System.IO.File.WriteAllText(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + savePathRef.rocketFolder + savePath + ".json", jsonString);
            } else if(File.Exists(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + savePathRef.rocketFolder + savePath + ".json"))
            {
                int x = Screen.width / 2;
                int y = Screen.height / 2;
                UnityEngine.Vector2 position = new UnityEngine.Vector2(x, y);
                Instantiate(popUp, position, UnityEngine.Quaternion.identity);
                panel.SetActive(false);
                //Tell player to either change the save name or delete the rocket of the same name, add delete button
                saveObject = null;
                return;
            }

        } 

        }

        
    }

    public void deleteRocket()
    {
        //Retrieve file and for each tank/engine saved remove 1 to usedNum
        if(File.Exists(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + savePathRef.rocketFolder + path))
        {
            saveRocket saveRocket = new saveRocket();
            var jsonString = JsonConvert.SerializeObject(saveRocket);
            jsonString = File.ReadAllText(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + savePathRef.rocketFolder + path );
            saveRocket loadedRocket = JsonConvert.DeserializeObject<saveRocket>(jsonString);
            int engineCount = 0;
            int tankCount = 0;
            foreach(string attachedBodies in loadedRocket.attachedBodies)
            {
                if(attachedBodies == "engine")
                {
                    saveEngine saveEngine = new saveEngine();
                    var jsonString1 = JsonConvert.SerializeObject(saveEngine);
                    jsonString1 = File.ReadAllText(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + loadedRocket.enginePaths[engineCount] + loadedRocket.engineNames[engineCount] + ".json");
                    saveEngine loadedEngine = JsonConvert.DeserializeObject<saveEngine>(jsonString1);
                    loadedEngine.usedNum--;
                    jsonString1 = JsonConvert.SerializeObject(loadedEngine);
                    System.IO.File.WriteAllText(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + loadedRocket.enginePaths[engineCount] + loadedRocket.engineNames[engineCount] + ".json", jsonString1);
                    engineCount++;
                }

                if(attachedBodies == "tank")
                {
                    saveTank saveTank = new saveTank();
                    var jsonString1 = JsonConvert.SerializeObject(saveTank);
                    jsonString1 = File.ReadAllText(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + loadedRocket.tankPaths[tankCount] + loadedRocket.tankNames[tankCount] + ".json");
                    saveTank loadedTank = JsonConvert.DeserializeObject<saveTank>(jsonString1);
                    loadedTank.usedNum--;
                    jsonString1 = JsonConvert.SerializeObject(loadedTank);
                    System.IO.File.WriteAllText(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + loadedRocket.tankPaths[tankCount] + loadedRocket.tankNames[tankCount] + ".json", jsonString1);
                    tankCount++;
                }
                
            }

            File.Delete(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + savePathRef.rocketFolder + path);
            retrieveRocketSaved();
        }
        
    }

    public void retrieveRocketSaved()
    {
        GameObject[] buttons = GameObject.FindGameObjectsWithTag("spawnedButton");
        foreach(GameObject but in buttons)
        {
            Destroy(but);
        }

        if (!Directory.Exists(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + savePathRef.rocketFolder))
        {
            Directory.CreateDirectory(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + savePathRef.rocketFolder);
            return;
        }

        var info = new DirectoryInfo(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + savePathRef.rocketFolder);
        var fileInfo = info.GetFiles();
        if(fileInfo.Length == 0)
        {
            return;
        }
        foreach (var file in fileInfo)
        {
            GameObject rocket = Instantiate(buttonPrefab) as GameObject;
            GameObject child = rocket.transform.GetChild(0).gameObject;
            child = child.transform.GetChild(0).gameObject;
            child.transform.SetParent(scroll.transform, false);
            TextMeshProUGUI b1text = child.GetComponentInChildren<TextMeshProUGUI>();
            b1text.text = Path.GetFileName(file.ToString());
            child.GetComponentInChildren<OnClick>().filePath = savePathRef.rocketFolder;

        }
        
    }

    public void retrieveEngineSaved()
    {
        GameObject[] buttons = GameObject.FindGameObjectsWithTag("engineButton");
        foreach (GameObject but in buttons)
        {
            Destroy(but);
        }

        if (!Directory.Exists(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + savePathRef.engineFolder))
        {
            Directory.CreateDirectory(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + savePathRef.engineFolder);
        }

        var info = new DirectoryInfo(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName+ savePathRef.engineFolder);
        var fileInfo = info.GetFiles();
        foreach (var file in fileInfo)
        {
            GameObject engine = Instantiate(engineButtonPrefab) as GameObject;
            GameObject child = engine.transform.GetChild(0).gameObject;
            child = child.transform.GetChild(0).gameObject;
            child.transform.SetParent(scrollEngine.transform, false);
            TextMeshProUGUI b1text = child.GetComponentInChildren<TextMeshProUGUI>();
            b1text.text = Path.GetFileName(file.ToString());
            child.GetComponentInChildren<OnClick>().filePath = savePathRef.engineFolder;
        }
    }

    public void retrieveTankSaved()
    {
        GameObject[] buttons = GameObject.FindGameObjectsWithTag("tankButton");
        foreach (GameObject but in buttons)
        {
            Destroy(but);
        }

        if (!Directory.Exists(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + savePathRef.tankFolder))
        {
            Directory.CreateDirectory(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + savePathRef.tankFolder);
        }

        var info = new DirectoryInfo(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + savePathRef.tankFolder);
        var fileInfo = info.GetFiles();
        foreach (var file in fileInfo)
        {
            GameObject tank = Instantiate(tankButtonPrefab) as GameObject;
            GameObject child = tank.transform.GetChild(0).gameObject;
            child = child.transform.GetChild(0).gameObject;
            child.transform.SetParent(scrollTank.transform, false);
            TextMeshProUGUI b1text = child.GetComponentInChildren<TextMeshProUGUI>();
            b1text.text = Path.GetFileName(file.ToString());
            child.GetComponentInChildren<OnClick>().filePath = savePathRef.tankFolder;
        }
    }

    public void CreateNewEngine()
    {
        if(File.Exists(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + partPath + path))
        {
            ConstructPart(PrefabToConstruct);
        }
    }

    public void DeleteEngine()
    {
        if(File.Exists(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + partPath + path) && capsule == null)
        {
            saveEngine saveEngine = new saveEngine();
            saveTank saveTank = new saveTank();
            if(partPath == savePathRef.engineFolder)
            {
                var jsonString2 = JsonConvert.SerializeObject(saveEngine);
                jsonString2 = File.ReadAllText(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + partPath + path);
                saveEngine loadedEngine = JsonConvert.DeserializeObject<saveEngine>(jsonString2);

                if(loadedEngine.usedNum == 0)
                {
                   File.Delete(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + partPath + path);
                   retrieveEngineSaved();
                }else if(loadedEngine.usedNum > 0)
                {
                    int x = Screen.width / 2;
                    int y = Screen.height / 2;
                    UnityEngine.Vector2 position = new UnityEngine.Vector2(x, y);
                    Instantiate(popUpPart, position, UnityEngine.Quaternion.identity);
                    panel.SetActive(false);
                }
            }

            if(partPath == savePathRef.tankFolder)
            {
                var jsonString2 = JsonConvert.SerializeObject(saveTank);
                jsonString2 = File.ReadAllText(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + partPath + path);
                saveTank loadedTank = JsonConvert.DeserializeObject<saveTank>(jsonString2);

                if(loadedTank.usedNum == 0)
                {
                   File.Delete(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + partPath + path);
                   retrieveTankSaved();
                }else if(loadedTank.usedNum > 0)
                {
                    int x = Screen.width / 2;
                    int y = Screen.height / 2;
                    UnityEngine.Vector2 position = new UnityEngine.Vector2(x, y);
                    Instantiate(popUpPart, position, UnityEngine.Quaternion.identity);
                    panel.SetActive(false);
                }
            }
        }
    }


    //Refactorized code
    public List<GameObject> findAvailableAttachPoint(GameObject currentPart)
    {
        AttachPointScript[] attachs = FindObjectsOfType<AttachPointScript>();
        List<GameObject> potentialAttach = new List<GameObject>();
        foreach (AttachPointScript attach in attachs)
        {
            if(attach.attachedBody == null && attach.referenceBody != currentPart)
            {
                potentialAttach.Add(attach.gameObject);
            }
        }
        return potentialAttach;
    }

    public GameObject findClosestAttach(UnityEngine.Vector2 position, GameObject currentPart)
    {
        List<GameObject> availableAttachs = findAvailableAttachPoint(currentPart);
        DebugList = availableAttachs;
        GameObject bestAttach = null;
        float bestDistance = Mathf.Infinity;
        foreach (GameObject attach in availableAttachs)
        {
            UnityEngine.Vector2 attachPos = new UnityEngine.Vector2(attach.transform.position.x, attach.transform.position.y);
            float distance = UnityEngine.Vector2.Distance(attachPos, position);
            if(distance < bestDistance)
            {
                bestAttach = attach;
                bestDistance = distance;
            }
        }
        return bestAttach;
    }

    public void setPosition(UnityEngine.Vector2 partPosition, GameObject part)
    {
        GameObject attachPoint = findClosestAttach(partPosition, part);
        if(attachPoint != null)
        {
            AttachPointScript attachRef = attachPoint.GetComponent<AttachPointScript>();
            List<GameObject> attachsSelf = new List<GameObject>();
        
            if(part.GetComponent<RocketPart>()._attachBottom != null)
            {
                attachsSelf.Add(part.GetComponent<RocketPart>()._attachBottom);
            }

            if(part.GetComponent<RocketPart>()._attachLeft != null)
            {
                attachsSelf.Add(part.GetComponent<RocketPart>()._attachLeft);
            }

            if(part.GetComponent<RocketPart>()._attachRight != null)
            {
                attachsSelf.Add(part.GetComponent<RocketPart>()._attachRight);
            }

            if(part.GetComponent<RocketPart>()._attachTop != null)
            {
                attachsSelf.Add(part.GetComponent<RocketPart>()._attachTop);
            }

            GameObject bestPoint = null;
            float bestDistance = Mathf.Infinity;
            UnityEngine.Vector2 attachPointPos = new UnityEngine.Vector2(attachPoint.transform.position.x, attachPoint.transform.position.y);

            foreach (GameObject attachSelf in attachsSelf)
            {
                UnityEngine.Vector2 attachSelfPos = new UnityEngine.Vector2(attachSelf.transform.position.x, attachSelf.transform.position.y);
                float Distance = UnityEngine.Vector2.Distance(attachPointPos, attachSelfPos);
                if(Distance < bestDistance)
                {
                    bestDistance = Distance;
                    bestPoint = attachSelf;
                }            
            }
            attachRef.attachedBody = part;
            bestPoint.GetComponent<AttachPointScript>().attachedBody = attachRef.referenceBody;
            UnityEngine.Vector2 pointPosition = new UnityEngine.Vector2(bestPoint.transform.position.x, bestPoint.transform.position.y);
            UnityEngine.Vector2 difference = partPosition - pointPosition;
            part.transform.position = attachPointPos + difference;
        }
        
    }

}
