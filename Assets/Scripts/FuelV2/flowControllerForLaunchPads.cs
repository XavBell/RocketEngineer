using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class flowControllerForLaunchPads : MonoBehaviour
{
    public launchPadManager launchPadManager;

    public Guid oxidizerGuid;
    public Guid fuelGuid;
    public container oxidizerContainerOrigin;
    public container fuelContainerOrigin;

    public RocketController rocket;

    public bool connected = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(launchPadManager.ConnectedRocket != null && connected == false)
        {
            setTankOrigin();
            connected = true;
        }

        if(launchPadManager.ConnectedRocket == null && connected == true)
        {
            Disconnect();
            connected = false;
        }
    }

    public void updateGuid()
    {
        if(oxidizerContainerOrigin)
        {
            oxidizerGuid = oxidizerContainerOrigin.guid;
        } 

        if(fuelContainerOrigin)
        {
            fuelGuid = fuelContainerOrigin.guid;
        }  
    }


    public void setTankOrigin()
    {
        rocket = launchPadManager.ConnectedRocket.GetComponent<RocketController>();
        Transform[] children = rocket.GetComponentsInChildren<Transform>();
        container[] containers = FindObjectsOfType<container>();
        foreach(Transform child in children)
        {
            if(child.GetComponent<TankComponent>() == null)
            {
                continue;
            }
            if(launchPadManager.connectedRocketLines.IndexOf(child.GetComponent<TankComponent>().lineGuid) != -1)
            {
                if(launchPadManager.connectedContainersPerLine.Count > launchPadManager.connectedRocketLines.IndexOf(child.GetComponent<TankComponent>().lineGuid))
                {
                    child.GetComponent<flowController>().originGuid = launchPadManager.connectedContainersPerLine[launchPadManager.connectedRocketLines.IndexOf(child.GetComponent<TankComponent>().lineGuid)];
                    foreach(container container in containers)
                    {
                        if(container.guid == child.GetComponent<flowController>().originGuid)
                        {
                            child.GetComponent<flowController>().origin = container;
                        }
                    }
                }
                
            }
            
        }


    }

    void Disconnect()
    {

    }
}
