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
        Debug.Log("Clicked");
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
            b1.interactable = false;

        }   
        
    }

    public void load(string fileTypePath)
    {
        bool decouplerPresent = false;
        GameObject MasterManagerGO = GameObject.FindGameObjectWithTag("MasterManager");
        MasterManager MasterManager = MasterManagerGO.GetComponent<MasterManager>();
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

            Vector2 position = new Vector2(launchPad.transform.position.x, launchPad.transform.position.y+launchPad.GetComponent<BoxCollider2D>().bounds.max.y/2+10);
            GameObject capsule = Instantiate(capsulePrefab, position, Quaternion.identity);
            capsule.GetComponent<PlanetGravity>().posUpdated = true;
            capsule.transform.position = position;

            AttachPointScript attachPoint = capsule.GetComponent<Part>().attachBottom;
            GameObject currentPrefab = null;
            GameObject lastPrefab = capsule;
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

                    setRocketValues(attachPoint, currentPrefab, capsule);

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

                    setRocketValues(attachPoint, currentPrefab, capsule);

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
                    capsule.GetComponent<outputInputManager>().engines.Add(currentPrefab);
                    lastPrefab = currentPrefab;
                }

                if (type == "decoupler")
                {
                    Vector2 attachPosition = attachPoint.transform.position;
                    currentPrefab = Instantiate(Decoupler, position, Quaternion.identity);
                    setRocketValues(attachPoint, currentPrefab, capsule);
                    decouplerPresent = true;
                    lastPrefab = currentPrefab;
                }

                attachPoint = lastPrefab.GetComponent<Part>().attachBottom;
            }


            capsule.transform.localScale = new Vector2(0.5f, 0.5f);
            if(launchPad != null && capsule != null)
            {
                //Reference capsule to the launchPad
                if(launchPad.GetComponent<launchPadManager>().ConnectedRocket != null)
                {
                    Destroy(launchPad.GetComponent<launchPadManager>().ConnectedRocket);
                }

                //Set Output/Input fuel values
                launchPad.GetComponent<launchPadManager>().ConnectedRocket = capsule;
                Debug.Log("Still Alive!");

                capsule.GetComponent<outputInputManager>().inputParent = launchPad;
                capsule.GetComponent<outputInputManager>().connectedAsRocket = true;
                capsule.GetComponent<outputInputManager>().inputParentID = launchPad.GetComponent<outputInputManager>().selfID;
                
                launchPad.GetComponent<outputInputManager>().outputParent = capsule;

                foreach(GameObject en in capsule.GetComponent<outputInputManager>().engines)
                {
                    capsule.GetComponent<outputInputManager>().volume += en.GetComponent<Part>().maxFuel;
                }

            }   

        }

        if(fileTypePath == savePathRef.engineFolder)
            {
                var jsonString1 = JsonConvert.SerializeObject(saveEngine);
                jsonString1 = File.ReadAllText(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + fileTypePath + path);
                saveEngine loadedEngine = JsonConvert.DeserializeObject<saveEngine>(jsonString1);

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
                var jsonString1 = JsonConvert.SerializeObject(saveTank);
                jsonString1 = File.ReadAllText(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + fileTypePath + path);
                saveTank loadedTank = JsonConvert.DeserializeObject<saveTank>(jsonString1);

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
        tank.GetComponent<outputInputManager>().moles = tank.GetComponent<outputInputManager>().volume;
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
}
