using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class StageViewer : MonoBehaviour
{
    public GameObject rocket;
    public TMP_Dropdown stageDropdown;
    public GameObject EngineUI;
    public GameObject DecouplerUI;
    public GameObject TankUI;
    public GameObject CapsuleUI;
    public GameObject Content;
    public TMP_Text altitude;
    public FloatingOrigin floatingOrigin;
    public TimeManager MyTime;
    public BuildingManager buildingManager;
    public bool runStopDelay = false;

    private int nStages = -1;

    // Start is called before the first frame update
    void Start()
    {
        floatingOrigin = FindObjectOfType<FloatingOrigin>();
        MyTime = FindObjectOfType<TimeManager>();
        buildingManager = FindObjectOfType<BuildingManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if(rocket != null)
        {
            if(nStages != rocket.GetComponent<Rocket>().Stages.Count)
            {
                fullReset(false);
                nStages = rocket.GetComponent<Rocket>().Stages.Count;
            }
            //altitude.text = (Vector2.Distance(rocket.GetComponent<PlanetGravity>().planet.transform.position, rocket.transform.position)-63710f).ToString();
        }
    }

    void FixedUpdate()
    {
        if(runStopDelay == true)
        {
            stopDelayed();
        }
    }

    public void fullReset(bool forcedCall)
    {
        updateStagesView(forcedCall);
        stageDropdown.value = 0;
        updateInfoPerStage(forcedCall);
    }
    public void updateStagesView(bool forcedCall)
    {
        if(rocket != null)
        {
            List<string> options = new List<string>();
            foreach(Transform child in Content.transform)
            {
                if(child.gameObject.GetComponent<EngineUIModule>() != null || child.gameObject.GetComponent<TankUIModule>() != null || child.gameObject.GetComponent<DecouplerUIModule>() != null)
                {
                    DestroyImmediate(child.gameObject);
                }

            }
            stageDropdown.ClearOptions();
            int i = 0;
            foreach(Stages stage in rocket.GetComponent<Rocket>().Stages)
            {
                options.Add($"Stage {i}");
                i++;
            }
            stageDropdown.value = 0;
            stageDropdown.AddOptions(options);
        }
    }

    public void resetDropdown()
    {
        List<string> options = new List<string>();
        stageDropdown.ClearOptions();
        int i = 0;
        foreach (Stages stage in rocket.GetComponent<Rocket>().Stages)
        {
            options.Add($"Stage {i}");
            i++;
        }
        stageDropdown.AddOptions(options);
        stageDropdown.value = 0;
    }

    public void updateInfoPerStage(bool forcedCall)
    {
        int maxCount = Content.transform.childCount;
        if(maxCount != 0)
        {
            for(int i = maxCount-1; i >= 0; i--)
            {
                Destroy(Content.transform.GetChild(i).gameObject);
            }
        }
        int value = 0;
        if(forcedCall == false)
        {
            value = stageDropdown.value;
        }

        if(forcedCall == true)
        {
            value = 0;
            stageDropdown.value = 0;
        }

        foreach(RocketPart part in rocket.GetComponent<Rocket>().Stages[value].Parts)
        {
            if(part._partType == "engine")
            {
                GameObject engineUI = Instantiate(EngineUI, Content.transform);
                engineUI.GetComponent<EngineUIModule>().engine = part.GetComponent<Engine>();
            }

            if(part._partType == "decoupler")
            {
                GameObject decouplerUI = Instantiate(DecouplerUI, Content.transform);
                decouplerUI.GetComponent<DecouplerUIModule>().decoupler = part.GetComponent<Decoupler>();
            }

            if(part._partType == "tank")
            {
                GameObject tankUI = Instantiate(TankUI, Content.transform);
                tankUI.GetComponent<TankUIModule>().tank = part.GetComponent<container>();
            }

            if(part._partType == "satellite")
            {
                GameObject satUI = Instantiate(CapsuleUI, Content.transform);
                satUI.GetComponent<CapsuleUIManager>().satellite = part.GetComponent<Satellite>();
            }
        }
    }

    public void launch()
    {
        rocket.GetComponent<PlanetGravity>().possessed = true;
        MasterManager masterManager = FindObjectOfType<MasterManager>();
        masterManager.gameState = "Flight";
        BuildingManager buildingManager = FindObjectOfType<BuildingManager>();
        buildingManager.enterFlightMode();
        masterManager.ActiveRocket = rocket;
        masterManager.GetComponent<pointManager>().nPoints += 2f;
        launchPadManager[] launchPadManagers = FindObjectsOfType<launchPadManager>();
            foreach(launchPadManager launchPadManager in launchPadManagers)
            {
                if(launchPadManager.ConnectedRocket == rocket)
                {
                    launchPadManager.ConnectedRocket = null;
                    return;
                }
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
            if(rocket != null)
            {
                rocket.GetComponent<PlanetGravity>().possessed = false;
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
            Rocket[] rockets = FindObjectsOfType<Rocket>();
            foreach (Rocket rp1 in rockets)
            {
                if((rp1.GetComponent<RocketStateManager>().curr_X != rp1.GetComponent<RocketStateManager>().previous_X) && (rp1.GetComponent<RocketStateManager>().curr_Y != rp1.GetComponent<RocketStateManager>().previous_Y))
                {
                    rp1.GetComponent<RocketStateManager>().state = "rail";
                    rp1.GetComponent<PlanetGravity>().rb.simulated = false;
                    rp1.GetComponent<RocketPath>().startTime = MyTime.time;
                    rp1.GetComponent<RocketPath>().CalculateParameters();
                    rp1.GetComponent<RocketStateManager>().previousState = "rail";
                    rp1.GetComponent<RocketStateManager>().UpdatePosition();
                }else if((rp1.GetComponent<RocketStateManager>().curr_X == rp1.GetComponent<RocketStateManager>().previous_X) && (rp1.GetComponent<RocketStateManager>().curr_Y == rp1.GetComponent<RocketStateManager>().previous_Y)){
                    rp1.GetComponent<RocketStateManager>().StateUpdater();
                }
            }

            masterManager.ActiveRocket = null;
            floatingOrigin.closestPlanet = null;
            buildingManager.exitFlightMode();
            floatingOrigin.bypass = true;
            runStopDelay = false;
            launchPadManager[] launchPadManagers = FindObjectsOfType<launchPadManager>();
            foreach(launchPadManager launchPadManager in launchPadManagers)
            {
                if(launchPadManager.ConnectedRocket == rocket)
                {
                    launchPadManager.ConnectedRocket = null;
                    return;
                }
            }
            Physics.SyncTransforms();
        }
    }

    public void Terminate()
    {
        if(rocket != null)
        {
            rocket.GetComponent<PlanetGravity>().possessed = false;
            Destroy(rocket);   
        }
        MasterManager masterManager = FindObjectOfType<MasterManager>();
        masterManager.gameState = "Building";
        CameraControl camera = FindObjectOfType<CameraControl>();
        launchsiteManager launchsiteManager = FindObjectOfType<launchsiteManager>();
        camera.transform.position = launchsiteManager.commandCenter.transform.position;
        this.gameObject.SetActive(false);
        masterManager.ActiveRocket = null;
        camera.transform.rotation = Quaternion.Euler(camera.transform.eulerAngles.x, camera.transform.eulerAngles.y, 0);
        buildingManager.exitFlightMode();
        launchPadManager[] launchPadManagers = FindObjectsOfType<launchPadManager>();
        foreach (launchPadManager launchPadManager in launchPadManagers)
        {
            if (launchPadManager.ConnectedRocket == rocket)
            {
                launchPadManager.ConnectedRocket = null;
                return;
            }
        }
    }
}
