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
    public float getAeroCoefficient()
    {
        return aeroCoefficient;
    }
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
    public Camera cam;
    public Camera getCamera()
    {
        return cam;
    }

    // Start is called before the first frame update
    void Start()
    {
        
        
    }

    void FixedUpdate()
    {
        checkManager();

        if (SceneManager.GetActiveScene().name == "SampleScene")
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
        if (Dist - planetRadius < atmoAlt && rb.velocity.magnitude > 5 && aeroCoefficient > 0)
        {
            double airPressure = 5*Math.Pow(Math.E, (-(Dist-planetRadius)*aeroCoefficient));
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
            SolarSystemManager = FindObjectOfType<SolarSystemManager>();
            rb = GetComponent<Rigidbody2D>();
            core.GetComponent<Rocket>().updateMass();
            pointManager = FindObjectOfType<pointManager>();
            rb.mass = core.GetComponent<Rocket>().rocketMass;
            initialized = true;
        }
        
        if (SceneManager.GetActiveScene().name == "SampleScene")
        {
            cam = GameObject.FindObjectOfType<Camera>();
            G = SolarSystemManager.G;
        }

        GetComponent<RocketStateManager>().StateUpdater();

        
    }

    public void updateReferenceBody()
    {
        GameObject previous = planet;
        setPlanetProperty();

        //If there's a SOI change during timewarp we must be safe and exit timewarp
        //Code HAS to be there 

        //For now, unpossessed rocket won't transfer
        //Probably just need to update everyone velocity if a rocket switches of planet but will need more thought
        GameObject moon = FindObjectOfType<MoonScript>().gameObject;
        GameObject earth = FindObjectOfType<EarthScript>().gameObject;
        GameObject sun = FindObjectOfType<SunScript>().gameObject;

        if (previous != planet && possessed == true && previous != null)
        {
            exitTimewarp();

            
            //Earth to Moon
            if (planet == moon && previous == earth)
            {
                Vector3 velocity = moon.GetComponent<BodyPath>().GetVelocityAtTime(TimeManager.time);
                rb.velocity -= new Vector2(velocity.x, velocity.y);
            }

            //Moon to Earth
            if (previous == moon && planet == earth)
            {
                Vector3 velocity = moon.GetComponent<BodyPath>().GetVelocityAtTime(TimeManager.time);
                rb.velocity += new Vector2(velocity.x, velocity.y);
            }

            //Sun to Earth
            if (previous == sun && planet == earth)
            {
                Vector3 velocity = earth.GetComponent<BodyPath>().GetVelocityAtTime(TimeManager.time);
                rb.velocity -= new Vector2(velocity.x, velocity.y);
            }

            //EarthToSun
            if (planet == sun && previous == earth)
            {
                Vector3 velocity = earth.GetComponent<BodyPath>().GetVelocityAtTime(TimeManager.time);
                rb.velocity += new Vector2(velocity.x, velocity.y);
            }
            GetComponent<RocketPath>().CalculateParameters();
        }

        if(previous != planet && possessed == false && previous != null)
        {
            //Earth to Moon
            if (planet == moon && previous == earth)
            {
                Vector3 velocity = moon.GetComponent<BodyPath>().GetVelocityAtTime(TimeManager.time);
                rb.velocity -= new Vector2(velocity.x, velocity.y);
            }

            //Moon to Earth
            if (previous == moon && planet == earth)
            {
                Vector3 velocity = moon.GetComponent<BodyPath>().GetVelocityAtTime(TimeManager.time);
                rb.velocity += new Vector2(velocity.x, velocity.y);
            }

            //Sun to Earth
            if (previous == sun && planet == earth)
            {
                Vector3 velocity = earth.GetComponent<BodyPath>().GetVelocityAtTime(TimeManager.time);
                rb.velocity -= new Vector2(velocity.x, velocity.y);
            }

            //EarthToSun
            if (planet == sun && previous == earth)
            {
                Vector3 velocity = earth.GetComponent<BodyPath>().GetVelocityAtTime(TimeManager.time);
                rb.velocity += new Vector2(velocity.x, velocity.y);
            }
            GetComponent<RocketPath>().CalculateParameters();

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
            aeroCoefficient = 0.0f;
            planetRadius = SolarSystemManager.moonRadius;
            atmoAlt = SolarSystemManager.moonAlt;
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


