using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.UI;

public class FuelConsumerComponent : MonoBehaviour
{
    public float fuelConsumptionRate;
    public Substance requiredOxidizer;
    public Substance requiredFuel;
    public List<TankComponent> tanks = new List<TankComponent>();

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void FindTanks()
    {
        tanks.Clear();
        GameObject highestParent = findHighestParent(this.gameObject);
        if(highestParent.GetComponent<TankComponent>() != null)
        {
            tanks.Add(highestParent.GetComponent<TankComponent>());
        }
        AddTanks(highestParent);
    }

    public GameObject findHighestParent(GameObject currentObject)
    {
        if (currentObject.transform.parent == null || currentObject.transform.parent.GetComponent<PhysicsPart>().BlockFuelFlow == true)
        {
            return currentObject;
        }
        else
        {
            currentObject = currentObject.transform.parent.gameObject;
            while (currentObject.transform.parent != null && currentObject.transform.parent.GetComponent<PhysicsPart>().BlockFuelFlow == false && currentObject.GetComponent<PhysicsPart>())
            {
                currentObject = currentObject.transform.parent.gameObject;
            }
            return currentObject;
        }
    }


    public void AddTanks(GameObject parent)
    {
        print(parent.transform.childCount);
        foreach (Transform  child in parent.transform)
        {
            PhysicsPart tankComponent = null;
            if(child.GetComponent<PhysicsPart>() == null)
            {
                continue;
            }else
            {
                tankComponent = child.GetComponent<PhysicsPart>();
            }
            if(tankComponent.type == "tank")
            {
                if(!tanks.Contains(tankComponent.GetComponent<TankComponent>()))
                {
                    tanks.Add(tankComponent.GetComponent<TankComponent>());
                }
            }

            if(tankComponent.BlockFuelFlow == false)
            {
                AddTanks(tankComponent.gameObject);
            }
        }
    }

    //Amount in kg
    public bool propellantSufficient(float amount, Propellants propellants)
    {
        float oxidizerQty = amount * propellants.oxidizerToFuelRatio;
        float fuelQty = amount - oxidizerQty;

        float oxidizerAvailable = 0;
        float fuelAvailable = 0;

        foreach (container container in GetFuelContainers(propellants.oxidizer))
        {
            oxidizerAvailable += container.mass;
        }

        foreach (container container in GetFuelContainers(propellants.fuel))
        {
            fuelAvailable += container.mass;
        }

        if(oxidizerAvailable >= oxidizerQty && fuelAvailable >= fuelQty)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void ConsumeFuel(float amount, Propellants propellants)
    {
        //Determine how much fuel and oxidizer to consume
        float oxidizerQty = amount * propellants.oxidizerToFuelRatio;
        float fuelQty = amount - oxidizerQty;

        //Find necessaryContainers
        List<container> oxidizerContainers = GetFuelContainers(propellants.oxidizer);
        List<container> fuelContainers = GetFuelContainers(propellants.fuel);

        int oxidizerContainerCount = oxidizerContainers.Count;
        int fuelContainerCount = fuelContainers.Count;

        float ConsumeFuelPerContainer = fuelQty / fuelContainerCount;
        float ConsumeOxidizerPerContainer = oxidizerQty / oxidizerContainerCount;

        //Consume fuel and oxidizer
        foreach (container container in oxidizerContainers)
        {
            container.Consume(ConsumeOxidizerPerContainer);
        }

        foreach (container container in fuelContainers)
        {
            container.Consume(ConsumeFuelPerContainer);
        }

    }

    public List<container> GetFuelContainers(Substance substance)
    {
        List<container> containers = new List<container>();
        foreach (TankComponent tank in tanks)
        {
            if(tank.GetComponent<container>().substance == substance)
            {
                containers.Add(tank.GetComponent<container>());
            }
        }
        return containers;
    }
}
