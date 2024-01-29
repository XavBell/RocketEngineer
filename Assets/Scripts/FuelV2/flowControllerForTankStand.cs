using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class flowControllerForTankStand : MonoBehaviour
{
    public container origin;
    public Guid originGuid;
    public standManager standManager;
    bool connected  = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(standManager.ConnectedTank != null && connected == false)
        {
            connectTank();
            connected = true;
        }
    }

    public void updateGuid()
    {
        if(origin)
        {
            originGuid = origin.guid;
        } 
    }

    void connectTank()
    {
        standManager.ConnectedTank.GetComponent<flowController>().origin = origin;
    }

}
