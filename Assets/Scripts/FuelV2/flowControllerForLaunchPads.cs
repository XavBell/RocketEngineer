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

    Rocket rocket;

    bool connected = false;

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
        rocket = launchPadManager.ConnectedRocket.GetComponent<Rocket>();
        foreach(Stages stage in rocket.Stages)
        {
            foreach(RocketPart part in stage.Parts)
            {
                if(part._partType == "tank")
                {
                    if(part.GetComponent<Tank>().propellantCategory == "oxidizer")
                    {
                        part.GetComponent<flowController>().origin = oxidizerContainerOrigin;
                    }

                    if(part.GetComponent<Tank>().propellantCategory == "fuel")
                    {
                        part.GetComponent<flowController>().origin = fuelContainerOrigin;
                    }
                }
            }
        }
    }

    void Disconnect()
    {
        if(rocket != null)
        {
            foreach(Stages stage in rocket.Stages)
            {
                foreach(RocketPart part in stage.Parts)
                {
                    if(part.gameObject != null)
                    {
                        if(part._partType == "tank")
                        {
                            if(part.GetComponent<Tank>().propellantCategory == "oxidizer")
                            {
                                part.GetComponent<flowController>().origin = null;
                            }

                            if(part.GetComponent<Tank>().propellantCategory == "fuel")
                            {
                                part.GetComponent<flowController>().origin = null;
                            }
                        }
                    }
                }
            }
        }
    }
}
