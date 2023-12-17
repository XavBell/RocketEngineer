using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class outputInputManager : MonoBehaviour
{
    [SerializeField]
    substanceProperty substanceProperty = new substanceProperty();
    public Guid guid;
    public Guid inputGuid;
    public Guid outputGuid;
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
    public TimeManager MyTime;

    void Start()
    {
        Initialize();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(inputParentID > 0 && inputParent == null)
        {
            //InitializeInput();
        }

        if(outputParentID > 0 && outputParent == null)
        {
            //InitializeOutput();
        }

        if(type == "default" && MyTime != null)
        {
            updateParents();
            setRate();
            fuelTransfer();
        }

        if(GetComponent<Tank>() != null)
        {
            InitializeCircuitTank();
        }

        
    }

    void Initialize()
    {
        guid = Guid.NewGuid();

        if(MyTime == null)
        {
            MyTime= FindObjectOfType<TimeManager>();
        }

        if(GetComponent<buildingType>())
        {
            selfID = GetComponent<buildingType>().buildingID;
            internalTemperature = externalTemperature;
            internalPressure = externalPressure;
        }
    }

    void InitializeCircuitTank()
    {
        circuit = GetComponent<Tank>().propellantCategory;
        tankVolume = GetComponent<Tank>()._volume;
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
        float molarRate = rate/substanceMolarMass;
        if(GetComponent<standManager>() != null)
        {
            selfRate = rate;
        }
        
        if(outputParent && this.GetComponent<launchPadManager>() == null)
        {
            if(moles -  molarRate * MyTime.deltaTime >= 0)
            {
                variation = molarRate * MyTime.deltaTime;
                moles -=  variation;
            }
        }

        if(inputParent && inputParent.moles - inputParent.variation > 0)
        {
            moles += inputParent.variation;
            substance = inputParent.substance;
        }

        //Logic for rockets
        if(circuit != "none" && GetComponent<launchPadManager>() != null)
        {
            CalculateRocketTankVariation(molarRate);
        }

        //Logic for engines static fire
        if(circuit != "none" && GetComponent<staticFireStandManager>() != null && GetComponent<staticFireStandManager>().ConnectedEngine != null)
        {
            staticFireStandManager sFSM = GetComponent<staticFireStandManager>();
            CalculateFlowStaticFireEngine(sFSM.ConnectedEngine.GetComponent<Engine>()._rate, sFSM.started, sFSM.ratio, sFSM);
        }

        calculateInternalConditions();
        
    }

    private void CalculateFlowStaticFireEngine(float massFlowRateEngine, bool started, float ratio, staticFireStandManager sFSM)
    {
        //Ratio is always oxidizer/fuel
        if(started == true)
        {
            if(circuit == "oxidizer")
            {
                float percentageOxidizer = ratio/(ratio + 1);
                float rate = percentageOxidizer * massFlowRateEngine;
                //Static fire will be able to be ran at timewarp
                float consumedOxidizer = rate * MyTime.deltaTime;
                if(mass - consumedOxidizer >= 0)
                {
                    sFSM.oxidizerSufficient = true;
                    //Multiply by 1000 bcs engine rate is kg
                    float consumedMoles = consumedOxidizer*1000/substanceMolarMass;
                    moles -= consumedMoles;
                    return;
                }else{
                    sFSM.oxidizerSufficient = false;
                    return;
                }
            }

            if(circuit == "fuel")
            {
                float percentageFuel = 1f/(ratio + 1);
                float rate = percentageFuel * massFlowRateEngine;
                //Static fire will be able to be ran at timewarp
                float consumedFuel = rate * MyTime.deltaTime;
                if(mass - consumedFuel >= 0)
                {
                    sFSM.fuelSufficient = true;
                    float consumedMoles = consumedFuel*1000/substanceMolarMass;
                    moles -= consumedMoles;
                    return;
                }else{
                    sFSM.fuelSufficient = false;
                    return;
                }
            }
        }
    }

    private void CalculateRocketTankVariation(float molarRate)
    {
        launchPadManager launchPad = this.GetComponent<launchPadManager>();

        if (launchPad.ConnectedRocket != null)
        {
            Rocket rocket = launchPad.ConnectedRocket.GetComponent<Rocket>();
            //Rate is in kg/s, we want to get the rate in mol/s
            //m = nM
            List<RocketPart> tanks = new List<RocketPart>();
            GetFuelTanksPerLine(rocket, tanks, circuit);
            DivideFuel(molarRate, tanks);

            launchPad.ConnectedRocket.GetComponent<Rocket>().updateMass();
        }
    }

    private void DivideFuel(float molarRate, List<RocketPart> tanks)
    {
        if (tanks.Count != 0 && moles - (molarRate * MyTime.deltaTime) >= 0)
        {
            double molesToGive = molarRate * MyTime.deltaTime / tanks.Count;
            foreach (RocketPart tank in tanks)
            {
                SetTankConditions(molesToGive, tank);
            }
            moles -= molarRate * MyTime.deltaTime;
        }
    }

    private static void GetFuelTanksPerLine(Rocket rocket, List<RocketPart> tanksFuel, string fuelType)
    {
        foreach (Stages stage in rocket.Stages)
        {
            foreach (RocketPart part in stage.Parts)
            {
                if (part._partType == "tank")
                {
                    if (part.GetComponent<outputInputManager>().circuit == fuelType)
                    {
                        tanksFuel.Add(part);
                    }
                }
            }
        }
    }

    private void SetTankConditions(double molesToGive, RocketPart tank)
    {
        if(inputParent != null)
        {
            tank.GetComponent<outputInputManager>().internalTemperature = inputParent.internalTemperature;
            tank.GetComponent<outputInputManager>().externalTemperature = inputParent.externalTemperature;
            tank.GetComponent<outputInputManager>().moles += (float)molesToGive;
            tank.GetComponent<outputInputManager>().substance = substance;
        }
    }

    void calculateInternalConditions()
    {
        substanceProperty.AssignProperty(substance, out substanceDensity, out substanceLiquidTemperature, out substanceGaseousTemperature, out substanceSolidTemperature, out substanceMolarMass, out substanceSpecificHeatCapacity);
        SetState();
        CalculateConditionsFromState();
    }

    private void CalculateConditionsFromState()
    {
        if (state == "liquid")
        {
            ConvertMass();
            volume = mass / substanceDensity;
            float ratio = volume / tankVolume;
            float heightLiquid = ratio * tankHeight;
            internalPressure = substanceDensity * 9.8f * heightLiquid;

            if (internalPressure == float.NaN)
            {
                internalPressure = 0;
            }

            if (tankVolume < volume)
            {
                //Pressure is critical, tank should break
                tankState = "broken";
            }

            CalculateTemperature();

        }

        if (state == "gas")
        {
            ConvertMass();
            CalculateTemperature();

            internalPressure = (moles * 8.314f * internalTemperature) / tankVolume; //Not sure about 8.314
        }

        if (state == "solid")
        {
            ConvertMass();

            volume = mass / substanceDensity;

            if (tankVolume < volume)
            {
                //Pressure is critical, tank should break, set pressure
                tankState = "broken";
            }

            CalculateTemperature();
        }

    }

    void CalculateTemperature()
    {
        //Calculate T (might not work if internal is higher than external or reverse)
        float Q_cond = (tankThermalConductivity * tankSurfaceArea * (externalTemperature - internalTemperature)) / tankThickness;
        float deltaInternal = (Q_cond * Time.deltaTime) / (mass * substanceSpecificHeatCapacity);
        if (internalTemperature != externalTemperature)
        {
            internalTemperature += deltaInternal;
        }
    }

    void ConvertMass()
    {
        //Convert moles to mass
        mass = moles * substanceMolarMass;
        //Convert g to kg
        mass /= 1000;
    }

    private void SetState()
    {
        if (substanceSolidTemperature < internalTemperature && internalTemperature < substanceGaseousTemperature)
        {
            state = "liquid";
        }
        else if (internalTemperature > substanceGaseousTemperature)
        {
            state = "gas";
        }
        else if (internalTemperature < substanceSolidTemperature)
        {
            state = "solid";
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
