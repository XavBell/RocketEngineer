using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class WDRTestViewer : MonoBehaviour
{
    public GameObject launchPad;
    public TMP_Text status;
    public bool previouslyRan;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(launchPad != null)
        {
            updateStatus();
        }
        
    }

    void updateStatus()
    {
        if(launchPad.GetComponent<launchPadManager>().started == false && launchPad.GetComponent<launchPadManager>().failed == false && previouslyRan == false)
        {
            status.text = "waiting";
        }

        if(launchPad.GetComponent<launchPadManager>().started == true && launchPad.GetComponent<launchPadManager>().failed == false && previouslyRan == true)
        {
            status.text = "running";
        }

        if(launchPad.GetComponent<launchPadManager>().failed == true && previouslyRan == true)
        {
            status.text = "stopped";
        }
    }

    public void startTest()
    {
        launchPad.GetComponent<launchPadManager>().started = true;
        previouslyRan = true;
        MasterManager masterManager = FindObjectOfType<MasterManager>();
        masterManager.GetComponent<pointManager>().nPoints += 2f;
    }

    public void stopTest()
    {
        launchPad.GetComponent<launchPadManager>().failed = true;
    }

    public void Terminate()
    {
        launchPad.GetComponent<launchPadManager>().failed = true;
        Destroy(launchPad.GetComponent<launchPadManager>().ConnectedRocket);
        this.gameObject.SetActive(false);
    }
}
