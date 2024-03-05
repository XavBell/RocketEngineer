using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.ShaderGraph.Serialization;
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

    }

    // Update is called once per frame
    void Update()
    {
        if (staticFireStand != null)
        {
            updateStatus();
            updateQuantity();
            updateThrust();
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
            if (staticFireStand.GetComponent<staticFireStandManager>().fuelController.opened == false)
            {
                staticFireStand.GetComponent<staticFireStandManager>().fuelController.opened = true;
                return;
            }

            if (staticFireStand.GetComponent<staticFireStandManager>().fuelController.opened == true)
            {
                staticFireStand.GetComponent<staticFireStandManager>().fuelController.opened = false;
                return;
            }
        }

        if (category == "oxidizer")
        {
            if (staticFireStand.GetComponent<staticFireStandManager>().oxidizerController.opened == false)
            {
                staticFireStand.GetComponent<staticFireStandManager>().oxidizerController.opened = true;
                return;
            }

            if (staticFireStand.GetComponent<staticFireStandManager>().oxidizerController.opened == true)
            {
                staticFireStand.GetComponent<staticFireStandManager>().oxidizerController.opened = false;
                return;
            }
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
            if (fuelVent.open == false)
            {
                fuelVent.open = true;
                return;
            }

            if (fuelVent.open == true)
            {
                fuelVent.open = false;
                return;
            }
        }

        if (type == "oxidizer")
        {
            if (oxidizerVent.open == false)
            {
                oxidizerVent.open = true;
                return;
            }

            if (oxidizerVent.open == true)
            {
                oxidizerVent.open = false;
                return;
            }
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
                if (fuelCooler.active == false)
                {
                    fuelCooler.active = true;
                    return;
                }

                if (fuelCooler.active == true)
                {
                    fuelCooler.active = false;
                    return;
                }
            }
        }

        if (oxidizerCooler != null)
        {
            if (type == "oxidizer")
            {
                if (oxidizerCooler.active == false)
                {
                    oxidizerCooler.active = true;
                    return;
                }

                if (oxidizerCooler.active == true)
                {
                    oxidizerCooler.active = false;
                    return;
                }
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
                oxidizerCooler.targetTemperature = tryN;
                return;
            }
        }

        if (type == "fuel" && fuelCooler != null)
        {
            float tryN = 0;
            if (float.TryParse(targetTempFuel.text.ToString(), out tryN))
            {
                fuelCooler.targetTemperature = tryN;
                return;
            }

        }
    }

}
