using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System;
using System.Runtime.Serialization;
using System.Xml.Linq;
using System.Text;
using Newtonsoft.Json;

public class WorldSaveManager : MonoBehaviour
{
    public GameObject capsulePrefab;
    public GameObject tankPrefab;
    public GameObject enginePrefab;
    public bool loaded = false;
    // Start is called before the first frame update
    void Start()
    {
        loadWorld();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.J))
        {
            saveTheWorld();
        }

        if(Input.GetKey(KeyCode.H) && loaded == false)
        {
            loadWorld();
        }
    }


    public void saveTheWorld()
    {
        saveWorld saveWorld = new saveWorld();
        GameObject[] rockets = GameObject.FindGameObjectsWithTag("capsule");
        foreach(GameObject rocket in rockets)
        {
            int i = 0;
            saveWorld.childrenNumber.Add(i);
            if(rocket.GetComponent<Part>().attachBottom.GetComponent<AttachPointScript>().attachedBody != null)
            {
                GameObject referenceBody = rocket;
                while(referenceBody.GetComponent<Part>().attachBottom.GetComponent<AttachPointScript>().attachedBody != null)
                {
                    saveWorld.types.Add(referenceBody.GetComponent<Part>().attachBottom.GetComponent<AttachPointScript>().attachedBody.GetComponent<Part>().type);
                    referenceBody = referenceBody.GetComponent<Part>().attachBottom.GetComponent<AttachPointScript>().attachedBody;
                    saveWorld.childrenNumber[i]++; 
                }
            }
            i++;
        }

        var jsonString = JsonConvert.SerializeObject(saveWorld);
        System.IO.File.WriteAllText(Application.persistentDataPath +  "/world.json", jsonString);
        Debug.Log("saved");
    }

    public void loadWorld()
    {
        saveWorld saveWorld = new saveWorld();
        var jsonString = JsonConvert.SerializeObject(saveWorld);
        jsonString = File.ReadAllText(Application.persistentDataPath + "/world.json");
        saveWorld loadedWorld = JsonConvert.DeserializeObject<saveWorld>(jsonString);
        int alreadyUsed = 0;
        foreach(int rocket in loadedWorld.childrenNumber)
        {
            int childrenNumber = rocket;
            GameObject capsule = Instantiate(capsulePrefab, new Vector3(1 , 51, 1), Quaternion.identity);
            capsule.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            GameObject currentPrefab = capsule;
            int i = 0;
            while(i < childrenNumber)
            {
                GameObject previousPrefab = currentPrefab;
                if(loadedWorld.types[i + alreadyUsed] == "tank")
                {
                    currentPrefab = Instantiate(tankPrefab, previousPrefab.GetComponent<Part>().attachBottom.transform.position, Quaternion.identity);
                    currentPrefab.transform.SetParent(capsule.transform);
                    previousPrefab.GetComponent<Part>().attachBottom.GetComponent<AttachPointScript>().attachedBody = currentPrefab;
                    currentPrefab.GetComponent<Part>().attachTop.GetComponent<AttachPointScript>().attachedBody = previousPrefab;
                }

                if(loadedWorld.types[i + alreadyUsed] == "engine")
                {
                    currentPrefab = Instantiate(enginePrefab, previousPrefab.GetComponent<Part>().attachBottom.transform.position, Quaternion.identity);
                    currentPrefab.transform.SetParent(capsule.transform);
                    previousPrefab.GetComponent<Part>().attachBottom.GetComponent<AttachPointScript>().attachedBody = currentPrefab;
                    currentPrefab.GetComponent<Part>().attachTop.GetComponent<AttachPointScript>().attachedBody = previousPrefab;
                }

                currentPrefab.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                i++;
            }
            
            alreadyUsed += childrenNumber;
        }
        loaded = true;
    }


}
