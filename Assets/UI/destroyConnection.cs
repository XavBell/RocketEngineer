using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class destroyConnection : MonoBehaviour
{
    public GameObject origin;
    public GameObject flowcontroller;
    public GameObject line;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void destroy()
    {
        if (line != null)
        {
            Destroy(line);
        }

        if (flowcontroller != null)
        {


            if (flowcontroller.GetComponent<flowController>())
            {
                flowController fc = flowcontroller.GetComponent<flowController>();
                fc.origin = null;
                fc.destination = null;
                fc.originGuid = System.Guid.Empty;
                fc.destinationGuid = System.Guid.Empty;
            }

            if (flowcontroller.GetComponent<flowControllerForLaunchPads>())
            {
                if (flowcontroller.GetComponent<flowControllerForLaunchPads>().fuelContainerOrigin != null)
                {
                    flowcontroller.GetComponent<flowControllerForLaunchPads>().fuelContainerOrigin = null;
                    flowcontroller.GetComponent<flowControllerForLaunchPads>().fuelGuid = System.Guid.Empty;
                }

                if (flowcontroller.GetComponent<flowControllerForLaunchPads>().oxidizerContainerOrigin != null)
                {
                    flowcontroller.GetComponent<flowControllerForLaunchPads>().oxidizerContainerOrigin = null;
                    flowcontroller.GetComponent<flowControllerForLaunchPads>().oxidizerGuid = System.Guid.Empty;
                }
            }

            if (flowcontroller.GetComponent<flowControllerForTankStand>())
            {
                flowcontroller.GetComponent<flowControllerForTankStand>().origin = null;
                flowcontroller.GetComponent<flowControllerForTankStand>().originGuid = System.Guid.Empty;
            }
        }

        Destroy(this.gameObject);
    }
}
