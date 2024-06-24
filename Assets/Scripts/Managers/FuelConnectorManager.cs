using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System;

public class FuelConnectorManager : MonoBehaviour
{
    public GameObject planet;
    public SolarSystemManager solarSystemManager;
    public BuildingManager buildingManager;
    public GameObject destroyPrefab;
    public container input;
    public container output;
    public GameObject Legend;
    public GameObject connectPopUp;
    public Toggle showToggle;
    public bool started = false;
    public List<LineRenderer> lines = new List<LineRenderer>();
    public List<GameObject> Destroyable = new List<GameObject>();
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
        if (started == true)
        {
            if (output != null && input == null)
            {
                selectInput();
            }

            if (output != null && input != null)
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
        if (showToggle.isOn)
        {
            foreach (LineRenderer line in lines)
            {
                if (line != null)
                {
                    Destroy(line.gameObject);
                }
            }

            foreach (GameObject destroy in Destroyable)
            {
                if (destroy != null)
                {
                    Destroy(destroy);
                }
            }

            lines.Clear();
            int n = 4;
            flowController[] flowControllers = FindObjectsOfType<flowController>();
            foreach (flowController flowController in flowControllers)
            {
                if (flowController.gameObject.GetComponent<buildingType>())
                {
                    if (flowController.origin != null)
                    {
                        GameObject line = Instantiate(linePrefab);
                        PanelFadeIn(line);
                        linePrefab.GetComponent<LineRenderer>();
                        LineRenderer lr = line.GetComponent<LineRenderer>();
                        Vector2 altitude1 = (planet.transform.position - flowController.transform.position).normalized * (n);
                        Vector2 altitude2 = (planet.transform.position - flowController.origin.transform.position).normalized * (n);
                        lr.positionCount = 4;
                        lr.SetPosition(0, flowController.transform.position);
                        lr.SetPosition(1, new Vector2(flowController.transform.position.x, altitude1.y));
                        lr.SetPosition(2, new Vector2(flowController.origin.transform.position.x, altitude2.y));
                        lr.SetPosition(3, flowController.origin.transform.position);
                        if (buildingManager.CanDestroy == true)
                        {
                            GameObject destroy = Instantiate(destroyPrefab);
                            destroy.transform.position = new Vector2((lr.GetPosition(2).x + lr.GetPosition(1).x) / 2, (altitude1.y + altitude2.y) / 2);
                            destroy.transform.parent = flowController.gameObject.transform;
                            destroy.transform.parent = null;
                            destroy.GetComponent<destroyConnection>().flowcontroller = flowController.gameObject;
                            destroy.GetComponent<destroyConnection>().line = line;
                            Destroyable.Add(destroy);
                        }
                        flowController.refLine = line;
                        if (flowController.destination.type == "fuel")
                        {
                            lr.startColor = orange;
                            lr.endColor = orange;
                        }
                        else if (flowController.destination.type == "oxidizer")
                        {
                            lr.startColor = blue;
                            lr.endColor = blue;
                        }
                        else
                        {
                            lr.startColor = Color.white;
                            lr.endColor = Color.white;
                        }
                        lines.Add(lr);
                        line.transform.parent = flowController.gameObject.transform;
                    }
                }
                n += 2;
            }

            launchPadManager[] launchPadManagers = FindObjectsOfType<launchPadManager>();
            container[] containers = FindObjectsOfType<container>();
            foreach (launchPadManager launchPad in launchPadManagers)
            {
                foreach (Guid container in launchPad.connectedContainersPerLine)
                {
                    foreach (container container1 in containers)
                    {
                        if (container1.guid == container)
                        {

                            GameObject line = Instantiate(linePrefab);
                            PanelFadeIn(line);
                            linePrefab.GetComponent<LineRenderer>();
                            LineRenderer lr = line.GetComponent<LineRenderer>();
                            Vector2 altitude1 = (planet.transform.position - launchPad.transform.position).normalized * (n);
                            Vector2 altitude2 = (planet.transform.position - container1.transform.position).normalized * (n);
                            lr.positionCount = 4;
                            lr.SetPosition(0, launchPad.transform.position);
                            lr.SetPosition(1, new Vector2(launchPad.transform.position.x, altitude1.y));
                            lr.SetPosition(2, new Vector2(container1.transform.position.x, altitude2.y));
                            lr.SetPosition(3, container1.transform.position);
                            if (buildingManager.CanDestroy == true)
                            {
                                GameObject destroy = Instantiate(destroyPrefab);
                                destroy.transform.position = new Vector2((lr.GetPosition(2).x + lr.GetPosition(1).x) / 2, (altitude1.y + altitude2.y) / 2);
                                destroy.transform.parent = launchPad.gameObject.transform;
                                destroy.transform.parent = null;
                                destroy.GetComponent<destroyConnection>().flowcontroller = launchPad.gameObject;
                                destroy.GetComponent<destroyConnection>().line = line;
                                Destroyable.Add(destroy);
                            }
                            lr.startColor = Color.white;
                            lr.endColor = Color.white;
                            lines.Add(lr);
                            line.transform.parent = launchPad.gameObject.transform;
                            n += 2;
                        }
                    }
                }
            }

            flowControllerForTankStand[] flowControllerForTankStands = FindObjectsOfType<flowControllerForTankStand>();
            foreach (flowControllerForTankStand flowControllerForTankStand in flowControllerForTankStands)
            {
                if (flowControllerForTankStand.origin != null)
                {
                    GameObject line = Instantiate(linePrefab);
                    PanelFadeIn(line);
                    linePrefab.GetComponent<LineRenderer>();
                    LineRenderer lr = line.GetComponent<LineRenderer>();
                    Vector2 altitude1 = (planet.transform.position - flowControllerForTankStand.transform.position).normalized * (n);
                    Vector2 altitude2 = (planet.transform.position - flowControllerForTankStand.origin.transform.position).normalized * (n);
                    lr.positionCount = 4;
                    lr.SetPosition(0, flowControllerForTankStand.transform.position);
                    lr.SetPosition(1, new Vector2(flowControllerForTankStand.transform.position.x, altitude1.y));
                    lr.SetPosition(2, new Vector2(flowControllerForTankStand.origin.transform.position.x, altitude2.y));
                    lr.SetPosition(3, flowControllerForTankStand.origin.transform.position);
                    if (buildingManager.CanDestroy == true)
                    {
                        GameObject destroy = Instantiate(destroyPrefab);
                        destroy.transform.position = new Vector2((lr.GetPosition(2).x + lr.GetPosition(1).x) / 2, (altitude1.y + altitude2.y) / 2);
                        destroy.transform.parent = flowControllerForTankStand.transform;
                        destroy.transform.parent = null;
                        destroy.GetComponent<destroyConnection>().flowcontroller = flowControllerForTankStand.gameObject;
                        destroy.GetComponent<destroyConnection>().line = line;
                        Destroyable.Add(destroy);
                    }
                    lr.startColor = Color.white;
                    lr.endColor = Color.white;
                    lines.Add(lr);
                    line.transform.parent = flowControllerForTankStand.gameObject.transform;
                }
                n += 2;
            }
        }

        if (!showToggle.isOn)
        {
            foreach (LineRenderer line in lines)
            {
                if (line != null)
                {
                    Destroy(line.gameObject);
                }
            }

            foreach (GameObject destroy in Destroyable)
            {
                if (destroy != null)
                {
                    Destroy(destroy);
                }
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

    public IEnumerator ActiveDeactive(float waitTime, GameObject panel, bool activated)
    {
        yield return new WaitForSeconds(waitTime);
        panel.SetActive(activated);
    }

    public void selectInput()
    {
        outputInputManager[] potentialInputsOutputs = FindObjectsOfType<outputInputManager>();
        List<GameObject> actualOutputInput = new List<GameObject>();
        foreach (outputInputManager outputInput in potentialInputsOutputs)
        {
            if (outputInput.gameObject.GetComponent<buildingType>() != null)
            {
                actualOutputInput.Add(outputInput.gameObject);
            }
        }

        foreach (GameObject building in actualOutputInput)
        {
            if (building.GetComponent<buildingType>().type == "launchPad" && building.GetComponent<buildingType>().inputUI.activeSelf == false)
            {
                building.GetComponent<buildingType>().inputUI.SetActive(true);
                PanelFadeIn(building.GetComponent<buildingType>().inputUI);
            }

            if (building.GetComponent<buildingType>().type == "staticFireStand" && building.GetComponent<buildingType>().inputUI.activeSelf == false)
            {
                building.GetComponent<buildingType>().inputUI.SetActive(true);
                PanelFadeIn(building.GetComponent<buildingType>().inputUI);
            }

            if (building.GetComponent<buildingType>().type == "standTank" && building.GetComponent<buildingType>().inputUI.activeSelf == false)
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
        foreach (container container in potentialContainer)
        {
            if (container.gameObject.GetComponent<buildingType>() != null)
            {
                actualContainer.Add(container.gameObject);
            }
        }

        foreach (GameObject building in actualContainer)
        {
            if (building.GetComponent<buildingType>().type == "GSEtank")
            {
                building.GetComponent<buildingType>().outputUI.SetActive(true);
                PanelFadeIn(building.GetComponent<buildingType>().outputUI);

            }
        }
    }

    //TODO refactor for new rocket system
    public void Connect()
    {
        if (input.GetComponent<flowControllerStaticFire>())
        {
            input.flowController.destination = input;
            input.flowController.origin = output;
            input.GetComponent<flowControllerStaticFire>().updateGuid();
        }

        if (input.GetComponent<flowControllerForTankStand>())
        {
            input.GetComponent<flowControllerForTankStand>().origin = output;
            input.GetComponent<flowControllerForTankStand>().updateGuid();
        }

        if (input.GetComponent<flowControllerForLaunchPads>())
        {
            if (input.type == "oxidizer")
            {
                input.GetComponent<flowControllerForLaunchPads>().oxidizerContainerOrigin = output;
            }

            if (input.type == "fuel")
            {
                input.GetComponent<flowControllerForLaunchPads>().fuelContainerOrigin = output;
            }

            if (input.GetComponent<launchPadManager>().ConnectedRocket != null)
            {
                input.GetComponent<flowControllerForLaunchPads>().setTankOrigin();
            }

            input.GetComponent<flowControllerForLaunchPads>().updateGuid();
        }

        ShowConnection();

        connectPopUp.SetActive(true);
        PanelFadeIn(connectPopUp);
        StartCoroutine(ActiveDeactive(1, connectPopUp, false));

    }
}
