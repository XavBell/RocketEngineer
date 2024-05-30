using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class rocketCursorManager : MonoBehaviour
{
    public Canvas canvasGroup = null;
    MasterManager masterManager = null;
    BuildingManager buildingManager = null;
    FloatingOrigin floatingOrigin = null;
    StageViewer stageViewer = null;
    public GameObject rocket = null;
    TimeManager MyTime;
    FloatingVelocity floatingVelocity;
    bool runClickDelay = false;
    // Start is called before the first frame update
    void Start()
    {
        canvasGroup = GetComponentInChildren<Canvas>();
        canvasGroup.worldCamera = GameObject.FindWithTag("mapCam").GetComponent<Camera>();
        MyTime = FindObjectOfType<TimeManager>();
        masterManager = FindObjectOfType<MasterManager>();
        floatingOrigin = FindObjectOfType<FloatingOrigin>();
        floatingVelocity = FindObjectOfType<FloatingVelocity>();
        stageViewer = FindObjectOfType<StageViewer>();
        buildingManager = FindObjectOfType<BuildingManager>();
    }

    void Update()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (runClickDelay == true)
        {
            clickDelayed();
        }
    }


    public void clicked()
    {
        runClickDelay = true;
    }

    public void clickDelayed()
    {
        if (runClickDelay == true)
        {
            runClickDelay = false;
            floatingOrigin.closestPlanet = null;
            if (masterManager.ActiveRocket != null)
            {
                masterManager.ActiveRocket.GetComponent<Rocket>().throttle = 0;
                masterManager.ActiveRocket.GetComponent<PlanetGravity>().possessed = false;
                masterManager.ActiveRocket.GetComponent<PlanetGravity>().rb.velocity = masterManager.ActiveRocket.GetComponent<PlanetGravity>().storedVelocity;
                masterManager.ActiveRocket.GetComponent<PlanetGravity>().velocityStored = false;
                floatingVelocity.velocity = (0, 0);
            }

            //Must absolutely be before changing state for the landed rockets to not change of position since the landed state
            //is dependant on the distance from the camera
            CameraControl camera = FindObjectOfType<CameraControl>();
            camera.cam.transform.position = rocket.transform.position;

            Rocket[] rockets = FindObjectsOfType<Rocket>();
            foreach (Rocket rp1 in rockets)
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
                else if ((rp1.GetComponent<RocketStateManager>().curr_X == rp1.GetComponent<RocketStateManager>().previous_X) && (rp1.GetComponent<RocketStateManager>().curr_Y == rp1.GetComponent<RocketStateManager>().previous_Y) && rp1 != rocket)
                {
                    rp1.GetComponent<RocketStateManager>().StateUpdater();
                }
            }

            masterManager.ActiveRocket = rocket;
            buildingManager.enterFlightMode();
            rocket.GetComponent<PlanetGravity>().possessed = true;
            stageViewer = FindObjectOfType<StageViewer>();
            stageViewer.rocket = rocket;
            masterManager.gameState = "Flight";
            stageViewer.updateStagesView(false);
            stageViewer.updateInfoPerStage(false);
            floatingOrigin.bypass = true;
            Physics.SyncTransforms();
        }


    }
}
