using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TankUIModule : MonoBehaviour
{
    [SerializeField]TMP_Text internalPressure;
    [SerializeField]TMP_Text internalTemperature;
    [SerializeField]TMP_Text quantity;
    public container tank;
    [SerializeField]private TMP_Text targetTemperature;
    public float rate = 5000;
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
        internalPressure.text = tank.internalPressure.ToString();
        internalTemperature.text = tank.internalTemperature.ToString();
        quantity.text = tank.mass.ToString();
    }

    public void updateTemp()
    {
        float tryN = 0;
        if(float.TryParse(targetTemperature.text.ToString(), out tryN))
        {
            tank.targetTemperature = tryN;
            return;
        }else{
            tank.targetTemperature = tank.internalTemperature;
        }
    }

    public void valve()
    {
        //if(tank.selfRate != 0)
        //{
        //    closeValve();
        //    return;
        //}else{
        //    openValve(rate);
        //}
    }

    public void cooler()
    {
        if(tank.GetComponent<cooler>().active == false)
        {
            tank.GetComponent<cooler>().active = true;
            return;
        }

        if(tank.GetComponent<cooler>().active == true)
        {
            tank.GetComponent<cooler>().active = false;
            return;
        }
    }

    public void vent()
    {
        if(tank.GetComponent<gasVent>().open == false)
        {
            tank.GetComponent<gasVent>().open  = true;
            return;
        }

        if(tank.GetComponent<gasVent>().open  == true)
        {
            tank.GetComponent<gasVent>().open  = false;
            return;
        }
    }
    public void openValve(float rate)
    {
        //tank.selfRate = rate;
    }

    public void closeValve()
    {
        //tank.selfRate = 0;
    }
}
