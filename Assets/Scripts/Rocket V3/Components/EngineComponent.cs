using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FuelConsumerComponent))]
[RequireComponent(typeof(ThrustComponent))]
public class EngineComponent : MonoBehaviour
{
    public FuelConsumerComponent fuelConsumerComponent;
    public ThrustComponent thrustComponent;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
