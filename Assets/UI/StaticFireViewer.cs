using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class StaticFireViewer : MonoBehaviour
{
    public GameObject staticFireStand;
    public GameObject Panel;
    public TMP_Text status;
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
}
