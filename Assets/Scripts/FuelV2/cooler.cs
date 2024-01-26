using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cooler : MonoBehaviour
{
    public container container;
    public float targetTemperature;
    public bool active = false; 
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        updateContainer();
    }

    void updateContainer()
    {
        if(active == true)
        {
            container.coolerActive = true;
            container.targetTemperature = targetTemperature;
            return;
        }

        if(active == false)
        {
            container.coolerActive = false;
            return;
        }
    }
}
