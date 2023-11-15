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
                UnityEngine.Debug.Log(GameManager.GetComponent<GameManager>().PrefabToConstruct.GetComponent<Tank>()._partType);
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
        savecraft saveObject = new savecraft();

        GameObject core = null;

        if (fileTypePath == savePathRef.rocketFolder)
        {
            var jsonString = JsonConvert.SerializeObject(saveObject);
            jsonString = File.ReadAllText(Application.persistentDataPath + savePathRef.worldsFolder + '/' + MasterManager.FolderName + fileTypePath + path);
            savecraft loadedRocket = JsonConvert.DeserializeObject<savecraft>(jsonString);

            Rocket rocket = new Rocket();
            int maxSteps = loadedRocket.PartsID.Count;
            int maxStage = Mathf.Max(loadedRocket.StageNumber.ToArray());
            List<GameObject> newParts = new List<GameObject>();

            //Create stages
            for(int i = 0; i<=maxStage; i++)
            {
                Stages stage = new Stages();
                rocket.Stages.Add(stage);
                UnityEngine.Debug.Log(maxStage);
            }

            //Put parts in stages
            for(int i = 0; i < maxSteps; i++)
            {
                RocketPart part = new RocketPart();
                part._partID = loadedRocket.PartsID[i];
                part._partType = loadedRocket.partType[i];
                UnityEngine.Vector2 position = new Vector2(launchPad.transform.position.x, launchPad.transform.position.y+launchPad.GetComponent<BoxCollider2D>().bounds.max.y/2 + 10);

                rocket.Stages[loadedRocket.StageNumber[i]].Parts.Add(part);
                rocket.Stages[loadedRocket.StageNumber[i]].PartsID.Add(part._partID);

                GameObject currentPart = SpawnPart(position, part._partType);
                newParts.Add(currentPart);

                UnityEngine.Debug.Log(currentPart);

                if(loadedRocket.coreID == loadedRocket.PartsID[i])
                {
                    currentPart.AddComponent<Rocket>();
                    rocket.core = currentPart;
                    core = currentPart;
                    //newParts.Remove(currentPart);
                }
            }

            core.GetComponent<Rocket>().core = core;
            core.GetComponent<Rocket>().Stages = rocket.Stages;

            //Parent core
            int j = 0;
            foreach(GameObject part in newParts)
            {
                if(part != core)
                {
                    part.transform.SetParent(core.transform);
                    part.transform.position = new UnityEngine.Vector3(loadedRocket.x_pos[j] + core.transform.position.x, loadedRocket.y_pos[j]+ core.transform.position.y, loadedRocket.z_pos[j]+ core.transform.position.z);
                }
                j++;
            }
        }
        filePath = null;
    }

    GameObject SpawnPart(UnityEngine.Vector3 position, string type)
    {
        GameObject go = null;
        if(type == "satellite")
        {
            go = Instantiate(capsulePrefab, position, UnityEngine.Quaternion.identity);
            UnityEngine.Debug.Log(go);
        }
        if(type == "engine")
        {
            go = Instantiate(Engine, position, UnityEngine.Quaternion.identity);
        }
        if(type == "tank")
        {
            go = Instantiate(Tank, position, UnityEngine.Quaternion.identity);
        }if(type == "decoupler")
        {
            go = Instantiate(Decoupler, position, UnityEngine.Quaternion.identity);
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
