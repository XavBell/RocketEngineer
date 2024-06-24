using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class StageEditor : MonoBehaviour
{
    public GameObject engineBtn;
    public GameObject decouplerBtn;
    public GameObject stageContainer;
    public List<GameObject> buttons;
    public List<stageContainer> stageContainers;
    public GameObject container;
    public int previousChildCount = 0;
    public RocketController rocketController;
    public FloatingVelocity floatingVelocity;
    public TimeManager MyTime;
    public FloatingOrigin floatingOrigin;
    public BuildingManager buildingManager;

    public bool runStopDelay = false;
    public bool runTerminateDelay = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (runStopDelay == true)
        {
            stopDelayed();
            runStopDelay = false;
        }

        if (runTerminateDelay == true)
        {
            TerminateDelayed();
            runTerminateDelay = false;
        }
    }

    public void clearButtons()
    {
        foreach(GameObject button in buttons)
        {
            if(button != null)
            {
                Destroy(button);
            }
        }

        buttons.Clear();
    }

    public void UpdateButtons()
    {
        clearButtons();
        if(stageContainers.Count == 0)
        {
            GameObject StageContainer = Instantiate(stageContainer, container.transform);
            stageContainers.Add(StageContainer.GetComponent<stageContainer>());
        }

        Transform[] transforms = rocketController.gameObject.GetComponentsInChildren<Transform>();
        foreach(Transform part in transforms)
        {
            if(part.GetComponent<EngineComponent>())
            {
                GameObject EngineButton = Instantiate(engineBtn);
                GameObject child = EngineButton.GetComponentInChildren<Button>().gameObject;
                buttons.Add(child);
                child.transform.SetParent(stageContainers[stageContainers.Count - 1].container.gameObject.transform);
                child.GetComponentInChildren<partRef>().refObj = part.gameObject;
                child.GetComponentInChildren<partRef>().initializeEngineColors();
                DestroyImmediate(EngineButton);
            }

            if(part.GetComponent<DecouplerComponent>())
            {
                GameObject DecouplerButton = Instantiate(decouplerBtn);
                GameObject child = DecouplerButton.GetComponentInChildren<Button>().gameObject;
                buttons.Add(child);
                child.transform.SetParent(stageContainers[stageContainers.Count - 1].container.gameObject.transform);
                child.GetComponentInChildren<partRef>().refObj = part.gameObject;
                child.GetComponentInChildren<partRef>().initializeDecouplerColor();
                DestroyImmediate(DecouplerButton);
            }
        }
    }

    public void launch()
    {
        rocketController.GetComponent<PlanetGravity>().possessed = true;
        MasterManager masterManager = FindObjectOfType<MasterManager>();
        masterManager.gameState = "Flight";
        BuildingManager buildingManager = FindObjectOfType<BuildingManager>();
        buildingManager.enterFlightMode();
        masterManager.ActiveRocket = rocketController.gameObject;
        masterManager.GetComponent<pointManager>().nPoints += 2f;
        launchPadManager[] launchPadManagers = FindObjectsOfType<launchPadManager>();
        foreach (launchPadManager launchPadManager in launchPadManagers)
        {
            if (launchPadManager.ConnectedRocket == rocketController.gameObject)
            {
                launchPadManager.ConnectedRocket = null;
                launchPadManager.GetComponent<flowControllerForLaunchPads>().connected = false;
            }
        }
        flowController[] flowControllers = rocketController.GetComponentsInChildren<flowController>();
        foreach (flowController flowController in flowControllers)
        {
            flowController.origin = null;
            flowController.originGuid = Guid.Empty;
            flowController.selfRate = 0;
            flowController.opened = false;
        }
    }

    public void Stop()
    {
        runStopDelay = true;
    }

    public void stopDelayed()
    {
        if (runStopDelay == true)
        {
            if (rocketController != null)
            {
                rocketController.GetComponent<PlanetGravity>().possessed = false;
                rocketController.GetComponent<PlanetGravity>().rb.velocity = rocketController.GetComponent<PlanetGravity>().storedVelocity;
                rocketController.GetComponent<PlanetGravity>().velocityStored = false;
                floatingVelocity.velocity = (0, 0);

            }

            MasterManager masterManager = FindObjectOfType<MasterManager>();
            masterManager.gameState = "Building";
            BuildingManager buildingManager = FindObjectOfType<BuildingManager>();
            CameraControl camera = FindObjectOfType<CameraControl>();
            launchsiteManager launchsiteManager = FindObjectOfType<launchsiteManager>();

            //Must absolutely be before changing state for the landed rockets to not change of position since the landed state
            //is dependant on the distance from the camera
            camera.transform.position = launchsiteManager.commandCenter.transform.position;
            camera.transform.rotation = Quaternion.Euler(camera.transform.eulerAngles.x, camera.transform.eulerAngles.y, 0);

            //FORCE BRUTE SWITCH TO ON RAIL
            RocketController[] rockets = FindObjectsOfType<RocketController>();
            foreach (RocketController rp1 in rockets)
            {
                if ((rp1.GetComponent<RocketStateManager>().curr_X != rp1.GetComponent<RocketStateManager>().previous_X) && (rp1.GetComponent<RocketStateManager>().curr_Y != rp1.GetComponent<RocketStateManager>().previous_Y))
                {
                    rp1.GetComponent<RocketStateManager>().state = "rail";
                    rp1.GetComponent<PlanetGravity>().rb.simulated = false;
                    rp1.GetComponent<RocketPath>().startTime = MyTime.time;
                    rp1.GetComponent<RocketPath>().CalculateParameters();
                    rp1.GetComponent<RocketStateManager>().previousState = "rail";
                    rp1.GetComponent<RocketStateManager>().UpdatePosition();
                }
                else if ((rp1.GetComponent<RocketStateManager>().curr_X == rp1.GetComponent<RocketStateManager>().previous_X) && (rp1.GetComponent<RocketStateManager>().curr_Y == rp1.GetComponent<RocketStateManager>().previous_Y))
                {
                    rp1.GetComponent<RocketStateManager>().StateUpdater();
                }
            }

            masterManager.ActiveRocket = null;
            floatingOrigin.closestPlanet = null;
            buildingManager.exitFlightMode();
            floatingOrigin.bypass = true;
            launchPadManager[] launchPadManagers = FindObjectsOfType<launchPadManager>();
            foreach (launchPadManager launchPadManager in launchPadManagers)
            {
                if (launchPadManager.ConnectedRocket == rocketController.gameObject)
                {
                    launchPadManager.ConnectedRocket = null;
                    return;
                }
            }
            Physics.SyncTransforms();
        }
    }

    public void TerminateDelayed()
    {
        FindObjectOfType<MapManager>().mapOn();
        if (rocketController != null)
        {
            rocketController.GetComponent<PlanetGravity>().possessed = false;
            rocketController.GetComponent<PlanetGravity>().rb.velocity = rocketController.GetComponent<PlanetGravity>().storedVelocity;
            rocketController.GetComponent<PlanetGravity>().velocityStored = false;
            floatingVelocity.velocity = (0, 0);
            Destroy(rocketController.gameObject);
        }
        MasterManager masterManager = FindObjectOfType<MasterManager>();
        masterManager.gameState = "Building";
        CameraControl camera = FindObjectOfType<CameraControl>();
        launchsiteManager launchsiteManager = FindObjectOfType<launchsiteManager>();
        camera.transform.position = launchsiteManager.commandCenter.transform.position;
        //Must absolutely be before changing state for the landed rockets to not change of position since the landed state
        //is dependant on the distance from the camera
        camera.transform.position = launchsiteManager.commandCenter.transform.position;
        camera.transform.rotation = Quaternion.Euler(camera.transform.eulerAngles.x, camera.transform.eulerAngles.y, 0);

        //FORCE BRUTE SWITCH TO ON RAIL
        RocketController[] rockets = FindObjectsOfType<RocketController>();
        foreach (RocketController rp1 in rockets)
        {
            if ((rp1.GetComponent<RocketStateManager>().curr_X != rp1.GetComponent<RocketStateManager>().previous_X) && (rp1.GetComponent<RocketStateManager>().curr_Y != rp1.GetComponent<RocketStateManager>().previous_Y))
            {
                rp1.GetComponent<RocketStateManager>().state = "rail";
                rp1.GetComponent<PlanetGravity>().rb.simulated = false;
                rp1.GetComponent<RocketPath>().startTime = MyTime.time;
                rp1.GetComponent<RocketPath>().CalculateParameters();
                rp1.GetComponent<RocketStateManager>().previousState = "rail";
                rp1.GetComponent<RocketStateManager>().UpdatePosition();
            }
            else if ((rp1.GetComponent<RocketStateManager>().curr_X == rp1.GetComponent<RocketStateManager>().previous_X) && (rp1.GetComponent<RocketStateManager>().curr_Y == rp1.GetComponent<RocketStateManager>().previous_Y))
            {
                rp1.GetComponent<RocketStateManager>().StateUpdater();
            }
        }
        masterManager.ActiveRocket = null;
        camera.transform.rotation = Quaternion.Euler(camera.transform.eulerAngles.x, camera.transform.eulerAngles.y, 0);
        buildingManager.exitFlightMode();
        launchPadManager[] launchPadManagers = FindObjectsOfType<launchPadManager>();
        foreach (launchPadManager launchPadManager in launchPadManagers)
        {
            if (launchPadManager.ConnectedRocket == rocketController.gameObject)
            {
                launchPadManager.ConnectedRocket = null;
                return;
            }
        }
        FindObjectOfType<MapManager>().mapOn();
        this.gameObject.SetActive(false);
    }

    public void Terminate()
    {
        runTerminateDelay = true;
    }

   

}
