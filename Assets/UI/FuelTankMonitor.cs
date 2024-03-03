using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using UnityEditor.ShaderGraph.Serialization;

public class FuelTankMonitor : MonoBehaviour
{
    [SerializeField]private TMP_Text temperature;
    [SerializeField]private TMP_Text pressure;
    [SerializeField]private TMP_Text volume;
    [SerializeField]private TMP_Text quantity;
    [SerializeField]private TMP_Text substance;
    [SerializeField]private TMP_Text state;
    [SerializeField]private TMP_Text targetTemperature;
    [SerializeField]private float rate;

    [SerializeField]private TMP_InputField target;
    [SerializeField]public container container;
    [SerializeField]public gasVent gasVent;
    [SerializeField]public cooler cooler;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(container != null)
        {
            if(container.substance != null)
            {
                updateValues();
            }
        }
    }

    void updateValues()
    {
        temperature.text = container.internalTemperature.ToString();
        volume.text = container.volume.ToString();
        pressure.text = container.internalPressure.ToString();
        quantity.text = container.mass.ToString();
        substance.text = container.substance.ToString();
        state.text = container.state.ToString();
        updateTemp();
    }

    public void updateTemp()
    {
        float tryN = 0;
        if(float.TryParse(target.text.ToString(), out tryN))
        {
            cooler.targetTemperature = tryN;
            return;
        }else{
            cooler.targetTemperature = container.targetTemperature;
        }
    }

    public void coolActivate()
    {
        if(cooler.active == false)
        {
            cooler.active = true;
            return;
        }

        if(cooler.active == true)
        {
            cooler.active = false;
            return;
        }
    }

    public void vent()
    {
        if(gasVent.open == false)
        {
            gasVent.open = true;
            return;
        }

        if(gasVent.open == true)
        {
            gasVent.open = false;
            return;
        }
    }
}
