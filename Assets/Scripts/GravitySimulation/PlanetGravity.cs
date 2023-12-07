using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.SceneManagement;
using System.Linq;


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

    public float time = 0;

    public SolarSystemManager SolarSystemManager;
    public DoubleTransform dt;
    public Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        WorldSaveManager = GameObject.FindGameObjectWithTag("WorldSaveManager");
        SolarSystemManager = GameObject.FindObjectOfType<SolarSystemManager>();
        rb = GetComponent<Rigidbody2D>();
        core.GetComponent<Rocket>().updateMass();
        rb.mass = core.GetComponent<Rocket>().rocketMass;
        if(SceneManager.GetActiveScene().name == "SampleScene")
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
            

            if(landed == true)
            {
                rb.simulated = false;
            }

            if(possessed == true)
            {
                rb.simulated = true;
                MasterManager.ActiveRocket = core;
                core.GetComponent<Rocket>().controlThrust();
                core.GetComponent<Rocket>()._orientation();
                core.GetComponent<Rocket>().updateRocketStaging();
            }

            if((cam.transform.position - this.transform.position).magnitude < 1000)
            {
                simulateGravity();
            }

            if(possessed == false && this.rb.velocity.magnitude== 0)
            {
                rb.simulated = false;
            }
            
            
        }


    }

    void simulateGravity()
    {
        updateReferenceBody();
        
        //Gravity
        rb.mass = core.GetComponent<Rocket>().rocketMass;
        float Dist = UnityEngine.Vector2.Distance(rb.transform.position, planet.transform.position);
        UnityEngine.Vector3 forceDir = (planet.transform.position - rb.transform.position).normalized;
        UnityEngine.Vector3 ForceVector = forceDir * (G*((Mass*rb.mass)/ (Dist * Dist)));
        UnityEngine.Vector3 Thrust = new UnityEngine.Vector3(core.GetComponent<Rocket>().currentThrust.x, core.GetComponent<Rocket>().currentThrust.y, 0);
        UnityEngine.Vector3 ResultVector = (ForceVector + Thrust);
        rb.AddForce(ResultVector);
        this.GetComponent<DoubleTransform>().x_pos = rb.position.x;
        this.GetComponent<DoubleTransform>().y_pos = rb.position.y;
    }

    void checkManager()
    {
        if(MasterManager == null)
        {
            GameObject MastRef = GameObject.FindGameObjectWithTag("MasterManager");
            if(TimeRef != null)
            {
                MasterManager = MastRef.GetComponent<MasterManager>();
            }
        }

        if(TimeRef == null)
        {
            TimeRef = GameObject.FindGameObjectWithTag("TimeManager");
            if(TimeRef != null)
            {
                TimeManager = TimeRef.GetComponent<TimeManager>();
            }
        }

        if(WorldSaveManager == null)
        {
            WorldSaveManager = GameObject.FindGameObjectWithTag("WorldSaveManager");
        }
    }

    void initializeRocket()
    {
        if (initialized == false)
        {       
            //transform.localScale = new UnityEngine.Vector3(0.5f, 0.5f, 0.5f);
            rb = GetComponent<Rigidbody2D>();
            initialized = true;
        }
    }

    void updateReferenceBody()
    {
        planets = GameObject.FindGameObjectsWithTag("Planet");
        float bestDistance = Mathf.Infinity;
        GameObject bestPlanet = null;
        
        GameObject Earth = null;
        GameObject Moon = null;

        foreach(GameObject go in planets)
        {
            UnityEngine.Vector3 distance = go.transform.position - transform.position;
            float distMag = distance.magnitude;

            if(distMag < bestDistance)
            {
                bestDistance = distMag;
                bestPlanet = go;
            }

            if(go.GetComponent<TypeScript>().type == "earth")
            {
                Earth = go;
            }

            if(go.GetComponent<TypeScript>().type == "moon")
            {
                Moon = go;
            }
        }

        if (bestPlanet.GetComponent<TypeScript>().type == "earth")
        {
            Mass = bestPlanet.GetComponent<EarthScript>().earthMass;
            atmoAlt = 0.0f;
            aeroCoefficient = 0.0f;
            planetRadius = bestPlanet.GetComponent<EarthScript>().earthRadius;
            planet = bestPlanet;
        }

        if(bestPlanet.GetComponent<TypeScript>().type == "moon" && bestDistance < 6700)
        {
            Mass = bestPlanet.GetComponent<MoonScript>().moonMass;
            atmoAlt = 0.0f;
            aeroCoefficient = 0.0f;
            planetRadius = bestPlanet.GetComponent<MoonScript>().moonRadius;
            planet = bestPlanet;
        }else if(Earth != null) {
            bestPlanet = Earth;
            Mass = bestPlanet.GetComponent<EarthScript>().earthMass;
            atmoAlt = 0.0f;
            aeroCoefficient = 0.0f;
            planetRadius = bestPlanet.GetComponent<EarthScript>().earthRadius;
            planet = bestPlanet;
        }

    }
}
