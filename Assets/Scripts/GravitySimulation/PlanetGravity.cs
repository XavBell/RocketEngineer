using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.SceneManagement;
using System.Linq;
using UnityEngine.UI;


public class PlanetGravity : MonoBehaviour
{

    //public LineRenderer PlLR;
    public GameObject[] planets;
    public bool initialized = false;
    public GameObject core;

    //Gravity variables for Earth
    public GameObject planet;
    public float Mass = 0f; //Planet mass in kg
    public float G = 0f; //Gravitational constant
    public float atmoAlt = 0.0f;
    public float aeroCoefficient = 0f;
    public float planetRadius = 0f; //Planet radius in m
    public float planetDensity = 0f;
    float maxAlt;


    //Rocket variables
    public Rigidbody2D rb;
    public float rocketMass;
    public float thrust;
    UnityEngine.Vector3 AeroForces;
    public ParticleSystem particle;

    public bool stageUpdated = false;

    public GameObject WorldSaveManager;

    public bool possessed = false;
    public bool landed = false;
    public Vector2 previousVelocity = new Vector2(Mathf.Infinity, Mathf.Infinity);

    public GameObject TimeRef;
    public TimeManager TimeManager;
    public MasterManager MasterManager;
    public StageViewer stageViewer;

    public float time = 0;

    public SolarSystemManager SolarSystemManager;
    public DoubleTransform dt;
    public Camera cam;
    public float baseCoefficient = 0.075f;

    // Start is called before the first frame update
    void Start()
    {
        WorldSaveManager = GameObject.FindGameObjectWithTag("WorldSaveManager");
        SolarSystemManager = GameObject.FindObjectOfType<SolarSystemManager>();
        rb = GetComponent<Rigidbody2D>();
        core.GetComponent<Rocket>().updateMass();
        rb.mass = core.GetComponent<Rocket>().rocketMass;
        if (SceneManager.GetActiveScene().name == "SampleScene")
        {
            cam = GameObject.FindObjectOfType<Camera>();
            G = SolarSystemManager.G;
        }

    }


    void FixedUpdate()
    {
        checkManager();

        if (SceneManager.GetActiveScene().name == "SampleScene" && cam != null)
        {

            initializeRocket();

            if (landed == true)
            {
                rb.simulated = false;
            }
            updateReferenceBody();
            if (possessed == true)
            {
                //rb.simulated = true;
                MasterManager.ActiveRocket = core;
                core.GetComponent<Rocket>().updateRocketStaging();
            }
        }
    }

    void Update()
    {
        if (possessed == true)
        {
            MasterManager.ActiveRocket = core;
            core.GetComponent<Rocket>().controlThrust();
            core.GetComponent<Rocket>()._orientation();
        }
    }

    public void simulate()
    {
        simulateGravity();
    }

    void simulateGravity()
    {
        //Gravity
        rb.mass = core.GetComponent<Rocket>().rocketMass;
        double Dist = Vector2.Distance(rb.transform.position, new Vector2((float)planet.GetComponent<DoubleTransform>().x_pos, (float)planet.GetComponent<DoubleTransform>().y_pos));
        Vector3 forceDir = (planet.transform.position - rb.transform.position).normalized;
        Vector3 ForceVector = forceDir * (G * (Mass * rb.mass) / (float)(Dist * Dist));
        Vector3 Thrust = new Vector3(core.GetComponent<Rocket>().currentThrust.x, core.GetComponent<Rocket>().currentThrust.y, 0);
        Vector3 DragVector = new Vector3(0, 0, 0);
        if (Dist - planetRadius < atmoAlt && rb.velocity.magnitude > 20)
        {
            double airPressure = 1 / ((Dist - planetRadius)) * planetDensity;
            double drag = baseCoefficient * airPressure * rb.velocity.magnitude / 2;
            DragVector = -new Vector3(rb.velocity.x, rb.velocity.y, 0) * (float)drag;
        }
        Vector3 ResultVector = (ForceVector + Thrust + DragVector);



        rb.AddForce(ResultVector);
        GetComponent<DoubleTransform>().x_pos = rb.position.x;
        GetComponent<DoubleTransform>().y_pos = rb.position.y;
    }

    void checkManager()
    {
        if (MasterManager == null)
        {
            GameObject MastRef = GameObject.FindGameObjectWithTag("MasterManager");
            if (TimeRef != null)
            {
                MasterManager = MastRef.GetComponent<MasterManager>();
            }
        }

        if (TimeRef == null)
        {
            TimeRef = GameObject.FindGameObjectWithTag("TimeManager");
            if (TimeRef != null)
            {
                TimeManager = TimeRef.GetComponent<TimeManager>();
            }
        }

        if (WorldSaveManager == null)
        {
            WorldSaveManager = GameObject.FindGameObjectWithTag("WorldSaveManager");
        }

        if (stageViewer == null)
        {
            stageViewer = GameObject.FindObjectOfType<StageViewer>();
        }
    }

    public void stageViewerForceCall()
    {
        StageViewer stageViewer1 = GameObject.FindObjectOfType<StageViewer>();
        stageViewer1.fullReset(true);
    }

    void initializeRocket()
    {
        if (initialized == false)
        {
            rb = GetComponent<Rigidbody2D>();
            initialized = true;
        }
    }

    public void updateReferenceBody()
    {
        planets = GameObject.FindGameObjectsWithTag("Planet");
        float bestDistance = Mathf.Infinity;
        GameObject bestPlanet = null;
        GameObject previous = planet;

        GameObject Earth = null;
        GameObject Moon = null;

        foreach (GameObject go in planets)
        {
            UnityEngine.Vector3 distance = go.transform.position - transform.position;
            float distMag = distance.magnitude;

            if (distMag < bestDistance)
            {
                bestDistance = distMag;
                bestPlanet = go;
            }

            if (go.GetComponent<TypeScript>().type == "earth")
            {
                Earth = go;
            }

            if (go.GetComponent<TypeScript>().type == "moon")
            {
                Moon = go;
            }
        }

        if (bestPlanet != null)
        {
            setPlanetProperty(bestPlanet, bestDistance, Earth);

            //If there's a SOI change during timewarp we must be safe and exit timewarp
            //Code HAS to be there 
            if (previous != planet)
            {
                exitTimewarp();
            }
        }
    }

    void exitTimewarp()
    {
        if (this.GetComponent<RocketStateManager>() != null && TimeManager != null)
        {
            this.GetComponent<RocketStateManager>().state = "simulate";
            TimeManager.setScaler(1);
        }
    }

    void setPlanetProperty(GameObject bestPlanet, float bestDistance, GameObject Earth)
    {
        if (bestPlanet.GetComponent<TypeScript>().type == "earth")
        {
            Mass = SolarSystemManager.earthMass;
            atmoAlt = SolarSystemManager.earthAlt;
            planetDensity = SolarSystemManager.earthDensitySlvl;
            planetRadius = SolarSystemManager.earthRadius;
            planet = bestPlanet;
        }

        if (bestPlanet.GetComponent<TypeScript>().type == "moon" && bestDistance < SolarSystemManager.moonSOI)
        {
            Mass = SolarSystemManager.moonMass;
            atmoAlt = 0.0f;
            aeroCoefficient = 0.0f;
            planetRadius = SolarSystemManager.moonRadius;
            planet = bestPlanet;
        }
        else if ((this.transform.position - Earth.transform.position).magnitude < SolarSystemManager.earthSOI)
        {
            Mass = SolarSystemManager.earthMass;
            atmoAlt = SolarSystemManager.earthAlt;
            aeroCoefficient = SolarSystemManager.earthDensitySlvl;
            planetDensity = SolarSystemManager.earthRadius;
            planet = Earth;

        }
        else
        {
            Mass = SolarSystemManager.sunMass;
            atmoAlt = 0.0f;
            aeroCoefficient = 0.0f;
            planetRadius = SolarSystemManager.sunRadius;
            planet = FindObjectOfType<SunScript>().gameObject;
        }
    }
}


