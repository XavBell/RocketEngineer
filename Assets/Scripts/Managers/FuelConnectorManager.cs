using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuelConnectorManager : MonoBehaviour
{
    public outputInputManager input;
    public outputInputManager output;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void selectInput()
    {
        outputInputManager[] potentialInputsOutputs = FindObjectsOfType<outputInputManager>();
        List<GameObject> actualOutputInput = new List<GameObject>();
        foreach(outputInputManager outputInput in potentialInputsOutputs)
        {
            if(outputInput.gameObject.GetComponent<buildingType>() != null)
            {
                actualOutputInput.Add(outputInput.gameObject);
            }
        }

        foreach(GameObject building in actualOutputInput)
        {
            if(building.GetComponent<buildingType>().type == "launchPad")
            {
                building.GetComponent<buildingType>().inputUI.SetActive(true);
            }

            if(building.GetComponent<buildingType>().type == "staticFireStand")
            {
                building.GetComponent<buildingType>().inputUI.SetActive(true);
            }
        }
    }

    public void selectOutput()
    {
        outputInputManager[] potentialInputsOutputs = FindObjectsOfType<outputInputManager>();
        List<GameObject> actualOutputInput = new List<GameObject>();
        foreach(outputInputManager outputInput in potentialInputsOutputs)
        {
            if(outputInput.gameObject.GetComponent<buildingType>() != null)
            {
                actualOutputInput.Add(outputInput.gameObject);
            }
        }

        foreach(GameObject building in actualOutputInput)
        {
            if(building.GetComponent<buildingType>().type == "GSEtank")
            {
                building.GetComponent<buildingType>().outputUI.SetActive(true);
            }
        }
    }

    public void Connect()
    {
        input.inputParent = output;
        output.outputParent = input;
    }
}
