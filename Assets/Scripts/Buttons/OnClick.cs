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
            }

            UnityEngine.Vector2 position = new Vector2(launchPad.transform.position.x, launchPad.transform.position.y+launchPad.GetComponent<BoxCollider2D>().bounds.max.y/2 + 10);

            List<System.Guid> partsGuid = new List<System.Guid>();
            List<System.Guid> topGuid = new List<System.Guid>();
            List<System.Guid> bottomGuid = new List<System.Guid>();
            List<System.Guid> leftGuid = new List<System.Guid>();
            List<System.Guid> rightGuid = new List<System.Guid>();

            //Put parts in stages
            for(int i = 0; i < maxSteps; i++)
            {
                GameObject currentPart = SpawnPart(position, loadedRocket.partType[i]);
                newParts.Add(currentPart);
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
            }

            core.AddComponent<PlanetGravity>();
            core.GetComponent<PlanetGravity>().core = core;
            core.GetComponent<Rocket>().core = core;
            foreach(Stages stage in rocket.Stages)
            {
                core.GetComponent<Rocket>().Stages.Add(stage);
            }
            linkParts(newParts, topGuid,bottomGuid,rightGuid, leftGuid, partsGuid);

            //Parent core and set values to proper parts
            int j = 0;
            int tankID = 0;
            int engineID = 0;
            foreach(GameObject part in newParts)
            {
                if(part != core)
                {
                    part.transform.SetParent(core.transform);
                    part.transform.position = new UnityEngine.Vector3(loadedRocket.x_pos[j] + core.transform.position.x, loadedRocket.y_pos[j]+ core.transform.position.y, loadedRocket.z_pos[j]+ core.transform.position.z);

                    if(part.GetComponent<RocketPart>()._partType == "tank")
                    {
                        part.transform.localScale = new UnityEngine.Vector2(loadedRocket.x_scale[tankID], loadedRocket.y_scale[tankID]);
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
            
        }
        filePath = null;
    }

    public void linkParts(List<GameObject> parts, List<System.Guid> topRef, List<System.Guid> bottomRef, List<System.Guid> rightRef, List<System.Guid> leftRef, List<System.Guid> partsID)
    {
        int i = 0;
        foreach(System.Guid partID in partsID)
        {
            bool top = topRef.Contains(partID);
            bool bottom = bottomRef.Contains(partID);
            bool left = leftRef.Contains(partID);
            bool right = rightRef.Contains(partID);

            if(top == true)
            {
                int pos = topRef.IndexOf(partID);
                parts[pos].GetComponent<RocketPart>()._attachTop.GetComponent<AttachPointScript>().attachedBody = parts[i];
            }

            if(bottom == true)
            {
                int pos = bottomRef.IndexOf(partID);
                parts[pos].GetComponent<RocketPart>()._attachBottom.GetComponent<AttachPointScript>().attachedBody = parts[i];
            }

            if(left == true)
            {
                int pos = leftRef.IndexOf(partID);
                parts[pos].GetComponent<RocketPart>()._attachLeft.GetComponent<AttachPointScript>().attachedBody = parts[i];
            }

            if(right == true)
            {
                int pos = rightRef.IndexOf(partID);
                parts[pos].GetComponent<RocketPart>()._attachRight.GetComponent<AttachPointScript>().attachedBody = parts[i];
            }
            i++;
        }
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
