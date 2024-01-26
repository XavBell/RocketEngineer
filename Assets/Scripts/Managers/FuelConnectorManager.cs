using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using UnityEditor;

public class FuelConnectorManager : MonoBehaviour
{
    public container input;
    public container output;
    public GameObject Legend;
    public GameObject connectPopUp;
    public Toggle showToggle;
    public bool started = false;
    public List<LineRenderer> lines = new List<LineRenderer>();
    public GameObject linePrefab;
    public Color orange;
    public Color blue;

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

    public void ShowConnection()
    {
        if(showToggle.isOn)
        {
            lines.Clear();
            outputInputManager[] outputInputManagers = FindObjectsOfType<outputInputManager>();
            foreach(outputInputManager outputInputManager in outputInputManagers)
            {
                if(outputInputManager.gameObject.GetComponent<buildingType>())
                {
                    if(outputInputManager.inputParent != null)
                    {
                        GameObject line = Instantiate(linePrefab);
                        PanelFadeIn(line);
                        linePrefab.GetComponent<LineRenderer>();
                        LineRenderer lr = line.GetComponent<LineRenderer>();
                        lr.positionCount = 2;
                        lr.SetPosition(0, outputInputManager.transform.position);
                        lr.SetPosition(1, outputInputManager.inputParent.transform.position);
                        if(outputInputManager.circuit == "fuel")
                        {
                            lr.startColor = orange;
                            lr.endColor = orange;
                        }

                        if(outputInputManager.circuit == "oxidizer")
                        {
                            lr.startColor = blue;
                            lr.endColor = blue;
                        }
                        lines.Add(lr);
                        line.transform.parent = outputInputManager.gameObject.transform;
                    }
                }
            }
        }

        if(!showToggle.isOn)
        {
            foreach(LineRenderer line in lines)
            {
                Destroy(line.gameObject);
            }
            
        }
    }

    public void StartConnect()
    {
        started = true;
        Legend.SetActive(true);
        selectOutput();
    }

    public void PanelFadeIn(GameObject panel)
    {
        panel.transform.localScale = new Vector3(0, 0, 0);
        panel.transform.DOScale(1, 0.1f);
    }

    private IEnumerator ActiveDeactive(float waitTime, GameObject panel, bool activated)
    {
        yield return new WaitForSeconds(waitTime);
        panel.SetActive(activated);
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
            if(building.GetComponent<buildingType>().type == "launchPad" && building.GetComponent<buildingType>().inputUI.active == false)
            {
                building.GetComponent<buildingType>().inputUI.SetActive(true);
                PanelFadeIn(building.GetComponent<buildingType>().inputUI);
            }

            if(building.GetComponent<buildingType>().type == "staticFireStand"&& building.GetComponent<buildingType>().inputUI.active == false)
            {
                building.GetComponent<buildingType>().inputUI.SetActive(true);
                PanelFadeIn(building.GetComponent<buildingType>().inputUI);
            }

            if(building.GetComponent<buildingType>().type == "standTank"&& building.GetComponent<buildingType>().inputUI.active == false)
            {
                building.GetComponent<buildingType>().inputUI.SetActive(true);
                PanelFadeIn(building.GetComponent<buildingType>().inputUI);
            }
        }
    }

    public void selectOutput()
    {
        container[] potentialContainer = FindObjectsOfType<container>();
        List<GameObject> actualContainer = new List<GameObject>();
        foreach(container container in potentialContainer)
        {
            if(container.gameObject.GetComponent<buildingType>() != null)
            {
                actualContainer.Add(container.gameObject);
            }
        }

        foreach(GameObject building in actualContainer)
        {
            if(building.GetComponent<buildingType>().type == "GSEtank")
            {
                building.GetComponent<buildingType>().outputUI.SetActive(true);
                PanelFadeIn(building.GetComponent<buildingType>().outputUI);
                
            }
        }
    }

    public void Connect()
    {
        //input.inputParent = output;
        //input.inputGuid = output.guid;
        //output.outputParent = input;
        //output.outputGuid = input.guid;
        if(input.GetComponent<flowControllerStaticFire>())
        {
            input.flowController.destination = input;
            input.flowController.origin = output;
        }

        if(input.GetComponent<flowControllerForTankStand>())
        {
            input.GetComponent<flowControllerForTankStand>().origin = output;
        }

        if(input.GetComponent<flowControllerForLaunchPads>())
        {
            if(input.type == "oxidizer")
            {
                input.GetComponent<flowControllerForLaunchPads>().oxidizerContainerOrigin = output;
            }

            if(input.type == "fuel")
            {
                input.GetComponent<flowControllerForLaunchPads>().fuelContainerOrigin = output;
            }

            if(input.GetComponent<launchPadManager>().ConnectedRocket != null)
            {
                input.GetComponent<flowControllerForLaunchPads>().setTankOrigin();
            }
            
        }

        connectPopUp.SetActive(true);
        PanelFadeIn(connectPopUp);
        StartCoroutine(ActiveDeactive(1,connectPopUp, false ));

    }
}
