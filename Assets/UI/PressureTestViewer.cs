using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PressureTestViewer : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject Stand;
    public TMP_Text status;
    public TMP_Text quantity;
    public TMP_Text volume;
    public TMP_Text temperature;
    public Toggle cooler;
    public Toggle vent;
    public Toggle valve;
    public TMP_InputField targetTemp;

    public bool previouslyRan = false;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Stand != null)
        {
            updateStatus();
            if(Stand.GetComponent<standManager>().ConnectedTank != null)
            {
                updateQuantity();
                updateTemp();
                if(targetTemp.text == "")
                {
                    targetTemp.text = Stand.GetComponent<standManager>().ConnectedTank.GetComponent<cooler>().targetTemperature.ToString();
                }
            }
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
        if(Stand.GetComponent<standManager>().ConnectedTank.GetComponent<TankComponent>().tested == false)
        {
            quantity.text = Stand.GetComponent<standManager>().ConnectedTank.GetComponent<container>().mass.ToString();
        }else if(Stand.GetComponent<standManager>().ConnectedTank.GetComponent<TankComponent>().tested == true)
        {
            if(Stand.GetComponent<standManager>().ConnectedTank.GetComponent<container>().substance != null)
            {
                //mass = volume * density
                float maxQuantity = Stand.GetComponent<standManager>().ConnectedTank.GetComponent<container>().tankVolume * Stand.GetComponent<standManager>().ConnectedTank.GetComponent<container>().substance.Density; 
                quantity.text = Stand.GetComponent<standManager>().ConnectedTank.GetComponent<container>().mass.ToString() + "/" + maxQuantity.ToString();
            }else{
                quantity.text = Stand.GetComponent<standManager>().ConnectedTank.GetComponent<container>().mass.ToString();   
            }
        }
        
        volume.text = Stand.GetComponent<standManager>().ConnectedTank.GetComponent<container>().volume.ToString() + "/" + Stand.GetComponent<standManager>().ConnectedTank.GetComponent<container>().tankVolume.ToString();
        temperature.text = Stand.GetComponent<standManager>().ConnectedTank.GetComponent<container>().internalTemperature.ToString();
    }

    public void startTest()
    {
        Stand.GetComponent<standManager>().started = true;
        Stand.GetComponent<standManager>().ConnectedTank.GetComponent<flowController>().opened = true;
        previouslyRan = true;
        MasterManager masterManager = FindObjectOfType<MasterManager>();
        masterManager.GetComponent<pointManager>().nPoints += 2f;
    }

    public void stopTest()
    {
        if(Stand.GetComponent<standManager>().ConnectedTank != null)
        {
            Stand.GetComponent<standManager>().ConnectedTank.GetComponent<flowController>().opened = false;
        }
        Stand.GetComponent<standManager>().failed = true;
    }

    public void openValve()
    {
        if(Stand.GetComponent<standManager>().ConnectedTank.GetComponent<flowController>().opened == true)
        {
            Stand.GetComponent<standManager>().ConnectedTank.GetComponent<flowController>().opened = false;
            return;
        }
        
        if(Stand.GetComponent<standManager>().ConnectedTank.GetComponent<flowController>().opened == false)
        {
            Stand.GetComponent<standManager>().ConnectedTank.GetComponent<flowController>().opened = true;
            return;
        }
    }

    public void activateCooler()
    {
        if(Stand.GetComponent<standManager>().ConnectedTank.GetComponent<cooler>().active == true)
        {
            Stand.GetComponent<standManager>().ConnectedTank.GetComponent<cooler>().active = false;
            return;
        }
        
        if(Stand.GetComponent<standManager>().ConnectedTank.GetComponent<cooler>().active == false)
        {
            Stand.GetComponent<standManager>().ConnectedTank.GetComponent<cooler>().active = true;
            return;
        }
    }

    public void updateTemp()
    {
        float tryN = 0;
        if (float.TryParse(targetTemp.text.ToString(), out tryN))
        {
            if(tryN < 0)
            {
                Stand.GetComponent<standManager>().ConnectedTank.GetComponent<cooler>().targetTemperature = 0;
                return;
            }else{
                Stand.GetComponent<standManager>().ConnectedTank.GetComponent<cooler>().targetTemperature = tryN;
                return;
            }
        }
    } 

    public void openVent()
    {
        if(Stand.GetComponent<standManager>().ConnectedTank.GetComponent<gasVent>().open == true)
        {
            Stand.GetComponent<standManager>().ConnectedTank.GetComponent<gasVent>().open = false;
            return;
        }
        
        if(Stand.GetComponent<standManager>().ConnectedTank.GetComponent<gasVent>().open == false)
        {
            Stand.GetComponent<standManager>().ConnectedTank.GetComponent<gasVent>().open = true;
            return;
        }
    }       



    public void Terminate()
    {
        Stand.GetComponent<standManager>().failed = false;
        Stand.GetComponent<standManager>().started = false;
        previouslyRan = false;
        Stand.GetComponent<standManager>().tankStatusTracker = null;
        Destroy(Stand.GetComponent<standManager>().ConnectedTank);
        this.gameObject.SetActive(false);
        Stand.GetComponent<standManager>().ConnectedTank = null;
        Stand.GetComponent<flowControllerForTankStand>().connected = false;
    }
}
