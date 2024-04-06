using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class gasVent : MonoBehaviour
{
    public container container;
    public float ventRate; //kg/s
    public bool open;
    TimeManager MyTime;

    // Start is called before the first frame update
    void Start()
    {
        MyTime = FindObjectOfType<TimeManager>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(container != null)
        {
            if(container.substance != null)
            {
                expell(ventRate);
            }
        }
    }

    void expell(float rate)
    {
        if(open == true)
        {
            float massToTransfer = rate * MyTime.deltaTime * 1000; //value in kg converted to g
            float molesToTransfer = massToTransfer/container.substance.MolarMass;

            if(container.moles - molesToTransfer >= 0)
            {
                container.moles -= molesToTransfer;
            }else if(container.moles - molesToTransfer < 0)
            {
                container.moles = 0;
            }
        }
    }
}
