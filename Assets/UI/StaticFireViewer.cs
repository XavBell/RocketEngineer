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
        if(staticFireStand != null)
        {
            updateStatus();
            updateQuantity();
            updateThrust();
        }
    }

    void updateStatus()
    {
        if(staticFireStand.GetComponent<staticFireStandManager>().started == false && previouslyRan == false)
        {
            status.text = "waiting";
        }

        if(staticFireStand.GetComponent<staticFireStandManager>().started == true)
        {
            status.text = "running";
        }

        if(staticFireStand.GetComponent<staticFireStandManager>().stopped == true)
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
        
    }

    void updateThrust()
    {
        if(staticFireStand.GetComponent<staticFireStandManager>().ConnectedEngine.GetComponent<Engine>() != null)
        {
            if(staticFireStand.GetComponent<staticFireStandManager>().ConnectedEngine.GetComponent<Engine>().outReadThrust != float.NaN)
            {
                thrust.text = staticFireStand.GetComponent<staticFireStandManager>().ConnectedEngine.GetComponent<Engine>().outReadThrust.ToString();
            }
        }
        
    }

    public void startTest()
    {
        staticFireStand.GetComponent<staticFireStandManager>().startTime = (float)FindObjectOfType<TimeManager>().time;
        staticFireStand.GetComponent<staticFireStandManager>().stopped = false;
        staticFireStand.GetComponent<staticFireStandManager>().started = true;
        staticFireStand.GetComponent<staticFireStandManager>().ConnectedEngine.GetComponent<Engine>().InitializeFail();
        previouslyRan = true;
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

    public void openValve(flowController flowController)
    {
        if(flowController.opened == false)
        {
            flowController.opened = true;
            return;
        }

        if(flowController.opened == true)
        {
            flowController.opened = false;
            return;
        }
    }

    public void openVent(gasVent gasVent)
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
