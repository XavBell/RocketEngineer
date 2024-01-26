using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class flowController : MonoBehaviour
{
    public container origin;
    public container destination;

    public float selfRate; //kg/s
    public bool opened = false; //is flow allowed or not

    public TimeManager MyTime;


    // Start is called before the first frame update
    void Start()
    {
        MyTime = FindObjectOfType<TimeManager>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(destination)
        {
            flow(selfRate);
        }
    }

    void flow(float rate)
    {
        if(opened == true)
        {
            float massToTransfer = rate * MyTime.deltaTime * 1000; //value in kg converted to g
            float molesToTransfer = massToTransfer/origin.substance.MolarMass;

            if(destination.mass == 0 && origin.mass > 0)
            {
                destination.substance = origin.substance;
                destination.internalTemperature = origin.internalTemperature;
            }

            if(origin.moles - molesToTransfer >= 0)
            {
                origin.moles -= molesToTransfer;
                destination.moles += molesToTransfer;
            }else if(origin.moles - molesToTransfer < 0)
            {
                destination.moles += origin.moles;
                origin.moles = 0;
            }
        }
    }
}
