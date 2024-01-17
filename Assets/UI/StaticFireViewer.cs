using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StaticFireViewer : MonoBehaviour
{
    public GameObject staticFireStand;
    public TMP_Text status;
    public TMP_Text oxidizerQty;
    public TMP_Text fuelQty;
    public TMP_Text thrust;
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
    }

    void updateThrust()
    {
        if(staticFireStand.GetComponent<staticFireStandManager>().ConnectedEngine.GetComponent<Engine>().outReadThrust != float.NaN)
        {
            thrust.text = staticFireStand.GetComponent<staticFireStandManager>().ConnectedEngine.GetComponent<Engine>().outReadThrust.ToString();
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
}
