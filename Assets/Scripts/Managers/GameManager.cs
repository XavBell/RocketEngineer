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
    private GameObject attachPoint;
    private AttachPointScript[] attachPoints;
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
                currentPrefab.GetComponent<RocketPart>().SetGuid();
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
                currentPrefab.GetComponent<RocketPart>().SetGuid();
            }

            if (partToConstruct.GetComponent<RocketPart>()._partType == "decoupler" && Cursor.visible == false)
            {
                UnityEngine.Vector2 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                currentPrefab = Instantiate(partToConstruct, position, UnityEngine.Quaternion.Euler(customCursor.transform.eulerAngles));
                setPosition(currentPrefab.transform.position, currentPrefab);
                currentPrefab.GetComponent<RocketPart>().SetGuid();
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
                currentPrefab.GetComponent<RocketPart>().SetGuid();
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
        saveEngine saveEngine = new saveEngine();
        saveTank saveTank = new saveTank();

        if(fileTypePath == savePathRef.engineFolder)
        {
            var jsonString = JsonConvert.SerializeObject(saveEngine);
            jsonString = File.ReadAllText(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + fileTypePath + path);
            saveEngine loadedEngine = JsonConvert.DeserializeObject<saveEngine>(jsonString);

            Engine engine = prefab.GetComponent<Engine>();

            engine._path = loadedEngine.path;
            engine._partName = loadedEngine.engineName;
            engine._thrust = loadedEngine.thrust_s;
            engine._rate = loadedEngine.rate_s;
            engine._partMass = loadedEngine.mass_s;
            engine._tvcSpeed = loadedEngine.tvcSpeed_s;
            engine._maxAngle = loadedEngine.tvcMaxAngle_s;
            engine._tvcName = loadedEngine.tvcName_s;
            engine._nozzleName = loadedEngine.nozzleName_s;
            engine._pumpName = loadedEngine.pumpName_s;
            engine._turbineName = loadedEngine.turbineName_s;
            
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
