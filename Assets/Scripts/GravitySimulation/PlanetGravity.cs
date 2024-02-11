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
    private GameObject[] planets;
    private bool initialized = false;
    private GameObject core;
    public GameObject getCore()
    {
        return core;
    }
    public void setCore(GameObject core)
    {
        this.core = core;
    }

    //Gravity variables for Earth
    private GameObject planet;
    public GameObject getPlanet()
    {
        return planet;
    }
    public void setPlanet(GameObject _planet)
    {
        this.planet = _planet;
    }
    
    private float Mass = 0f; //Planet mass in kg
    public float getMass()
    {
        return Mass;
    }

    private float G = 0f; //Gravitational constant
    private float atmoAlt = 0.0f;
    public float getAtmoAlt()
    {
        return atmoAlt;
    }
    private float aeroCoefficient = 0f;
    private float planetRadius = 0f; //Planet radius in m
    public float getPlanetRadius()
    {
        return planetRadius;
    }
    private float planetDensity = 0f;


    //Rocket variables
    //Leaving rb public bcs SO MANY THINGS are using it
    public Rigidbody2D rb;

    //Leaving rocket mass public too because it is used in another script for increment
    public float rocketMass;
    //Aerodynamic coefficient
    public float baseCoefficient = 0.75f;

    //This is public bcs it makes sense, everything should be able to quickly change that
    public bool possessed = false;
    private bool landed = false;

    private TimeManager TimeManager;
    private MasterManager MasterManager;
    private StageViewer stageViewer;

    public SolarSystemManager SolarSystemManager;
    private Camera cam;
    public Camera getCamera()
    {
        return cam;
    }

    // Start is called before the first frame update
    void Start()
    {
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
            updateReferenceBody();
            if (possessed == true)
            {
                MasterManager.ActiveRocket = core;
                core.GetComponent<Rocket>().updateRocketStaging();
            }
        }
    }

    void Update()
    {
        //Rocket controls
        if (possessed == true)
        {
            MasterManager.ActiveRocket = core;
            core.GetComponent<Rocket>().controlThrust();
            core.GetComponent<Rocket>()._orientation();
        }
    }

    //To be called from RocketStateManager
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
        if (Dist - planetRadius < atmoAlt && rb.velocity.magnitude > 5)
        {
            double airPressure = 5*Math.Pow(Math.E, (-(Dist-planetRadius)*0.006));
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
            if (TimeManager != null)
            {
                MasterManager = FindObjectOfType<MasterManager>();
            }
        }

        if (TimeManager == null)
        {
            TimeManager = FindObjectOfType<TimeManager>();
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

    public void initializeRocket()
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


