using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FuelConnectorManager : MonoBehaviour
{
    public outputInputManager input;
    public outputInputManager output;
    public GameObject Legend;
    public bool started = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(started == true)
        {
            if(output != null && input == null)
            {
                selectInput();
            }

            if(output != null && input != null)
            {
                Connect();
                Legend.SetActive(false);
                started = false;
                input = null;
                output = null;
            }
        }
    }

    public void StartConnect()
    {
        started = true;
        Legend.SetActive(true);
        selectOutput();
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

            if(building.GetComponent<buildingType>().type == "standTank")
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
        input.inputGuid = output.guid;
        output.outputParent = input;
        output.outputGuid = input.guid;
    }
}
