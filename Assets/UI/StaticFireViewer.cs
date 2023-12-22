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
        }
    }

    void updateStatus()
    {
        if(staticFireStand.GetComponent<staticFireStandManager>().started == false && staticFireStand.GetComponent<staticFireStandManager>().failed == false && previouslyRan == false)
        {
            status.text = "waiting";
        }

        if(staticFireStand.GetComponent<staticFireStandManager>().started == true && staticFireStand.GetComponent<staticFireStandManager>().failed == false && previouslyRan == true)
        {
            status.text = "running";
        }

        if(staticFireStand.GetComponent<staticFireStandManager>().failed == true && previouslyRan == true)
        {
            status.text = "stopped";
        }

        if(staticFireStand.GetComponent<staticFireStandManager>().failed == true)
        {
            status.text = "stopped";
        }
    }

    void updateQuantity()
    {
        oxidizerQty.text = staticFireStand.GetComponent<staticFireStandManager>().oxidizer.mass.ToString();
        fuelQty.text = staticFireStand.GetComponent<staticFireStandManager>().fuel.mass.ToString();
    }

    public void startTest()
    {
        staticFireStand.GetComponent<staticFireStandManager>().started = true;
        previouslyRan = true;
    }

    public void stopTest()
    {
        staticFireStand.GetComponent<staticFireStandManager>().failed = true;
    }

    public void Terminate()
    {
        staticFireStand.GetComponent<staticFireStandManager>().failed = true;
        Destroy(staticFireStand.GetComponent<staticFireStandManager>().ConnectedEngine);
        this.gameObject.SetActive(false);
    }
}
