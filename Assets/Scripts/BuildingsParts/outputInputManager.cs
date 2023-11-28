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
    public float mass = 0;

    public float tankVolume = 0;
    public float tankHeight = 0;
    public float tankThickness = 0.1f;
    public float tankThermalConductivity = 10f;
    public float tankSurfaceArea = 2000f;

    public float externalTemperature = 298f;
    public float externalPressure = 101f;
    public float internalPressure;
    public float internalTemperature;
    public string substance = "none";

    public float substanceDensity; //kg/m3
    public float substanceLiquidTemperature; //K
    public float substanceSolidTemperature; //K
    public float substanceGaseousTemperature; //K
    public float substanceMolarMass; //g/mol
    public float substanceSpecificHeatCapacity; //J/kgK

    public bool log = false;

    public float variation;

    public List<GameObject> engines = new List<GameObject>();

    public string type = "default";

    void Start()
    {
        if(this.GetComponent<buildingType>())
        {
            selfID = this.GetComponent<buildingType>().buildingID;
            internalTemperature = externalTemperature;
            internalPressure = externalPressure;
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
            substanceDensity = 810f;
            substanceLiquidTemperature = 226f; //up to 424
            substanceGaseousTemperature = 424f; //and more
            substanceSolidTemperature = 226f; //and below
            substanceMolarMass = 170f;
            substanceSpecificHeatCapacity = 2010f;
        }

        if(substance == "LOX")
        {
            substanceDensity = 1141f;
            substanceLiquidTemperature = 56f; //up to 91
            substanceGaseousTemperature = 91f; //and more
            substanceSolidTemperature = 56f; //and below
            substanceMolarMass = 32f;
            substanceSpecificHeatCapacity = 2010f;
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
        if(substanceSolidTemperature < internalTemperature && internalTemperature < substanceGaseousTemperature)
        {
            state = "liquid";
        }
        else if(internalTemperature > substanceGaseousTemperature)
        {
            state = "gas";
        }
        else if(internalTemperature < substanceSolidTemperature)
        {
            state = "solid";
        }

        if(state == "liquid")
        {
            //Convert moles to mass
            mass = moles*substanceMolarMass;
            volume = mass/substanceDensity;
            float ratio = volume/tankVolume;
            float heightLiquid = ratio*tankHeight;
            internalPressure = substanceDensity  * 9.8f * heightLiquid;

            //Calculate T
            float Q_cond = (tankThermalConductivity * tankSurfaceArea * (internalTemperature - externalTemperature)) / tankThickness;
            float deltaInternal = (Q_cond * Time.deltaTime) / (mass * substanceSpecificHeatCapacity);
            internalTemperature += deltaInternal;
        }

        if(state == "gas")
        {
            //TODO
        }

        if(state == "solid")
        {
            //TODO
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
