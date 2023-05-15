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


    public Vector2 engineBox = new Vector2(0, 0);
    public Vector2 tankBox = new Vector2(0, 0);
    public Vector2 engineOffset = new Vector2(0, 0);
    public Vector2 tankOffset = new Vector2(0, 0);
    public Vector2 decouplerBox = new Vector2(0, 0);
    public Vector2 decouplerOffset = new Vector2(0, 0);

    public float capsuleInitialSizeX;
    public savePath savePathRef = new savePath();
    public string partPath;

    public GameObject popUp;
    public GameObject popUpPart;

    public GameObject panel;

    public MasterManager MasterManager = new MasterManager();

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

        updateSaveName();
        if(Input.GetMouseButtonDown(0) && partToConstruct != null)
        {

            if (partToConstruct.GetComponent<Part>().type.ToString() == "capsule" && capsuleBuilt == false && Cursor.visible == false)
            {
                Vector2 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                capsule = Instantiate(partToConstruct, position, Quaternion.identity);
                capsule.GetComponent<PlanetGravity>().capsule = capsule;
                capsuleBuilt = true;
                Debug.Log(capsuleBuilt);
            }


            if (partToConstruct.GetComponent<Part>().type.ToString() == "tank" && capsuleBuilt == true && Cursor.visible == false)
            {
                parts = GameObject.FindObjectsOfType<Part>();
                attachPoints = GameObject.FindObjectsOfType<AttachPointScript>();
                Vector2 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                GameObject currentPrefab = Instantiate(partToConstruct, position, Quaternion.identity);
                tankPrefab = currentPrefab;
                if (partPath != null)
                {
                    load(partPath);
                }
                float bestDist = Mathf.Infinity;
                AttachPointScript bestAttachPoint = null;
                
                    foreach (AttachPointScript go1 in attachPoints)
                    {
                        float currentDistance = (go1.transform.position - currentPrefab.transform.position).magnitude;
                        if (currentDistance < bestDist && go1.GetComponent<AttachPointScript>().attachedBody == null)
                        {
                            bestDist = currentDistance;
                            bestAttachPoint = go1;
                        }
                    }

                if (bestAttachPoint != null)
                {
                    tankBuilt = true;

                    setRocketValues(bestAttachPoint, currentPrefab, tankBox, tankOffset);

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
                                Debug.Log(newPrefabDetach);
                                if (newPrefabDetach.GetComponent<Part>().type.ToString() == "decoupler")
                                {
                                    currentPrefab.GetComponent<Part>().referenceDecoupler = newPrefabDetach;
                                }
                            }
                        }
                    }

                    
                }

                

            }


            if (partToConstruct.GetComponent<Part>().type.ToString() == "decoupler" && Cursor.visible == false)
            {
                parts = GameObject.FindObjectsOfType<Part>();
                attachPoints = GameObject.FindObjectsOfType<AttachPointScript>();
                Vector2 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                GameObject currentPrefab = Instantiate(partToConstruct, position, Quaternion.identity);
                float bestDist = Mathf.Infinity;
                AttachPointScript bestAttachPoint = null;

                foreach (AttachPointScript go1 in attachPoints)
                {
                    float currentDistance = (go1.transform.position - currentPrefab.transform.position).magnitude;
                    if (currentDistance < bestDist && go1.GetComponent<AttachPointScript>().attachedBody == null)
                    {
                        bestDist = currentDistance;
                        bestAttachPoint = go1;
                    }
                }

                if (bestAttachPoint != null)
                {
                    setRocketValues(bestAttachPoint, currentPrefab, decouplerBox, decouplerOffset);
                    decouplerPresent = true;
                }

            }



            if (partToConstruct.GetComponent<Part>().type.ToString() == "engine" && capsuleBuilt == true && Cursor.visible == false)
            {
                parts = GameObject.FindObjectsOfType<Part>();
                attachPoints = GameObject.FindObjectsOfType<AttachPointScript>();
                Vector2 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                GameObject currentPrefab = Instantiate(partToConstruct, position, Quaternion.identity);
                enginePrefab = currentPrefab;
                if (partPath != null)
                {
                    load(partPath);
                }
                float bestDist = Mathf.Infinity;
                AttachPointScript bestAttachPoint = null;

                foreach (AttachPointScript go1 in attachPoints)
                {
                    float currentDistance = (go1.transform.position - currentPrefab.transform.position).magnitude;
                    if (currentDistance < bestDist && go1.GetComponent<AttachPointScript>().attachedBody == null)
                    {
                        bestDist = currentDistance;
                        bestAttachPoint = go1;
                    }
                }

                if (bestAttachPoint != null)
                {
                    engineBuilt = true;
                    setRocketValues(bestAttachPoint, currentPrefab, engineBox, engineOffset);

                    if (bestAttachPoint.GetComponent<AttachPointScript>().referenceBody.GetComponent<Part>().type.ToString() == "tank")
                    {
                        currentPrefab.GetComponent<Part>().StageNumber = bestAttachPoint.GetComponent<AttachPointScript>().referenceBody.GetComponent<Part>().StageNumber;
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
                                Debug.Log(newPrefabDetach);
                                if (newPrefabDetach.GetComponent<Part>().type.ToString() == "decoupler")
                                {
                                    currentPrefab.GetComponent<Part>().referenceDecoupler = newPrefabDetach;
                                }
                            }

                        }

                    }


                }

                
            }


            partToConstruct = null;
            customCursor.GetComponent<SpriteRenderer>().sprite = null;
            Cursor.visible = true;

            if (capsuleBuilt == false)
            {
                //DecouplerButton.interactable = false;
            }

            if (capsuleBuilt == true)
            {
                CapsuleButton.interactable = false;
                //DecouplerButton.interactable = false;
            }

            if (tankBuilt == true)
            {
                //DecouplerButton.interactable = true;
            }

            if (engineBuilt == true)
            {
                //DecouplerButton.interactable = true;
            }
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

    public void ConstructPart(GameObject part)
    {
        if(Cursor.visible == true)
        {
            customCursor.gameObject.SetActive(true);
            customCursor.GetComponent<SpriteRenderer>().sprite = part.GetComponent<SpriteRenderer>().sprite;
            customCursor.GetComponent<SpriteRenderer>().color = part.GetComponent<SpriteRenderer>().color;
            Cursor.visible = false;
            partToConstruct = part;
        }
    }

    public void updateSaveName()
    {
        savePath = "/" + saveName.text;
    }

    public void load(string fileTypePath)
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

            Vector2 position = new Vector2(420, 264);
            capsule = Instantiate(Capsule, position, Quaternion.identity);

            AttachPointScript attachPoint = capsule.GetComponent<Part>().attachBottom;
            GameObject currentPrefab = null;
            GameObject lastPrefab = capsule;
            capsuleBuilt = true;
            foreach (string type in loadedRocket.attachedBodies)
            {
                if (type == "tank")
                {
                    Vector2 attachPosition = attachPoint.transform.position;
                    currentPrefab = Instantiate(Tank, position, Quaternion.identity);

                    tankPrefab = currentPrefab;
                    string TypePath = loadedRocket.tankPaths[tankCount];
                    Debug.Log(path);
                    path = loadedRocket.tankNames[tankCount] + ".json";
                    load(TypePath);
                    tankCount++;

                    setRocketValues(attachPoint, currentPrefab, tankBox, tankOffset);

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
                                Debug.Log(newPrefabDetach);
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
                
                    Vector2 attachPosition = attachPoint.transform.position;
                    currentPrefab = Instantiate(Engine, position, Quaternion.identity);

                    enginePrefab = currentPrefab;
                    string TypePath = loadedRocket.enginePaths[engineCount];
                    Debug.Log(path);
                    path = loadedRocket.engineNames[engineCount] + ".json";
                    load(TypePath);
                    engineCount++;

                    setRocketValues(attachPoint, currentPrefab, engineBox, engineOffset);

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
                                Debug.Log(newPrefabDetach);
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
                    Vector2 attachPosition = attachPoint.transform.position;
                    currentPrefab = Instantiate(Decoupler, position, Quaternion.identity);
                    setRocketValues(attachPoint, currentPrefab, decouplerBox, decouplerOffset);
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

            enginePrefab.GetComponent<Part>().path = loadedEngine.path;
            enginePrefab.GetComponent<Part>().partName = loadedEngine.engineName;
            enginePrefab.GetComponent<Part>().maxThrust = loadedEngine.thrust_s;
            enginePrefab.GetComponent<Part>().rate = loadedEngine.rate_s;
            enginePrefab.GetComponent<Part>().mass = loadedEngine.mass_s;

            enginePrefab.GetComponent<Part>().nozzleExit.transform.localScale = new Vector2(loadedEngine.nozzleExitSize_s, loadedEngine.verticalSize_s);
            enginePrefab.GetComponent<Part>().nozzleExit.transform.localPosition = new Vector2(enginePrefab.GetComponent<Part>().nozzleExit.transform.localPosition.x, loadedEngine.verticalPos);
            enginePrefab.GetComponent<Part>().attachBottom.transform.localPosition = (new Vector3(0, loadedEngine.attachBottomPos, 0));
            enginePrefab.GetComponent<Part>().nozzleEnd.transform.localScale = new Vector2(loadedEngine.nozzleEndSize_s, enginePrefab.GetComponent<Part>().nozzleEnd.GetComponent<SpriteRenderer>().transform.localScale.y);
            enginePrefab.GetComponent<Part>().turbopump.transform.localScale = new Vector2(loadedEngine.turbopumpSize_s, enginePrefab.GetComponent<Part>().turbopump.GetComponent<SpriteRenderer>().transform.localScale.y);
        }

        if (fileTypePath == savePathRef.tankFolder)
        {
            var jsonString = JsonConvert.SerializeObject(saveTank);
            jsonString = File.ReadAllText(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + fileTypePath + path);
            saveTank loadedTank = JsonConvert.DeserializeObject<saveTank>(jsonString);

            tankPrefab.GetComponent<Part>().path = loadedTank.path;
            tankPrefab.GetComponent<Part>().partName = loadedTank.tankName;

            tankPrefab.GetComponent<Part>().maxFuel = loadedTank.maxFuel;
            tankPrefab.GetComponent<Part>().mass = loadedTank.mass;
            tankPrefab.GetComponent<Part>().tank.GetComponent<SpriteRenderer>().size = new Vector2(loadedTank.tankSizeX, loadedTank.tankSizeY);
            tankPrefab.GetComponent<Part>().attachTop.transform.localPosition = (new Vector3(0, loadedTank.attachTopPos, 0));
            tankPrefab.GetComponent<Part>().attachBottom.transform.localPosition = (new Vector3(0, loadedTank.attachBottomPos, 0));
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
                Vector2 position = new Vector2(x, y);
                Instantiate(popUp, position, Quaternion.identity);
                panel.SetActive(false);
                //Tell player to either change the save name or delete the rocket of the same name, add delete button
                Debug.Log("AlreadyExists");
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
            Debug.Log("hello");
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
                    Vector2 position = new Vector2(x, y);
                    Instantiate(popUpPart, position, Quaternion.identity);
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
                    Vector2 position = new Vector2(x, y);
                    Instantiate(popUpPart, position, Quaternion.identity);
                    panel.SetActive(false);
                }
            }
        }else if(capsule != null)
        {
            Debug.Log("Clear the rocket before removing engine designs");
            //TODO 
        }
    }

    public void setRocketValues(AttachPointScript attachPoint, GameObject currentPrefab, Vector2 boxScale, Vector2 offsets)
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


}
