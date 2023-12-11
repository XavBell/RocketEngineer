using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class outputInputManager : MonoBehaviour
{
    [SerializeField]
    public int selfID = 0;
    public bool connectedAsRocket = false;
    
    public int inputParentID = 0;
    public outputInputManager inputParent;

    public int outputParentID = 0;
    public outputInputManager outputParent;

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
    public string tankState = "working";

    public float externalTemperature = 298f;
    public float externalPressure = 101f;
    public float internalPressure = 0f;
    public float internalTemperature = 0f;
    public string substance = "none";
    public string state = "none";

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
    public string circuit = "none";

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

        if(this.GetComponent<Tank>() != null)
        {
            InitializeCircuitTank();
        }

        
    }

    void InitializeCircuitTank()
    {
        circuit = this.GetComponent<Tank>().propellantCategory;
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
        if(inputParent)
        {
            substance = inputParent.substance;
        }
        
        if(outputParent)
        {
            outputParent.substance = substance;
        }
    }

    void setRate()
    {
        if(inputParent)
        {

            rate = inputParent.rate;
        }

        if(!inputParent && moles != 0)
        {
            rate = selfRate;
        }
    }

    void fuelTransfer()
    {

        if(outputParent && this.GetComponent<launchPadManager>() == null)
        {
            if(moles - rate * Time.fixedDeltaTime >= 0)
            {
                variation = rate * Time.fixedDeltaTime;
                moles -=  variation;
            }
        }

        if(inputParent && inputParent.moles - inputParent.variation > 0)
        {
            moles += inputParent.variation;
            substance = inputParent.substance;
        }

        //Logic for rockets
        if(circuit != "none" && this.GetComponent<launchPadManager>() != null)
        {
            launchPadManager launchPad = this.GetComponent<launchPadManager>();
            if(launchPad.ConnectedRocket != null)
            {
                Rocket rocket = launchPad.ConnectedRocket.GetComponent<Rocket>();
                if(circuit == "oxidizer")
                {
                    List<RocketPart> tanksOxidizer = new List<RocketPart>();
                    foreach(Stages stage in rocket.Stages)
                    {
                        foreach(RocketPart part in stage.Parts)
                        {
                            if(part._partType == "tank")
                            {
                                if(part.GetComponent<outputInputManager>().circuit == "oxidizer")
                                {
                                    tanksOxidizer.Add(part);
                                }
                            }
                        }
                    }

                    if(moles - rate * Time.fixedDeltaTime >= 0 && tanksOxidizer.Count != 0)
                    {
                        double molesToGive = (moles - rate*Time.fixedDeltaTime)/tanksOxidizer.Count;
                        foreach(RocketPart tank in tanksOxidizer)
                        {
                            tank.GetComponent<outputInputManager>().moles += (float)molesToGive;
                        }
                    }

                    
                }

                if(circuit == "fuel")
                {
                    List<RocketPart> tanksFuel = new List<RocketPart>();
                    foreach(Stages stage in rocket.Stages)
                    {
                        foreach(RocketPart part in stage.Parts)
                        {
                            if(part._partType == "tank")
                            {
                                if(part.GetComponent<outputInputManager>().circuit == "fuel")
                                {
                                    tanksFuel.Add(part);
                                }
                            }
                        }
                    }

                    if(tanksFuel.Count != 0 && (moles - rate*Time.fixedDeltaTime) >= 0)
                    {
                        double molesToGive = moles/tanksFuel.Count;
                        foreach(RocketPart tank in tanksFuel)
                        {
                            tank.GetComponent<outputInputManager>().moles += (float)molesToGive;
                            tank.GetComponent<outputInputManager>().substance = substance;
                        }
                        moles -= rate * Time.fixedDeltaTime;
                    }
                }

                launchPad.ConnectedRocket.GetComponent<Rocket>().updateMass();
            }
        }

        calculateInternalConditions();
        
    }

    void calculateInternalConditions()
    {
        setProperty(substance);

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

            if(tankVolume < volume)
            {
                //Pressure is critical, tank should break
                tankState = "broken";
            }

            //Calculate T (might not work if internal is higher than external or reverse)
            float Q_cond = (tankThermalConductivity * tankSurfaceArea * (externalTemperature - internalTemperature)) / tankThickness;
            float deltaInternal = (Q_cond * Time.deltaTime) / (mass * substanceSpecificHeatCapacity);
            if(internalTemperature != externalTemperature)
            {
                internalTemperature += deltaInternal;
                
            }
            
        }

        if(state == "gas")
        {
            //Convert moles to mass
            mass = moles*substanceMolarMass;

            //Calculate T (might not work if internal is higher than external or reverse)
            float Q_cond = (tankThermalConductivity * tankSurfaceArea * (externalTemperature - internalTemperature)) / tankThickness;
            float deltaInternal = (Q_cond * Time.deltaTime) / (mass * substanceSpecificHeatCapacity);
            if(internalTemperature != externalTemperature)
            {
                internalTemperature += deltaInternal;
                
            }

            internalPressure = (moles*8.314f*internalTemperature)/tankVolume; //Not sure about 8.314
        }

        if(state == "solid")
        {
            //Convert moles to mass
            mass = moles*substanceMolarMass;
            volume = mass/substanceDensity;

            if(tankVolume < volume)
            {
                //Pressure is critical, tank should break, set pressure
                tankState = "broken";

            }

            //Calculate T (might not work if internal is higher than external or reverse)
            float Q_cond = (tankThermalConductivity * tankSurfaceArea * (externalTemperature - internalTemperature)) / tankThickness;
            float deltaInternal = (Q_cond * Time.deltaTime) / (mass * substanceSpecificHeatCapacity);
            UnityEngine.Debug.Log(mass);
            if(internalTemperature != externalTemperature)
            {
                internalTemperature += deltaInternal;
            }
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
                //inputParent = input;
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
                //outputParent = output;
            }
        }
    }
}
