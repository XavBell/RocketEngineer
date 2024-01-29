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
            flowController[] flowControllers = FindObjectsOfType<flowController>();
            foreach(flowController flowController in flowControllers)
            {
                if(flowController.gameObject.GetComponent<buildingType>())
                {
                    if(flowController.origin != null)
                    {
                        GameObject line = Instantiate(linePrefab);
                        PanelFadeIn(line);
                        linePrefab.GetComponent<LineRenderer>();
                        LineRenderer lr = line.GetComponent<LineRenderer>();
                        lr.positionCount = 2;
                        lr.SetPosition(0, flowController.transform.position);
                        lr.SetPosition(1, flowController.origin.transform.position);
                        if(flowController.destination.type == "fuel")
                        {
                            lr.startColor = orange;
                            lr.endColor = orange;
                        }else if(flowController.destination.type == "oxidizer")
                        {
                            lr.startColor = blue;
                            lr.endColor = blue;
                        }else{
                            lr.startColor = Color.white;
                            lr.endColor = Color.white;
                        }
                        lines.Add(lr);
                        line.transform.parent = flowController.gameObject.transform;
                    }
                }
            }

            flowControllerForLaunchPads[] flowControllersLaunchPad = FindObjectsOfType<flowControllerForLaunchPads>();
            foreach(flowControllerForLaunchPads flowControllerForLaunchPad in flowControllersLaunchPad)
            {
                if(flowControllerForLaunchPad.oxidizerContainerOrigin != null)
                {
                    GameObject line = Instantiate(linePrefab);
                    PanelFadeIn(line);
                    linePrefab.GetComponent<LineRenderer>();
                    LineRenderer lr = line.GetComponent<LineRenderer>();
                    lr.positionCount = 2;
                    lr.SetPosition(0, flowControllerForLaunchPad.transform.position);
                    lr.SetPosition(1, flowControllerForLaunchPad.oxidizerContainerOrigin.transform.position);
                    lr.startColor = blue;
                    lr.endColor = blue;
                    lines.Add(lr);
                    line.transform.parent = flowControllerForLaunchPad.gameObject.transform;
                }

                if(flowControllerForLaunchPad.fuelContainerOrigin != null)
                {
                    GameObject line = Instantiate(linePrefab);
                    PanelFadeIn(line);
                    linePrefab.GetComponent<LineRenderer>();
                    LineRenderer lr = line.GetComponent<LineRenderer>();
                    lr.positionCount = 2;
                    lr.SetPosition(0, flowControllerForLaunchPad.transform.position);
                    lr.SetPosition(1, flowControllerForLaunchPad.fuelContainerOrigin.transform.position);
                    lr.startColor = orange;
                    lr.endColor = orange;
                    lines.Add(lr);
                    line.transform.parent = flowControllerForLaunchPad.gameObject.transform;
                }
            }

            flowControllerForTankStand[] flowControllerForTankStands = FindObjectsOfType<flowControllerForTankStand>();
            foreach(flowControllerForTankStand flowControllerForTankStand in flowControllerForTankStands)
            {
                if(flowControllerForTankStand.origin != null)
                {
                    GameObject line = Instantiate(linePrefab);
                    PanelFadeIn(line);
                    linePrefab.GetComponent<LineRenderer>();
                    LineRenderer lr = line.GetComponent<LineRenderer>();
                    lr.positionCount = 2;
                    lr.SetPosition(0, flowControllerForTankStand.transform.position);
                    lr.SetPosition(1, flowControllerForTankStand.origin.transform.position);
                    lr.startColor = Color.white;
                    lr.endColor = Color.white;
                    lines.Add(lr);
                    line.transform.parent = flowControllerForTankStand.gameObject.transform;
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
