using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RocketStaticFireViewer : MonoBehaviour
{
    public GameObject launchpad;
    public TMP_Text status;
    public bool previouslyRan = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(launchpad != null)
        {
            updateStatus();
        }
    }

    void updateStatus()
    {
        if(launchpad.GetComponent<launchPadManager>().started == false && launchpad.GetComponent<launchPadManager>().failed == false && previouslyRan == false)
        {
            status.text = "waiting";
        }

        if(launchpad.GetComponent<launchPadManager>().started == true && launchpad.GetComponent<launchPadManager>().failed == false && previouslyRan == true)
        {
            status.text = "running";
        }

        if(launchpad.GetComponent<launchPadManager>().failed == true && previouslyRan == true)
        {
            status.text = "stopped";
        }
    }

    public void startTest()
    {
        launchpad.GetComponent<launchPadManager>().started = true;
        previouslyRan = true;
    }

    public void stopTest()
    {
        launchpad.GetComponent<launchPadManager>().failed = true;
    }

    public void Terminate()
    {
        launchpad.GetComponent<launchPadManager>().failed = true;
        Destroy(launchpad.GetComponent<launchPadManager>().ConnectedRocket);
        this.gameObject.SetActive(false);
    }
}
