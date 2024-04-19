using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class flowController : MonoBehaviour
{
    public Guid originGuid;
    public Guid destinationGuid;
    public container origin;
    public container destination;

    public float selfRate; //kg/s
    public bool opened = false; //is flow allowed or not

    public TimeManager MyTime;
    public GameObject refLine;


    // Start is called before the first frame update
    void Start()
    {
        MyTime = FindObjectOfType<TimeManager>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(destination && origin)
        {
            flow(selfRate);
        }

        if(originGuid == Guid.Empty || destinationGuid == Guid.Empty)
        {
            updateGuid();
        }
    }

    public void updateGuid()
    {
        if(destination)
        {
            destinationGuid = destination.guid;
        } 

        if(origin)
        {
            originGuid = origin.guid;
        }  
    }

    void flow(float rate)
    {
        if(opened == true && origin.substance != null)
        {
            float massToTransfer = rate * MyTime.deltaTime * 1000; //value in kg converted to g
            float molesToTransfer = massToTransfer/origin.substance.MolarMass;

            if(origin.mass > 0)
            {
                destination.substance = origin.substance;
                destination.internalTemperature = origin.internalTemperature;
            }
            
            if(destination.tested == false)
            {
                if (origin.moles - molesToTransfer >= 0)
                {
                    origin.moles -= molesToTransfer;
                    destination.moles += molesToTransfer;
                }
                else if (origin.moles - molesToTransfer < 0)
                {
                    destination.moles += origin.moles;
                    origin.moles = 0;
                }
            }

            if(destination.tested == true)
            {
                float volumeToTransfer = massToTransfer / (destination.substance.Density *1000);
                if(volumeToTransfer + destination.volume <= destination.tankVolume)
                {
                    if (origin.moles - molesToTransfer >= 0)
                    {
                        origin.moles -= molesToTransfer;
                        destination.moles += molesToTransfer;
                    }
                    else if (origin.moles - molesToTransfer < 0)
                    {
                        destination.moles += origin.moles;
                        origin.moles = 0;
                    }
                }
            }
            
        }
    }
}
