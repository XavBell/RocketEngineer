using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PressureTestViewer : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject Stand;
    public TMP_Text status;
    public bool previouslyRan;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Stand != null)
        {
            updateStatus();
        }
    }

    void updateStatus()
    {
        if(Stand.GetComponent<standManager>().started == false && Stand.GetComponent<standManager>().failed == false && previouslyRan == false)
        {
            status.text = "waiting";
        }

        if(Stand.GetComponent<standManager>().started == true && Stand.GetComponent<standManager>().failed == false && previouslyRan == true)
        {
            status.text = "running";
        }

        if(Stand.GetComponent<standManager>().failed == true && previouslyRan == true)
        {
            status.text = "stopped";
        }
    }

    public void startTest()
    {
        Stand.GetComponent<standManager>().started = true;
        previouslyRan = true;
    }

    public void stopTest()
    {
        Stand.GetComponent<standManager>().failed = true;
    }

    public void Terminate()
    {
        Stand.GetComponent<standManager>().failed = true;
        Destroy(Stand.GetComponent<standManager>().ConnectedTank);
        this.gameObject.SetActive(false);
    }
}
