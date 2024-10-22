using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class container : MonoBehaviour
{
    public Guid guid;
    public string type = "oxidizer";
    public float tankVolume; //m3 container volume
    public float tankHeight; //container height in meter
    public float tankThermalConductivity; //Constant
    public float tankSurfaceArea;
    public float tankThickness;
    public float maxPressure;


    public float moles = 0;
    public float internalPressure; //Pa
    public float internalTemperature = 298; //K
    public float externalTemperature = 298; //K
    public bool coolerActive = false;
    public float targetTemperature = 0;
    public float volume; //m3 volume of substance
    public string state = "none"; //state of substance
    public float mass; //mass in kg of the substance
    public bool tested = false; //If container is tested, flow will stop

    public flowController flowController;

    public Substance substance; //substance in the container
    public ParticleSystem explosion;

    public TimeManager MyTime;

    // Start is called before the first frame update
    void Start()
    {
        MyTime = FindObjectOfType<TimeManager>();
        if(guid == Guid.Empty)
        {
            guid = Guid.NewGuid();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(substance != null && moles > 0)
        {
            calculateInternalConditions();
            checkBreak();
        }else if(substance == null)
        {
            state = "none";
            internalPressure = 0;
            internalTemperature = 298;
        }
        
    }

    void calculateInternalConditions()
    {

        SetState();
        CalculateConditionsFromState();
    }

    private void CalculateConditionsFromState()
    {
        if (state == "liquid")
        {
            ConvertMass();
            volume = mass / substance.Density;
            float ratio = volume / tankVolume;
            float heightLiquid = ratio * tankHeight;
            internalPressure =0;

            if (internalPressure == float.NaN)
            {
                internalPressure = 0;
            }

            CalculateTemperature();

        }

        if (state == "gas")
        {
            ConvertMass();
            CalculateTemperature();

            internalPressure = (moles * 8.314f * internalTemperature) / (tankVolume*1000); //Not sure about 8.314
        }

        if (state == "solid")
        {
            ConvertMass();
            volume = mass / substance.Density;
            CalculateTemperature();
        }

    }

    void CalculateTemperature()
    {
        //Calculate T (might not work if internal is higher than external or reverse)
        float temp = externalTemperature;
        if(coolerActive == true)
        {
            temp = targetTemperature;
        }

        float Q_cond = 0;
        if (temp > internalTemperature)
        {
            Q_cond = tankThermalConductivity * tankSurfaceArea * (temp - internalTemperature) / tankThickness;
        }

        if (temp < internalTemperature)
        {
            Q_cond = -(tankThermalConductivity * tankSurfaceArea * (internalTemperature - temp)) / tankThickness;
        }

        float deltaInternal = Q_cond * MyTime.deltaTime / (mass * substance.SpecificHeatCapacity);
        internalTemperature += deltaInternal;
        
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

    void checkBreak()
    {
        if (internalPressure > maxPressure)
        {
            if (this.gameObject.transform.parent.gameObject.GetComponent<PlanetGravity>() != null)
            {
                GameObject toDestroy = this.gameObject.transform.parent.gameObject;
                Destroy(toDestroy);
                FindObjectOfType<DestroyPopUpManager>().ShowDestroyPopUp("Destroyed due to overpressure");
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
            if(this.GetComponentInParent<standManager>() != null)
            {
                this.GetComponentInParent<standManager>().logData();
            }
            
            if (this.gameObject.transform.parent.gameObject.GetComponent<PlanetGravity>() != null)
            {
                GameObject toDestroy = this.gameObject.transform.parent.gameObject;
                FindObjectOfType<DestroyPopUpManager>().ShowDestroyPopUp("Destroyed due to overfilling");
                Destroy(toDestroy);
            }
            else if (explosion != null)
            {
                FindObjectOfType<DestroyPopUpManager>().ShowDestroyPopUp("Destroyed due to overfilling");
                explosion.transform.parent = null;
                explosion.Play();
                Destroy(this.gameObject);
            }
            return;
        }
    }
}
