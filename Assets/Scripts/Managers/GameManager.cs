using System.Net.Mime;
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

    public GameObject partToConstruct;
    private GameObject capsule;
    public CustomCursor customCursor;
    public TMP_InputField saveName;
    public GameObject scroll;
    public GameObject scrollEngine;
    public GameObject scrollTank;
    public GameObject scrollBox;
    public string path = "rocket";
    public string savePath = "rocket";
    public string filePath;
    public GameObject Tank;
    public GameObject Engine;

    public GameObject PrefabToConstruct;
    public GameObject buttonPrefab;
    public GameObject engineButtonPrefab;
    public GameObject tankButtonPrefab;

    public savePath savePathRef = new savePath();
    public string partPath;

    public GameObject popUpPart;

    public GameObject panel;

    public MasterManager MasterManager = new MasterManager();

    public bool delayStarted = false;

    public GameObject CursorGameObject;
    public GameObject Rocket;
    public TMP_Dropdown propellantLine;

    public GameObject[] panels;
    public TMP_Dropdown rocketDropdown;

    public GameObject MainPanel;
    public GameObject CreatorPanel;
    public GameObject DataPanel;
    public GameObject propellantPanel;

    public TMP_Text costTxt;

    // Start is called before the first frame update
    void Start()
    {
        if (SceneManager.GetActiveScene().name.ToString() == "Building")
        {
            GameObject GMM = GameObject.FindGameObjectWithTag("MasterManager");
            MasterManager = GMM.GetComponent<MasterManager>();

            retrieveEngineSaved();
            retrieveTankSaved();
        }
    }

    // Update is called once per frame
    void Update()
    {
        Rotate();
        updateSaveName();
        if (Input.GetMouseButtonDown(0) && partToConstruct != null)
        {
            Vector3 savedRot = customCursor.transform.GetChild(1).transform.eulerAngles;
            ResetCursorGameObject();
            GameObject currentPrefab = null;
            if (Cursor.visible == false)
            {
                Vector2 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                currentPrefab = Instantiate(partToConstruct, position, Quaternion.Euler(new Vector3(0, 0, 0)));
                if (partPath != null)
                {
                    load(partPath, currentPrefab);
                }
                if(currentPrefab.GetComponentInChildren<autoSpritePositionner>())
                {
                    currentPrefab.GetComponent<Engine>().InitializeSprite();
                    currentPrefab.GetComponentInChildren<autoSpritePositionner>().UpdatePosition();
                    currentPrefab.GetComponentInChildren<autoSpritePositionner>().bypass = true;
                }
                currentPrefab.transform.localRotation = Quaternion.Euler(savedRot);
                Vector2 prefabPos = new Vector2(currentPrefab.transform.position.x, currentPrefab.transform.position.y);
                setPosition(prefabPos, currentPrefab);
                print(currentPrefab.GetComponent<RocketPart>());
                currentPrefab.GetComponent<RocketPart>().SetGuid();
            }

            if (currentPrefab != null && Rocket.GetComponent<Rocket>().core == null)
            {
                Rocket.GetComponent<Rocket>().core = currentPrefab;
            }

            partToConstruct = null;
            partPath = null;
            customCursor.GetComponent<SpriteRenderer>().sprite = null;
            Cursor.visible = true;
            Rocket.GetComponent<Rocket>().scanRocket();
            updateCost();
        }

        if(Input.GetMouseButton(1) && partToConstruct != null)
        {
            Cursor.visible = true;
            customCursor.GetComponent<SpriteRenderer>().sprite = null;
            partPath = null;
            partToConstruct = null;
            if(customCursor.transform.childCount > 1)
            {
                ResetCursorGameObject();
            }
        }

        if(Input.GetMouseButton(1) && partToConstruct == false && propellantPanel.activeSelf == false)
        {
            DetectClick();
        }
    }

    public void DetectClick()
    {
        RaycastHit2D raycastHit;
        Vector2 cameraPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 ray = cameraPos;
        raycastHit = Physics2D.Raycast(ray, new Vector2(0, 1000));
        print(raycastHit);
        if(raycastHit.transform != null)
        {
            if(raycastHit.transform.gameObject.GetComponent<Tank>())
            {
                propellantPanel.SetActive(true);
                propellantPanel.GetComponent<dropDownManager>().tank = raycastHit.transform.gameObject;
            }
        }
    }

    public void Launch()
    {

        SceneManager.LoadScene("SampleScene");
    }

    void updateCost()
    {
        float cost = 0;
        foreach (Stages stage in Rocket.GetComponent<Rocket>().Stages)
        {
            foreach (RocketPart part in stage.Parts)
            {
                if (cost != float.NaN)
                {
                    cost += part._partCost;
                }
            }
        }

        costTxt.text = cost.ToString();
    }

    public void Clear()
    {
        if (capsule != null)
        {
            Destroy(capsule);
        }
        SceneManager.LoadScene("Building");
    }

    public void ResetCursorGameObject()
    {
        DestroyImmediate(customCursor.transform.GetChild(1).gameObject);
    }

    public void ConstructPart(GameObject part)
    {
        if (Cursor.visible == true)
        {
            customCursor.gameObject.SetActive(true);
            UnityEngine.Vector2 position = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            CursorGameObject = Instantiate(part, position, UnityEngine.Quaternion.Euler(customCursor.transform.eulerAngles));
            CursorGameObject.transform.SetParent(customCursor.transform);

            if (partPath != null)
            {
                load(partPath, CursorGameObject);
            }
            Cursor.visible = false;
            partToConstruct = part;
        }
    }

    public void Rotate()
    {
        if (Cursor.visible == false && delayStarted == false)
        {
            if (Input.GetKey(KeyCode.R))
            {
                customCursor.transform.GetChild(1).transform.Rotate(0, 0, 90);
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

        if (fileTypePath == savePathRef.engineFolder)
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
            engine.reliability = loadedEngine.reliability;
            engine.maxTime = loadedEngine.maxTime;
            engine._partCost = loadedEngine.cost;

        }

        if (fileTypePath == savePathRef.tankFolder)
        {
            var jsonString = JsonConvert.SerializeObject(saveTank);
            jsonString = File.ReadAllText(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + fileTypePath + path);
            saveTank loadedTank = JsonConvert.DeserializeObject<saveTank>(jsonString);

            prefab.GetComponent<SpriteRenderer>().size = new UnityEngine.Vector2(loadedTank.tankSizeX, loadedTank.tankSizeY);

            Tank tank = prefab.GetComponent<Tank>();

            tank._path = loadedTank.path;
            tank._partName = loadedTank.tankName;

            tank._volume = loadedTank.volume;
            tank._partMass = loadedTank.mass;
            tank._attachTop.transform.localPosition = new Vector3(0, loadedTank.attachTopPos, 0);
            tank._attachBottom.transform.localPosition = new UnityEngine.Vector3(0, loadedTank.attachBottomPos, 0);
            tank._attachRight.transform.localPosition = new UnityEngine.Vector3(loadedTank.attachRightPos, 0, 0);
            tank._attachLeft.transform.localPosition = new UnityEngine.Vector3(loadedTank.attachLeftPos, 0, 0);
            tank.tankMaterial = loadedTank.tankMaterial;
            tank.x_scale = loadedTank.tankSizeX;
            tank.y_scale = loadedTank.tankSizeY;
            tank._partCost = loadedTank.cost;
            tank.tested = loadedTank.tested;
            int value = propellantLine.value;
            if (value == 0)
            {
                tank.propellantCategory = "oxidizer";
            }

            if (value == 1)
            {
                tank.propellantCategory = "fuel";
            }
        }
        filePath = null;
    }

    public void deleteRocket()
    {
        //Retrieve file and for each tank/engine saved remove 1 to usedNum
        if (File.Exists(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + savePathRef.rocketFolder + path))
        {
            saveRocket saveRocket = new saveRocket();
            var jsonString = JsonConvert.SerializeObject(saveRocket);
            jsonString = File.ReadAllText(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + savePathRef.rocketFolder + path);
            saveRocket loadedRocket = JsonConvert.DeserializeObject<saveRocket>(jsonString);
            int engineCount = 0;
            int tankCount = 0;
            foreach (string attachedBodies in loadedRocket.attachedBodies)
            {
                if (attachedBodies == "engine")
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

                if (attachedBodies == "tank")
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
        foreach (GameObject but in buttons)
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
        if (fileInfo.Length == 0)
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

        var info = new DirectoryInfo(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + savePathRef.engineFolder);
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
        if (File.Exists(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + partPath + path))
        {
            ConstructPart(PrefabToConstruct);
        }
    }

    public void DeleteEngine()
    {
        if (File.Exists(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + partPath + path) && capsule == null)
        {
            saveEngine saveEngine = new saveEngine();
            saveTank saveTank = new saveTank();
            if (partPath == savePathRef.engineFolder)
            {
                var jsonString2 = JsonConvert.SerializeObject(saveEngine);
                jsonString2 = File.ReadAllText(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + partPath + path);
                saveEngine loadedEngine = JsonConvert.DeserializeObject<saveEngine>(jsonString2);

                if (loadedEngine.usedNum == 0)
                {
                    File.Delete(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + partPath + path);
                    retrieveEngineSaved();
                }
                else if (loadedEngine.usedNum > 0)
                {
                    int x = Screen.width / 2;
                    int y = Screen.height / 2;
                    UnityEngine.Vector2 position = new UnityEngine.Vector2(x, y);
                    Instantiate(popUpPart, position, UnityEngine.Quaternion.identity);
                    panel.SetActive(false);
                }
            }

            if (partPath == savePathRef.tankFolder)
            {
                var jsonString2 = JsonConvert.SerializeObject(saveTank);
                jsonString2 = File.ReadAllText(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + partPath + path);
                saveTank loadedTank = JsonConvert.DeserializeObject<saveTank>(jsonString2);

                if (loadedTank.usedNum == 0)
                {
                    File.Delete(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + partPath + path);
                    retrieveTankSaved();
                }
                else if (loadedTank.usedNum > 0)
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
            if (attach.attachedBody == null && attach.referenceBody != currentPart)
            {
                potentialAttach.Add(attach.gameObject);
            }
        }
        return potentialAttach;
    }

    public GameObject findClosestAttach(UnityEngine.Vector2 position, GameObject currentPart)
    {
        List<GameObject> availableAttachs = findAvailableAttachPoint(currentPart);
        GameObject bestAttach = null;
        float bestDistance = Mathf.Infinity;
        foreach (GameObject attach in availableAttachs)
        {
            UnityEngine.Vector2 attachPos = new UnityEngine.Vector2(attach.transform.position.x, attach.transform.position.y);
            float distance = UnityEngine.Vector2.Distance(attachPos, position);
            if (distance < bestDistance)
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
        if (attachPoint != null)
        {
            AttachPointScript attachRef = attachPoint.GetComponent<AttachPointScript>();
            List<GameObject> attachsSelf = new List<GameObject>();

            if (part.GetComponent<RocketPart>()._attachBottom != null)
            {
                attachsSelf.Add(part.GetComponent<RocketPart>()._attachBottom);
            }

            if (part.GetComponent<RocketPart>()._attachLeft != null)
            {
                attachsSelf.Add(part.GetComponent<RocketPart>()._attachLeft);
            }

            if (part.GetComponent<RocketPart>()._attachRight != null)
            {
                attachsSelf.Add(part.GetComponent<RocketPart>()._attachRight);
            }

            if (part.GetComponent<RocketPart>()._attachTop != null)
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
                if (Distance < bestDistance)
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

    public void save()
    {
        if (!Directory.Exists(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + savePathRef.rocketFolder))
        {
            Directory.CreateDirectory(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + savePathRef.rocketFolder);
        }

        if (!File.Exists(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + savePathRef.rocketFolder + savePath))
        {
            savecraft saveRocket = new savecraft();
            int i = 0;
            saveRocket.coreID = Rocket.GetComponent<Rocket>().core.GetComponent<RocketPart>()._partID;
            foreach (Stages stage in Rocket.GetComponent<Rocket>().Stages)
            {
                foreach (RocketPart part in stage.Parts)
                {
                    if (!saveRocket.PartsID.Contains(part._partID))
                    {
                        saveRocket.StageNumber.Add(i);
                        saveRocket.PartsID.Add(part._partID);
                        saveRocket.partType.Add(part._partType);

                        saveRocket.mass.Add(part._partMass);

                        UnityEngine.Vector3 positionCore = Rocket.GetComponent<Rocket>().core.transform.position;
                        saveRocket.x_pos.Add(part.transform.position.x - positionCore.x);
                        saveRocket.y_pos.Add(part.transform.position.y - positionCore.y);
                        saveRocket.z_pos.Add(part.transform.position.z - positionCore.z);
                        saveRocket.z_rot.Add(part.transform.localEulerAngles.z);

                        //Set attachpoints references
                        if (part._attachTop != null)
                        {
                            if (part._attachTop.GetComponent<AttachPointScript>().attachedBody != null)
                            {
                                saveRocket.attachedTop.Add(part._attachTop.GetComponent<AttachPointScript>().attachedBody.GetComponent<RocketPart>()._partID);
                            }
                            else if (part._attachTop.GetComponent<AttachPointScript>().attachedBody == null)
                            {
                                saveRocket.attachedTop.Add(new Guid());
                            }

                        }
                        else if (part._attachTop == null)
                        {
                            saveRocket.attachedTop.Add(new Guid());
                        }

                        if (part._attachBottom != null)
                        {
                            if (part._attachBottom.GetComponent<AttachPointScript>().attachedBody != null)
                            {
                                saveRocket.attachedBottom.Add(part._attachBottom.GetComponent<AttachPointScript>().attachedBody.GetComponent<RocketPart>()._partID);
                            }
                            else if (part._attachBottom.GetComponent<AttachPointScript>().attachedBody == null)
                            {
                                saveRocket.attachedBottom.Add(new Guid());
                            }

                        }
                        else if (part._attachBottom == null)
                        {
                            saveRocket.attachedBottom.Add(new Guid());
                        }

                        if (part._attachLeft != null)
                        {
                            if (part._attachLeft.GetComponent<AttachPointScript>().attachedBody != null)
                            {
                                saveRocket.attachedLeft.Add(part._attachLeft.GetComponent<AttachPointScript>().attachedBody.GetComponent<RocketPart>()._partID);
                            }
                            else if (part._attachLeft.GetComponent<AttachPointScript>().attachedBody == null)
                            {
                                saveRocket.attachedLeft.Add(new Guid());
                            }

                        }
                        else if (part._attachLeft == null)
                        {
                            saveRocket.attachedLeft.Add(new Guid());
                        }

                        if (part._attachRight != null)
                        {
                            if (part._attachRight.GetComponent<AttachPointScript>().attachedBody != null)
                            {
                                saveRocket.attachedRight.Add(part._attachRight.GetComponent<AttachPointScript>().attachedBody.GetComponent<RocketPart>()._partID);
                            }
                            else if (part._attachRight.GetComponent<AttachPointScript>().attachedBody == null)
                            {
                                saveRocket.attachedRight.Add(new Guid());
                            }

                        }
                        else if (part._attachRight == null)
                        {
                            saveRocket.attachedRight.Add(new Guid());
                        }


                        //Set tank
                        if (part._partType == "tank")
                        {
                            saveRocket.tankName.Add(part.GetComponent<Tank>()._partName);
                            saveRocket.tankCost.Add(part.GetComponent<Tank>()._partCost);
                            saveRocket.x_scale.Add(part.GetComponent<BoxCollider2D>().size.x);
                            saveRocket.y_scale.Add(part.GetComponent<BoxCollider2D>().size.y);
                            saveRocket.volume.Add(part.GetComponent<Tank>()._volume);
                            saveRocket.propellantType.Add(part.GetComponent<Tank>().propellantCategory);
                            saveRocket.tankMaterial.Add(part.GetComponent<Tank>().tankMaterial);
                            saveRocket.tested.Add(part.GetComponent<Tank>().tested);
                        }

                        //Set Engine
                        if (part._partType == "engine")
                        {
                            saveRocket.engineName.Add(part.GetComponent<Engine>()._partName);
                            saveRocket.nozzleName.Add(part.GetComponent<Engine>()._nozzleName);
                            saveRocket.turbineName.Add(part.GetComponent<Engine>()._turbineName);
                            saveRocket.pumpName.Add(part.GetComponent<Engine>()._pumpName);
                            saveRocket.engineCost.Add(part.GetComponent<Engine>()._partCost);
                            saveRocket.thrust.Add(part.GetComponent<Engine>()._thrust);
                            saveRocket.flowRate.Add(part.GetComponent<Engine>()._rate);
                            saveRocket.maxTime.Add(part.GetComponent<Engine>().maxTime);
                            saveRocket.reliability.Add(part.GetComponent<Engine>().reliability);
                        }
                    }


                }
                i++;
            }

            var jsonString = JsonConvert.SerializeObject(saveRocket);
            System.IO.File.WriteAllText(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + savePathRef.rocketFolder + savePath + ".json", jsonString);

        }
        else
        {
            //cry
        }
    }

    public void activateDeactivate(GameObject button)
    {
        hidePanels(button);
        if (button.activeSelf == true)
        {
            button.SetActive(false);
            return;
        }

        if (button.activeSelf == false)
        {
            button.SetActive(true);
            return;
        }
    }

    public void hidePanels(GameObject excludedPanel)
    {
        foreach (GameObject panel in panels)
        {
            if (panel != excludedPanel)
            {
                panel.SetActive(false);
            }
        }
    }

    public void initializeRocketInFolder()
    {
        List<string> options = new List<string>();
        if (!Directory.Exists(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + savePathRef.rocketFolder))
        {
            Directory.CreateDirectory(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + savePathRef.rocketFolder);
            return;
        }

        var info = new DirectoryInfo(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + savePathRef.rocketFolder);
        var fileInfo = info.GetFiles();
        if (fileInfo.Length == 0)
        {
            return;
        }
        foreach (var file in fileInfo)
        {
            options.Add(Path.GetFileName(file.ToString()));
        }
        rocketDropdown.AddOptions(options);
    }

    public void BackToMain()
    {
        MainPanel.SetActive(true);
        CreatorPanel.SetActive(false);
        DataPanel.SetActive(false);
    }

    public void EnterCreator()
    {
        MainPanel.SetActive(false);
        CreatorPanel.SetActive(true);
        DataPanel.SetActive(false);
    }

    public void EnterData()
    {
        MainPanel.SetActive(false);
        CreatorPanel.SetActive(false);
        DataPanel.SetActive(true);
    }

}
