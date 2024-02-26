using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using System.Linq;


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
    private pointManager pointManager;

    public SolarSystemManager SolarSystemManager;
    private Camera cam;
    public Camera getCamera()
    {
        return cam;
    }

    // Start is called before the first frame update
    void Start()
    {
        SolarSystemManager = FindObjectOfType<SolarSystemManager>();
        rb = GetComponent<Rigidbody2D>();
        core.GetComponent<Rocket>().updateMass();
        pointManager = FindObjectOfType<pointManager>();
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
            gainPoints();
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
            MasterManager.ActiveRocket = core; //Line should be able to be removed
            core.GetComponent<Rocket>().controlThrust();
            core.GetComponent<Rocket>()._orientation();
        }
        gainPoints();
    }

    //To be called from RocketStateManager
    public void simulate()
    {
        simulateGravity();
    }

    //Gain points if rocket is active
    void gainPoints()
    {
        if(rb.velocity.magnitude > 20)
        {
            pointManager.nPoints += 0.1f*TimeManager.deltaTime;
        }
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
        if(Mathf.Abs(ResultVector.x) != Mathf.Infinity || Mathf.Abs(ResultVector.y) != Mathf.Infinity )
        {
            rb.AddForce(ResultVector);
        }
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
        GameObject previous = planet;
        setPlanetProperty();

        //If there's a SOI change during timewarp we must be safe and exit timewarp
        //Code HAS to be there 
        if (previous != planet)
        {
            exitTimewarp();
        }
    }

    void exitTimewarp()
    {
        if (this.GetComponent<RocketStateManager>() != null && TimeManager != null)
        {
            TimeManager.setScaler(1);
        }
    }

    void setPlanetProperty()
    {
        if(Vector2.Distance(rb.position, FindObjectOfType<MoonScript>().gameObject.transform.position) < SolarSystemManager.moonSOI)
        {
            Mass = SolarSystemManager.moonMass;
            atmoAlt = 0.0f;
            aeroCoefficient = 0.0f;
            planetRadius = SolarSystemManager.moonRadius;
            planet = FindObjectOfType<MoonScript>().gameObject;
            return;
        }
        if (Vector2.Distance(rb.position, FindObjectOfType<EarthScript>().gameObject.transform.position) < SolarSystemManager.earthSOI)
        {
            Mass = SolarSystemManager.earthMass;
            atmoAlt = SolarSystemManager.earthAlt;
            aeroCoefficient = SolarSystemManager.earthDensitySlvl;
            planetRadius = SolarSystemManager.earthRadius;
            planet =  FindObjectOfType<EarthScript>().gameObject;
            return;
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


