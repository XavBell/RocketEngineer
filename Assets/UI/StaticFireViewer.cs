using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StaticFireViewer : MonoBehaviour
{
    public GameObject staticFireStand;
    public TMP_Text status;
    public TMP_Text oxidizerQty;
    public TMP_Text fuelQty;
    public TMP_Text oxidizerVolume;
    public TMP_Text fuelVolume;
    public TMP_Text fuelTemperature;
    public TMP_Text oxidizerTemperature;
    public TMP_Text thrust;

    public Toggle coolerFuel;
    public Toggle coolerOxidizer;

    public Toggle ventFuel;
    public Toggle ventOxidizer;

    public Toggle valveFuel;
    public Toggle valveOxidizer;

    public TMP_InputField targetTempFuel;
    public TMP_InputField targetTempOxidizer;

    public bool previouslyRan = false;
    // Start is called before the first frame update
    void Start()
    {
        coolerFuel.isOn = staticFireStand.GetComponent<staticFireStandManager>().fuel.GetComponent<cooler>().active;
        coolerOxidizer.isOn = staticFireStand.GetComponent<staticFireStandManager>().oxidizer.GetComponent<cooler>().active;

        ventFuel.isOn = staticFireStand.GetComponent<staticFireStandManager>().fuel.GetComponent<gasVent>().open;
        ventOxidizer.isOn = staticFireStand.GetComponent<staticFireStandManager>().oxidizer.GetComponent<gasVent>().open;

        valveFuel.isOn = staticFireStand.GetComponent<staticFireStandManager>().fuel.GetComponent<flowController>().opened;
        valveOxidizer.isOn = staticFireStand.GetComponent<staticFireStandManager>().oxidizer.GetComponent<flowController>().opened;

        targetTempFuel.text = staticFireStand.GetComponent<staticFireStandManager>().fuel.GetComponent<cooler>().targetTemperature.ToString();
        targetTempOxidizer.text = staticFireStand.GetComponent<staticFireStandManager>().oxidizer.GetComponent<cooler>().targetTemperature.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if (staticFireStand != null)
        {
            updateStatus();
            updateQuantity();
            updateThrust();
            updateTemp("fuel");
            updateTemp("oxidizer");

            if(targetTempFuel.text == "")
            {
                targetTempFuel.text = staticFireStand.GetComponent<staticFireStandManager>().fuel.GetComponent<cooler>().targetTemperature.ToString();
            }

            if(targetTempOxidizer.text == "")
            {
                targetTempOxidizer.text = staticFireStand.GetComponent<staticFireStandManager>().oxidizer.GetComponent<cooler>().targetTemperature.ToString();
            }
        }
    }

    void updateStatus()
    {
        if (staticFireStand.GetComponent<staticFireStandManager>().started == false && previouslyRan == false)
        {
            status.text = "waiting";
        }

        if (staticFireStand.GetComponent<staticFireStandManager>().started == true)
        {
            status.text = "running";
        }

        if (staticFireStand.GetComponent<staticFireStandManager>().stopped == true)
        {
            status.text = "stopped";
        }

    }

    void updateQuantity()
    {
        oxidizerQty.text = staticFireStand.GetComponent<staticFireStandManager>().oxidizer.mass.ToString();
        fuelQty.text = staticFireStand.GetComponent<staticFireStandManager>().fuel.mass.ToString();

        oxidizerVolume.text = staticFireStand.GetComponent<staticFireStandManager>().oxidizer.volume.ToString() + "/" + staticFireStand.GetComponent<staticFireStandManager>().oxidizer.tankVolume.ToString();
        fuelVolume.text = staticFireStand.GetComponent<staticFireStandManager>().fuel.volume.ToString() + "/" + staticFireStand.GetComponent<staticFireStandManager>().fuel.tankVolume.ToString();

        fuelTemperature.text = staticFireStand.GetComponent<staticFireStandManager>().fuel.internalTemperature.ToString();
        oxidizerTemperature.text = staticFireStand.GetComponent<staticFireStandManager>().oxidizer.internalTemperature.ToString();

        

    }

    void updateThrust()
    {
        if (staticFireStand.GetComponent<staticFireStandManager>().ConnectedEngine != null)
        {
            if (staticFireStand.GetComponent<staticFireStandManager>().ConnectedEngine.GetComponent<Engine>() != null)
            {
                if (staticFireStand.GetComponent<staticFireStandManager>().ConnectedEngine.GetComponent<Engine>().outReadThrust != float.NaN)
                {
                    thrust.text = staticFireStand.GetComponent<staticFireStandManager>().ConnectedEngine.GetComponent<Engine>().outReadThrust.ToString();
                }
            }
        }


    }

    public void startTest()
    {
        staticFireStand.GetComponent<staticFireStandManager>().startTime = (float)FindObjectOfType<TimeManager>().time;
        staticFireStand.GetComponent<staticFireStandManager>().stopped = false;
        staticFireStand.GetComponent<staticFireStandManager>().started = true;
        staticFireStand.GetComponent<staticFireStandManager>().ConnectedEngine.GetComponent<Engine>().InitializeFail();
        //previouslyRan = true;
        MasterManager masterManager = FindObjectOfType<MasterManager>();
        masterManager.GetComponent<pointManager>().nPoints += 2f;
    }

    public void stopTest()
    {
        staticFireStand.GetComponent<staticFireStandManager>().stopped = true;
    }

    public void Terminate()
    {
        staticFireStand.GetComponent<staticFireStandManager>().stopped = true;
        staticFireStand.GetComponent<staticFireStandManager>().failed = false;
        Destroy(staticFireStand.GetComponent<staticFireStandManager>().ConnectedEngine);
        this.gameObject.SetActive(false);
    }

    public void openValve(string category)
    {
        if (category == "fuel")
        {
            staticFireStand.GetComponent<staticFireStandManager>().fuelController.opened = valveFuel.isOn;
        }

        if (category == "oxidizer")
        {
            staticFireStand.GetComponent<staticFireStandManager>().oxidizerController.opened = valveOxidizer.isOn;
        }
    }

    public void openVent(string type)
    {
        gasVent[] gasVents = staticFireStand.GetComponents<gasVent>();
        gasVent fuelVent = null;
        gasVent oxidizerVent = null;
        foreach (gasVent gasVent in gasVents)
        {
            if (gasVent.container.type == "fuel")
            {
                fuelVent = gasVent;
            }

            if (gasVent.container.type == "oxidizer")
            {
                oxidizerVent = gasVent;
            }
        }

        if (type == "fuel")
        {
            fuelVent.open = ventFuel.isOn;
        }

        if (type == "oxidizer")
        {
            oxidizerVent.open = ventOxidizer.isOn;
        }
    }

    public void activateCooler(string type)
    {
        cooler[] coolers = staticFireStand.GetComponents<cooler>();
        cooler fuelCooler = null;
        cooler oxidizerCooler = null;
        foreach (cooler cooler in coolers)
        {
            if (cooler.container.type == "fuel")
            {
                fuelCooler = cooler;
            }

            if (cooler.container.type == "oxidizer")
            {
                oxidizerCooler = cooler;
            }
        }

        if (fuelCooler != null)
        {
            if (type == "fuel")
            {
                fuelCooler.active = coolerFuel.isOn;
            }
        }

        if (oxidizerCooler != null)
        {
            if (type == "oxidizer")
            {
                oxidizerCooler.active = coolerOxidizer.isOn;
            }
        }


    }

    public void updateTemp(string type)
    {
        cooler[] coolers = staticFireStand.GetComponents<cooler>();
        cooler fuelCooler = null;
        cooler oxidizerCooler = null;
        foreach (cooler cooler in coolers)
        {
            if (cooler.container.type == "fuel")
            {
                fuelCooler = cooler;
            }

            if (cooler.container.type == "oxidizer")
            {
                oxidizerCooler = cooler;
            }
        }

        if (type == "oxidizer" && oxidizerCooler != null)
        {
            float tryN = 0;
            if (float.TryParse(targetTempOxidizer.text.ToString(), out tryN))
            {
                if(tryN < 0)
                {
                    oxidizerCooler.targetTemperature = 0;
                    return;
                }else{
                    oxidizerCooler.targetTemperature = tryN;
                    return;
                }
            }
        }

        if (type == "fuel" && fuelCooler != null)
        {
            float tryN = 0;
            if (float.TryParse(targetTempFuel.text.ToString(), out tryN))
            {
                if(tryN < 0)
                {
                    fuelCooler.targetTemperature = 0;
                    return;
                }else{
                    fuelCooler.targetTemperature = tryN;
                    return;
                }
            }

        }
    }

}
