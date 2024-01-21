using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.Assertions.Must;
using UnityEditor.Localization.Plugins.XLIFF.V12;

public class outputInputManager : MonoBehaviour
{
    [SerializeField]
    public Guid guid;
    public Guid inputGuid;
    public Guid outputGuid;

    public outputInputManager inputParent;

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
    public float tankCoolerPower = 500f;
    public float tankSurfaceArea = 2000f;
    public string tankState = "working";

    public float externalTemperature = 298f;
    public float targetTemperature = 298f;
    //In pascals
    public float externalPressure = 101000f;
    public float internalPressure = 298f;
    public float internalTemperature = 298f;
    public float maxPressure = 4e8f;
    public Substance substance;
    public string state = "none";


    public bool log = false;
    public bool coolerActive = false;
    public bool ventActive = false;

    public float variation;

    public List<GameObject> engines = new List<GameObject>();

    public string type = "default";
    public string circuit = "none";
    public TimeManager MyTime;
    public ParticleSystem explosion;

    void Start()
    {
        Initialize();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Meaning its either a rocket tank alone or a tank in a rocket
        if (GetComponent<Tank>() != null)
        {
            InitializeCircuitTank();
        }

        if (type == "default" && MyTime != null && substance != null)
        {
            fuelLogic();
        }

    }

    void fuelLogic()
    {
        updateParents();
        setRate();
        fuelTransfer();
        calculateInternalConditions();
        vent();
        checkBreak();
    }

    void checkBreak()
    {
        if (internalPressure > maxPressure)
        {
            if (this.gameObject.transform.parent.gameObject.GetComponent<PlanetGravity>() != null)
            {
                GameObject toDestroy = this.gameObject.transform.parent.gameObject;
                Destroy(toDestroy);
            }
            else if (explosion != null)
            {
                explosion.transform.parent = null;
                explosion.Play();
                Destroy(this.gameObject);
            }
            return;
        }

        if (volume > tankVolume)
        {
            if (this.gameObject.transform.parent.gameObject.GetComponent<PlanetGravity>() != null)
            {
                GameObject toDestroy = this.gameObject.transform.parent.gameObject;
                DestroyImmediate(toDestroy);
            }
            else if (explosion != null)
            {
                explosion.transform.parent = null;
                explosion.Play();
                DestroyImmediate(this.gameObject);
            }
            return;
        }
    }

    void Initialize()
    {
        guid = Guid.NewGuid();

        if (MyTime == null)
        {
            MyTime = FindObjectOfType<TimeManager>();
        }

        if (GetComponent<buildingType>())
        {
            internalTemperature = externalTemperature;
            internalPressure = externalPressure;
        }
    }

    void InitializeCircuitTank()
    {
        circuit = GetComponent<Tank>().propellantCategory;
        //Assume tank volume is in m3, convert to liters
        tankVolume = GetComponent<Tank>()._volume * 1000;
        tankSurfaceArea = (GetComponent<Tank>().x_scale / 2) * Mathf.PI * 2 * GetComponent<Tank>().y_scale;
    }

    void updateParents()
    {
        if (inputParent)
        {
            substance = inputParent.substance;
        }

        if (outputParent)
        {
            outputParent.substance = substance;
        }
    }

    void setRate()
    {
        if (inputParent)
        {
            rate = inputParent.rate;
            targetTemperature = inputParent.targetTemperature;
        }

        if (!inputParent && moles != 0)
        {
            rate = selfRate;
        }
    }

    void vent()
    {
        if (ventActive == true)
        {
            state = "gas";
            float molarRate = 5000 / substance.MolarMass;
            variation = molarRate * MyTime.deltaTime;
            if (moles - variation >= 0)
            {
                variation = molarRate * MyTime.deltaTime;
                moles -= variation;
            }
            else
            {
                moles = 0;
                substance = null;
            }
        }
    }

    void fuelTransfer()
    {
        float molarRate = rate / substance.MolarMass;

        if (outputParent && this.GetComponent<launchPadManager>() == null)
        {
            if (moles - molarRate * MyTime.deltaTime >= 0)
            {
                variation = molarRate * MyTime.deltaTime;
                moles -= variation;
            }
        }

        if (inputParent && inputParent.moles - inputParent.variation > 0)
        {
            moles += inputParent.variation;
            substance = inputParent.substance;
        }

        //Logic for rockets
        if (circuit != "none" && GetComponent<launchPadManager>() != null)
        {
            CalculateRocketTankVariation(molarRate);
        }

        //Logic for engines static fire
        if (circuit != "none" && GetComponent<staticFireStandManager>() != null && GetComponent<staticFireStandManager>().ConnectedEngine != null)
        {
            staticFireStandManager sFSM = GetComponent<staticFireStandManager>();
            CalculateFlowStaticFireEngine(sFSM.ConnectedEngine.GetComponent<Engine>()._rate, sFSM.started, sFSM.ratio, sFSM);
        }

    }

    //Manage flow of fuel for static fire stand
    private void CalculateFlowStaticFireEngine(float massFlowRateEngine, bool started, float ratio, staticFireStandManager sFSM)
    {
        //Ratio is always oxidizer/fuel
        if (started == true)
        {
            if (circuit == "oxidizer")
            {
                float percentageOxidizer = ratio / (ratio + 1);
                float rate = percentageOxidizer * massFlowRateEngine;
                //Static fire will be able to be ran at timewarp
                float consumedOxidizer = rate * MyTime.deltaTime;
                if (mass - consumedOxidizer >= 0)
                {
                    sFSM.oxidizerSufficient = true;
                    //Multiply by 1000 bcs engine rate is kg
                    float consumedMoles = consumedOxidizer * 1000 / substance.MolarMass;
                    moles -= consumedMoles;
                    return;
                }
                else
                {
                    sFSM.oxidizerSufficient = false;
                    return;
                }
            }

            if (circuit == "fuel")
            {
                float percentageFuel = 1f / (ratio + 1);
                float rate = percentageFuel * massFlowRateEngine;
                //Static fire will be able to be ran at timewarp
                float consumedFuel = rate * MyTime.deltaTime;
                if (mass - consumedFuel >= 0)
                {
                    sFSM.fuelSufficient = true;
                    float consumedMoles = consumedFuel * 1000 / substance.MolarMass;
                    moles -= consumedMoles;
                    return;
                }
                else
                {
                    sFSM.fuelSufficient = false;
                    return;
                }
            }
        }
    }

    public void updateExternalTemp()
    {
        if (this.gameObject.transform.parent != null)
        {
            if (this.gameObject.transform.parent.gameObject.GetComponent<PlanetGravity>() != null)
            {
                PlanetGravity PG = this.gameObject.transform.parent.gameObject.GetComponent<PlanetGravity>();
                if ((this.gameObject.transform.position - PG.getPlanet().transform.position).magnitude > PG.getPlanetRadius() + PG.getAtmoAlt())
                {
                    externalTemperature = internalTemperature;
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
        if (inputParent != null)
        {
            tank.GetComponent<outputInputManager>().internalTemperature = inputParent.internalTemperature;
            tank.GetComponent<outputInputManager>().externalTemperature = inputParent.externalTemperature;
            tank.GetComponent<outputInputManager>().moles += (float)molesToGive;
            tank.GetComponent<outputInputManager>().substance = substance;
        }
    }

    void calculateInternalConditions()
    {

        SetState();
        CalculateConditionsFromState();
        updateExternalTemp();
    }

    private void CalculateConditionsFromState()
    {
        if (state == "liquid")
        {
            ConvertMass();
            volume = mass / substance.Density;
            float ratio = volume / tankVolume;
            float heightLiquid = ratio * tankHeight;
            internalPressure = substance.Density * 9.8f * heightLiquid;

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

            volume = mass / substance.Density;

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
        float temp = 0;
        float tankLocalThermalConductivity = 0;
        if (coolerActive == true)
        {
            temp = targetTemperature;
            tankLocalThermalConductivity = tankCoolerPower;
        }

        if (coolerActive == false)
        {
            temp = externalTemperature;
            tankLocalThermalConductivity = tankThermalConductivity;
        }


        //Calculate T (might not work if internal is higher than external or reverse)
        float Q_cond = 0;
        if (temp < internalTemperature)
        {
            Q_cond = (tankLocalThermalConductivity * tankSurfaceArea * (temp - internalTemperature)) / tankThickness;
        }

        if (temp > internalTemperature)
        {
            Q_cond = -(tankLocalThermalConductivity * tankSurfaceArea * (internalTemperature - temp)) / tankThickness;
        }


        float deltaInternal = (Q_cond * Time.deltaTime) / (mass * substance.SpecificHeatCapacity);
        if (internalTemperature != temp)
        {
            internalTemperature += deltaInternal;
        }
    }

    void ConvertMass()
    {
        //Convert moles to mass
        mass = moles * substance.MolarMass;
        //Convert g to kg
        mass /= 1000;
    }

    private void SetState()
    {
        if (substance.SolidTemperature < internalTemperature && internalTemperature < substance.GaseousTemperature)
        {
            state = "liquid";
        }
        else if (internalTemperature > substance.GaseousTemperature)
        {
            state = "gas";
        }
        else if (internalTemperature < substance.SolidTemperature)
        {
            state = "solid";
        }
    }
}
