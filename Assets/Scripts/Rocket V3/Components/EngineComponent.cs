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
    public bool active;
    public Propellants usedPropellant;

    // Start is called before the first frame update
    void Start()
    {
        FindObjectOfType<TimeManager>();
    }

    public Vector2 produceThrust(float throttle)
    {
        float qty = maxFuelFlow * throttle;
        if(fuelConsumerComponent.propellantSufficient(qty, usedPropellant))
        {
            fuelConsumerComponent.ConsumeFuel(qty, usedPropellant);
            return maxThrust * throttle * timeManager.deltaTime * gameObject.transform.up;
        }else
        {
            return Vector2.zero;
        }
    }
}
