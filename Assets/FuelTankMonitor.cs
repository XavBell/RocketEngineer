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
    [SerializeField]private TMP_Text quantity;
    [SerializeField]private TMP_Text substance;
    [SerializeField]private TMP_Text state;
    [SerializeField]private TMP_Text targetTemperature;

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
        pressure.text = outputInputManager.internalPressure.ToString();
        quantity.text = outputInputManager.mass.ToString();
        substance.text = outputInputManager.substance.ToString();
        state.text = outputInputManager.state.ToString();
    }

    public void updateTemp()
    {
        targetTemperature.text = target.text.ToString();
        outputInputManager.externalTemperature = Convert.ToSingle(target.text.ToString());
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
