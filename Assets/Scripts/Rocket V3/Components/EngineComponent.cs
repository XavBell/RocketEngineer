using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.PropertyVariants.TrackedProperties;

[RequireComponent(typeof(FuelConsumerComponent))]
public class EngineComponent : MonoBehaviour
{
    public FuelConsumerComponent fuelConsumerComponent;
    TimeManager timeManager;

    public float maxThrust;
    public float maxFuelFlow;
    public float reliability;
    public bool operational = true;
    public bool willFail = false;
    public float timeOfFail;
    public bool willExplode = false;
    public float maxTime;
    public bool active;
    public Propellants usedPropellant;
    public bool initialized = false;
    public string _nozzleName;
    public string _pumpName;
    public string _turbineName;

    // Start is called before the first frame update
    void Start()
    {
        timeManager = FindObjectOfType<TimeManager>();
        InitializeFail();
    }

    public Vector2 produceThrust(float throttle)
    {
        if(initialized == false && active == true && throttle > 0)
        {
            InitializeFail();
            initialized = true;
        }

        checkFail();
        
        if(active == false || operational == false)
        {
            return Vector2.zero;
        }

        float qty = maxFuelFlow * throttle;
        if(fuelConsumerComponent.propellantSufficient(qty, usedPropellant))
        {
            fuelConsumerComponent.ConsumeFuel(qty, usedPropellant);
            //Debug.Log("Producing thrust" + maxThrust * throttle * timeManager.deltaTime * gameObject.transform.up);
            return maxThrust * throttle * timeManager.deltaTime * gameObject.transform.up;
        }else
        {
            return Vector2.zero;
        }
    }

    public void checkFail()
    {
        if(willFail == true && (float)timeManager.time > timeOfFail)
        {
            operational = false;
            if(willExplode == true)
            {
                Destroy(gameObject);
            }
        }
    }

    public void InitializeFail()
    {
        float percentageOfThrust = Random.Range(reliability, 2-reliability);
        float outThrust = maxThrust * percentageOfThrust;
        float tMin = maxThrust * 0.8f;
        float tMax = maxThrust * 1.2f;
        if(outThrust < tMin || outThrust > tMax)
        {
            willFail = true;
            if(percentageOfThrust < 0.5f || percentageOfThrust > 1.5f)
            {
                willExplode = true;
            }
            timeOfFail = maxTime + (float)timeManager.time;
            if(maxTime < 2)
            {
                timeOfFail += 2;
            }
        }else{
            willFail = false;
        }
    }

    public void InitializeSprite()
    {
        Nozzle nozzle = Resources.Load<Nozzle>("Engine/Nozzles/" + _nozzleName);
        Pump pump = Resources.Load<Pump>("Engine/Pumps/" + _pumpName);
        Turbine turbine = Resources.Load<Turbine>("Engine/Turbines/" + _turbineName);

        this.GetComponentInChildren<autoSpritePositionner>().nozzle.GetComponent<SpriteRenderer>().sprite = nozzle.sprite;
        this.GetComponentInChildren<autoSpritePositionner>().pump.GetComponent<SpriteRenderer>().sprite = pump.sprite;
        this.GetComponentInChildren<autoSpritePositionner>().turbine.GetComponent<SpriteRenderer>().sprite = turbine.sprite;
    }
}
