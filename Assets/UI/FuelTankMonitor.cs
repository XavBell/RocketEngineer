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
    [SerializeField]private float rate;

    [SerializeField]private TMP_InputField target;
    [SerializeField]private outputInputManager outputInputManager;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        updateValues();
    }

    void updateValues()
    {
        temperature.text = outputInputManager.internalTemperature.ToString();
        volume.text = outputInputManager.volume.ToString();
        pressure.text = outputInputManager.internalPressure.ToString();
        quantity.text = outputInputManager.mass.ToString();
        substance.text = outputInputManager.substance.ToString();
        state.text = outputInputManager.state.ToString();
        updateTemp();
    }

    public void updateTemp()
    {
        //targetTemperature.text = target.text.ToString();
        float tryN = 0;
        if(float.TryParse(target.text.ToString(), out tryN))
        {
            outputInputManager.targetTemperature = tryN;
            return;
        }else{
            outputInputManager.targetTemperature = outputInputManager.internalTemperature;
        }
    }

    public void valve()
    {
        if(outputInputManager.selfRate != 0)
        {
            closeValve();
            return;
        }else{
            openValve(rate);
        }
    }

    public void cooler()
    {
        if(outputInputManager.coolerActive == false)
        {
            outputInputManager.coolerActive = true;
            return;
        }

        if(outputInputManager.coolerActive == true)
        {
            outputInputManager.coolerActive = false;
            return;
        }
    }

    public void vent()
    {
        if(outputInputManager.ventActive == false)
        {
            outputInputManager.ventActive = true;
            return;
        }

        if(outputInputManager.ventActive == true)
        {
            outputInputManager.ventActive = false;
            return;
        }
    }
    public void openValve(float rate)
    {
        outputInputManager.selfRate = rate;
    }

    public void closeValve()
    {
        outputInputManager.selfRate = 0;
    }
    
}
