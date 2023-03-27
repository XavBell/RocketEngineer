using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class outputInputManager : MonoBehaviour
{
    public GameObject input;
    public GameObject output;

    public GameObject attachedInput;
    public GameObject attachedOutput;

    public GameObject inputParent;
    public GameObject outputParent;

    public TextMeshProUGUI quantityText;
    public TextMeshProUGUI rateText;

    //Rate is in unit/s
    public float selfRate;
    public float rate;

    public float moles = 0;
    public float volume = 0;

    public bool log = false;

    public float variation;

    public List<GameObject> engines = new List<GameObject>();

    public string type = "default";

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(type == "default")
        {
            updateParents();
            setRate();
            fuelTransfer();
            if(quantityText != null && log == true){
                quantityText.enabled = true;
                rateText.enabled = true;
                quantityText.text = "Quantity: " + moles.ToString();
                rateText.text= "Rate: " + rate.ToString();
            }else if(quantityText != null && log == false)
            {
                quantityText.enabled = false;
                rateText.enabled = false;
            }
        }

        DebugLog();
    }

    void FixedUpdate()
    {
        fuelTransfer();
    }
    
    void updateParents()
    {
        if(!inputParent  && attachedInput)
        {
            inputParent = attachedInput.transform.parent.gameObject;
        }
        
        if(!outputParent && attachedOutput)
        {
            outputParent = attachedOutput.transform.parent.gameObject;
        }
    }

    void setRate()
    {
        if(inputParent)
        {

            rate = inputParent.GetComponent<outputInputManager>().rate;
        }

        if(!inputParent && moles != 0)
        {
            rate = selfRate;
        }
    }

    void fuelTransfer()
    {

        if(outputParent)
        {
           if(moles - rate * Time.deltaTime >= 0 && outputParent.GetComponent<outputInputManager>().moles + outputParent.GetComponent<outputInputManager>().rate*Time.deltaTime < outputParent.GetComponent<outputInputManager>().volume)
           {
                variation = rate * Time.deltaTime;
                moles -=  variation;
           }
        }

        if(inputParent && inputParent.GetComponent<outputInputManager>().moles - inputParent.GetComponent<outputInputManager>().variation > 0 && moles + inputParent.GetComponent<outputInputManager>().variation < volume)
        {
            moles += inputParent.GetComponent<outputInputManager>().variation;

            if(engines.Count > 0)
            {
                float newVolume = moles/engines.Count;
                foreach(GameObject en in engines)
                {
                    en.GetComponent<Part>().fuel = newVolume;
                    this.GetComponent<PlanetGravity>().rocketMass += inputParent.GetComponent<outputInputManager>().variation;
                }
            }
        }
    }

    void DebugLog()
    {
        if(log == true)
        {
            //Debug.Log(moles);
        }
    }
}
