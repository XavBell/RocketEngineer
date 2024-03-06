using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TankUIModule : MonoBehaviour
{
    [SerializeField]TMP_Text internalPressure;
    [SerializeField]TMP_Text internalTemperature;
    [SerializeField]TMP_Text volume;
    [SerializeField]TMP_Text quantity;
    [SerializeField]Toggle coolerToggle;
    [SerializeField]Toggle ventToggle;
    [SerializeField]Toggle valveToggle;
    public container tank;
    [SerializeField]private TMP_InputField targetTemperature;
    
    public float rate = 5000;
    // Start is called before the first frame update
    void Start()
    {
        coolerToggle.isOn = tank.GetComponent<cooler>().active;
        ventToggle.isOn = tank.GetComponent<gasVent>().open;
        valveToggle.isOn = tank.GetComponent<flowController>().opened;
        targetTemperature.text = tank.GetComponent<cooler>().targetTemperature.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if(tank != null)
        {
            updateValues();    
        }
        
    }

    void updateValues()
    {
        internalPressure.text = tank.internalPressure.ToString();
        internalTemperature.text = tank.internalTemperature.ToString();
        volume.text = tank.volume.ToString() + "/" + tank.tankVolume.ToString();
        
        if(tank.GetComponent<Tank>().tested == false)
        {
            quantity.text = tank.mass.ToString();
        }else if(tank.GetComponent<Tank>().tested == true)
        {
            if(tank.substance != null)
            {
                //mass = volume * density
                float maxQuantity = tank.tankVolume * tank.substance.Density;
                quantity.text = tank.mass.ToString() + "/" + maxQuantity.ToString();
            }else{
                quantity.text = tank.mass.ToString();
            }
        }
    }

    public void updateTemp()
    {
        float tryN = 0;
        if(float.TryParse(targetTemperature.text.ToString(), out tryN))
        {
            tank.GetComponent<cooler>().targetTemperature = tryN;
            return;
        }else{
            tank.GetComponent<cooler>().targetTemperature = tank.internalTemperature;
        }
    }

    public void valve()
    {
        tank.GetComponent<flowController>().opened = valveToggle.isOn;
    }

    public void cooler()
    {
        
        tank.GetComponent<cooler>().active = coolerToggle.isOn; 
    }

    public void vent()
    {
        tank.GetComponent<gasVent>().open = ventToggle.isOn;
    }
}
