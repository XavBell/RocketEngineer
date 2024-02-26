using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class rocketCursorManager : MonoBehaviour
{
    MasterManager masterManager = null;
    BuildingManager buildingManager = null;
    FloatingOrigin floatingOrigin = null;
    StageViewer stageViewer = null;
    GameObject rocket = null;
    TimeManager MyTime;
    bool runClickDelay = false;
    // Start is called before the first frame update
    void Start()
    {
        MyTime = FindObjectOfType<TimeManager>();
        masterManager = FindObjectOfType<MasterManager>();
        floatingOrigin = FindObjectOfType<FloatingOrigin>();
        stageViewer = FindObjectOfType<StageViewer>();
        buildingManager = FindObjectOfType<BuildingManager>();
        rocket = this.transform.parent.gameObject;
    }

    void Update()
    {
        if(runClickDelay == false && stageViewer.runStopDelay == false)
        {
            //floatingOrigin.enabled = true;
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(runClickDelay == true)
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
            floatingOrigin.closestPlanet = null;
            if (masterManager.ActiveRocket != null)
            {
                masterManager.ActiveRocket.GetComponent<Rocket>().throttle = 0;
                masterManager.ActiveRocket.GetComponent<PlanetGravity>().possessed = false;
            }

            Rocket[] rockets = FindObjectsOfType<Rocket>();
            foreach (Rocket rp1 in rockets)
            {
                if(rp1.GetComponent<RocketStateManager>().state != "landed")
                {
                    rp1.GetComponent<RocketStateManager>().state = "rail";
                    rp1.GetComponent<PlanetGravity>().rb.simulated = false;
                    rp1.GetComponent<RocketPath>().startTime = MyTime.time;
                    rp1.GetComponent<RocketPath>().CalculateParameters();
                    rp1.GetComponent<RocketStateManager>().previousState = "rail";
                    rp1.GetComponent<RocketStateManager>().UpdatePosition();
                }
            }

            masterManager.ActiveRocket = null;
            CameraControl camera = FindObjectOfType<CameraControl>();
            camera.cam.transform.position = rocket.transform.position;
            buildingManager.enterFlightMode();
            rocket.GetComponent<PlanetGravity>().possessed = true;
            stageViewer.rocket = rocket;
            stageViewer.updateStagesView(false);
            stageViewer.updateInfoPerStage(false);
            floatingOrigin.bypass = true;
            runClickDelay = false;
            Physics.SyncTransforms();
        }


    }
}
