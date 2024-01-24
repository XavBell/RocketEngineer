using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class container : MonoBehaviour
{
    public Guid guid;
    public float tankVolume; //m3 container volume
    public float tankHeight; //container height in meter
    public float tankThermalConductivity; //Constant
    public float tankSurfaceArea;
    public float tankThickness;
    public float maxPressure;


    public float moles;
    public float internalPressure; //Pa
    public float internalTemperature; //K
    public float externalTemperature; //K
    public float volume; //m3 volume of substance
    public string state; //state of substance
    public float mass; //mass in kg of the substance

    public Substance substance; //substance in the container
    public ParticleSystem explosion;

    public TimeManager MyTime;

    // Start is called before the first frame update
    void Start()
    {
        MyTime = FindObjectOfType<TimeManager>();
        if(guid == null)
        {
            guid = Guid.NewGuid();
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        calculateInternalConditions();
        checkBreak();
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
            internalPressure = substance.Density * 9.8f * heightLiquid;

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

            internalPressure = (moles * 8.314f * internalTemperature) / tankVolume; //Not sure about 8.314
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
        float Q_cond = 0;
        if (externalTemperature < internalTemperature)
        {
            Q_cond = (tankThermalConductivity * tankSurfaceArea * (internalTemperature - externalTemperature)) / tankThickness;
        }

        if (externalTemperature > internalTemperature)
        {
            Q_cond = -(tankThermalConductivity * tankSurfaceArea * (externalTemperature - internalTemperature)) / tankThickness;
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
}
