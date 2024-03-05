using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;


public class FuelTankMonitor : MonoBehaviour
{
    [SerializeField]private TMP_Text temperature;
    [SerializeField]private TMP_Text pressure;
    [SerializeField]private TMP_Text volume;
    [SerializeField]private TMP_Text quantity;
    [SerializeField]private TMP_Text substance;
    [SerializeField]private TMP_Text state;
    [SerializeField]private TMP_Text targetTemperature;
    [SerializeField]public Toggle coolerToggle;
    [SerializeField]public Toggle ventToggle;
    [SerializeField]private float rate;

    [SerializeField]public TMP_InputField target;
    [SerializeField]public container container;
    [SerializeField]public gasVent gasVent;
    [SerializeField]public cooler cooler;
    // Start is called before the first frame update
    void Start()
    {
        

        if(container != null)
        {
            if (container.GetComponent<gasVent>() != null)
            {
                ventToggle.isOn = container.GetComponent<gasVent>().open;
            }
            if (container.GetComponent<cooler>() != null)
            {
                coolerToggle.isOn = container.GetComponent<cooler>().active;
            }
            if(targetTemperature != null)
            {
                targetTemperature.text = container.GetComponent<cooler>().targetTemperature.ToString();
            }
        }
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
        print("value");
        if(coolerToggle.isOn == true)
        {
            cooler.active = true;
            return;
        }

        if(coolerToggle.isOn == false)
        {
            cooler.active = false;
            return;
        }
    }

    public void vent()
    {
        if(ventToggle.isOn == true)
        {
            gasVent.open = true;
            return;
        }

        if(ventToggle.isOn == false)
        {
            gasVent.open = false;
            return;
        }
    }
}
