using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FuelConsumerComponent : MonoBehaviour
{
    public float fuelConsumptionRate;
    public List<TankComponent> tanks = new List<TankComponent>();
    //public List<GameObject> visitedObjects = new List<GameObject>();
    public GameObject hi;

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
        //visitedObjects.Clear();
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
}
