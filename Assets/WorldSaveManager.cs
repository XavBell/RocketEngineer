using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]
public class WorldSaveManager : MonoBehaviour
{
    void FixedUpdate()
    {
        saveWorld();
    }


    public void saveWorld()
    {
        saveWorld saveWorld = new saveWorld();
        public GameObject[] rockets = GameObject.FindGameObjectsWithTag("capsule");
        i = 0;
        foreach(GameObject rocket in rockets)
        {
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
    }

    public void loadWorld()
    {
        //TODO
    }

}
