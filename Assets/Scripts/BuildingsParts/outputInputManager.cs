using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class outputInputManager : MonoBehaviour
{
    [SerializeField]
    public int selfID = 0;
    public bool connectedAsRocket = false;
    public GameObject input;
    public GameObject output;

    public GameObject attachedInput;
    public GameObject attachedOutput;
    
    public int inputParentID = 0;
    public GameObject inputParent;

    public int outputParentID = 0;
    public GameObject outputParent;

    public TextMeshProUGUI quantityText;
    public TextMeshProUGUI rateText;

    //Rate is in unit/s
    public float selfRate;
    public float rate;

    public float moles = 0;
    public float volume = 0;
    public float externalTemperature = 298f;
    public float pressure;
    public float internalTemperature;
    public string substance = "none";

    public float substanceDensity; //g/cm3
    public float substanceLiquidTemperature; //K
    public float substanceMolarMass; //g/mol

    public bool log = false;

    public float variation;

    public List<GameObject> engines = new List<GameObject>();

    public string type = "default";

    void Start()
    {
        if(this.GetComponent<buildingType>())
        {
            selfID = this.GetComponent<buildingType>().buildingID;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(inputParentID > 0 && inputParent == null)
        {
            InitializeInput();
        }

        if(outputParentID > 0 && outputParent == null)
        {
            InitializeOutput();
        }

        if(type == "default")
        {
            updateParents();
            setRate();
            fuelTransfer();
        }
    }

    void setProperty(string substance)
    {
        if(substance == "kerosene")
        {
            substanceDensity = 0.81f;
            substanceLiquidTemperature = 298f;
        }

        if(substance == "LOX")
        {
            substanceDensity = 1.141f;
            substanceLiquidTemperature = 80f;
        }
    }
    
    void updateParents()
    {
        if(!inputParent  && attachedInput)
        {
            inputParent = attachedInput.transform.parent.gameObject;
            substance = attachedInput.GetComponent<outputInputManager>().substance;
        }
        
        if(!outputParent && attachedOutput)
        {
            outputParent = attachedOutput.transform.parent.gameObject;
            attachedOutput.GetComponent<outputInputManager>().substance = substance;
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

    void calculateInternalConditions()
    {
        float mass = substanceMolarMass * moles;

        string state = "none";
        if(substanceLiquidTemperature-25 < internalTemperature < substanceLiquidTemperature+25)
        {
            state = "liquid";
        }
        else if((substanceLiquidTemperature + 100) > internalTemperature > (substanceLiquidTemperature + 100))
        {
            state = "gas";
        }

        if(state == "liquid")
        {

        }

        if(state == "gas")
        {
            
        }

    }

    public void InitializeInput()
    {
        GameObject[] Inputs;
        Inputs = GameObject.FindGameObjectsWithTag("building");
        foreach (GameObject input in Inputs)
        {
            if(input.GetComponent<buildingType>().buildingID == inputParentID)
            {
                inputParent = input;

                if(connectedAsRocket)
                {
                    input.GetComponent<launchPadManager>().ConnectedRocket = this.gameObject;
                    input.GetComponent<outputInputManager>().outputParent = this.gameObject; 
                }
            }
        }
    }

    public void InitializeOutput()
    {
        GameObject[] Outputs;
        Outputs = GameObject.FindGameObjectsWithTag("building");
        foreach (GameObject output in Outputs)
        {
            if(output.GetComponent<buildingType>().buildingID == outputParentID)
            {
                outputParent = output;
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
