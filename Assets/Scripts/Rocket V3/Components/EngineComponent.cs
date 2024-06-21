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
    public bool active;
    public Propellants usedPropellant;

    public string _nozzleName;
    public string _pumpName;
    public string _turbineName;

    // Start is called before the first frame update
    void Start()
    {
        timeManager = FindObjectOfType<TimeManager>();
    }

    public Vector2 produceThrust(float throttle)
    {
        if(active == false)
        {
            print("not active");
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
