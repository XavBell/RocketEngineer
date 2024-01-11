using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PressureTestViewer : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject Stand;
    public TMP_Text status;
    public TMP_Text quantity;
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
            updateQuantity();
        }
    }

    void updateStatus()
    {
        if(Stand.GetComponent<standManager>().started == false)
        {
            status.text = "waiting";
        }

        if(Stand.GetComponent<standManager>().started == true)
        {
            status.text = "running";
        }

        if(Stand.GetComponent<standManager>().failed == true)
        {
            status.text = "stopped";
        }
    }

    void updateQuantity()
    {
        quantity.text = Stand.GetComponent<standManager>().ConnectedTank.GetComponent<outputInputManager>().mass.ToString();
    }

    public void startTest()
    {
        Stand.GetComponent<standManager>().started = true;
        previouslyRan = true;
        MasterManager masterManager = FindObjectOfType<MasterManager>();
        masterManager.GetComponent<pointManager>().nPoints += 2f;
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
