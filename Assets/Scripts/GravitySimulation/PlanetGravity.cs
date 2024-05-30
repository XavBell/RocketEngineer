using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using System.Linq;
using Unity.Mathematics;


public class PlanetGravity : MonoBehaviour
{
    private GameObject[] planets;
    public bool velocityStored = false;
    public FloatingVelocity floatingVelocity;
    public float velocityThreshold = 5000;
    public Vector2 storedVelocity;
    private bool initialized = false;
    private GameObject core;
    private GameObject currentSOIPlanet = null;
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


    //Rocket variables
    //Leaving rb public bcs SO MANY THINGS are using it
    public Rigidbody2D rb;

    //Leaving rocket mass public too because it is used in another script for increment
    public float rocketMass;
    //Aerodynamic coefficient
    public float baseCoefficient = 0.75f;

    //This is public bcs it makes sense, everything should be able to quickly change that
    public bool possessed = false;

    private TimeManager TimeManager;
    private MasterManager MasterManager;
    private StageViewer stageViewer;
    private pointManager pointManager;
    public FloatingOrigin floatingOrigin;

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
        if (rb.velocity.magnitude > 20)
        {
            pointManager.nPoints += 0.1f * TimeManager.deltaTime;
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
            double airPressure = 5 * Math.Pow(Math.E, (-(Dist - planetRadius) * aeroCoefficient));
            double drag = baseCoefficient * airPressure * rb.velocity.magnitude / 2;
            DragVector = -new Vector3(rb.velocity.x, rb.velocity.y, 0) * (float)drag;
        }
        Vector3 ResultVector = (ForceVector + Thrust + DragVector);
        if ((Mathf.Abs(ResultVector.x) != Mathf.Infinity || Mathf.Abs(ResultVector.y) != Mathf.Infinity) && storedVelocity.magnitude <= velocityThreshold)
        {
            if (velocityStored == true)
            {
                rb.velocity = storedVelocity;
                floatingVelocity.velocity.Item1 = 0;
                floatingVelocity.velocity.Item2 = 0;
                velocityStored = false;
            }
            rb.AddForce(ResultVector);
            storedVelocity = new Vector2(rb.velocity.x, rb.velocity.y);
        }
        else if (storedVelocity.magnitude > velocityThreshold)
        {
            if (rb.velocity.magnitude > velocityThreshold)
            {
                storedVelocity = rb.velocity;
                rb.velocity = rb.velocity.normalized * velocityThreshold*0.9f;
                floatingVelocity.velocity.Item1 = -(storedVelocity.x - rb.velocity.x);
                floatingVelocity.velocity.Item2 = -(storedVelocity.y - rb.velocity.y);
                velocityStored = true;
            }
            floatingVelocity.velocity.Item1 -= (double)(ResultVector.x / rb.mass * TimeManager.deltaTime);
            floatingVelocity.velocity.Item2 -= (double)(ResultVector.y / rb.mass * TimeManager.deltaTime);
            storedVelocity += new Vector2(ResultVector.x / rb.mass * TimeManager.deltaTime, ResultVector.y / rb.mass * TimeManager.deltaTime);
            velocityStored = true;
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
            floatingOrigin = FindObjectOfType<FloatingOrigin>();
            floatingVelocity = FindObjectOfType<FloatingVelocity>();
            if (core != null)
            {
                core.GetComponent<Rocket>().updateMass();
                pointManager = FindObjectOfType<pointManager>();
                rb.mass = core.GetComponent<Rocket>().rocketMass;
                initialized = true;
            }

        }

        if (SceneManager.GetActiveScene().name == "SampleScene")
        {
            cam = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
            G = SolarSystemManager.G;
        }


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

        if (previous != planet && possessed == true && previous != null && rb.velocity.magnitude != float.NaN)
        {
            if (this.gameObject == MasterManager.ActiveRocket && previous != sun && GetComponent<RocketStateManager>().state == "rail")
            {
                floatingOrigin.corrected = false;
                floatingOrigin.previousPlanet = previous;
                floatingOrigin.UpdateReferenceBody();
                GetComponent<RocketPath>().CalculateParameters();
            }
            exitTimewarp();


            //Earth to Moon
            if (planet == moon && previous == earth)
            {
                Vector3 velocity = moon.GetComponent<BodyPath>().GetVelocityAtTime(TimeManager.time);
                if (!float.IsNaN(velocity.x) && !float.IsNaN(velocity.y))
                {

                    rb.velocity -= new Vector2(velocity.x, velocity.y);
                    storedVelocity -= new Vector2(velocity.x, velocity.y);
                    GetComponent<RocketPath>().CalculateParameters();
                    return;


                }
            }

            //Moon to Earth
            if (previous == moon && planet == earth)
            {
                Vector3 velocity = moon.GetComponent<BodyPath>().GetVelocityAtTime(TimeManager.time);
                if (!float.IsNaN(velocity.x) && !float.IsNaN(velocity.y))
                {

                    rb.velocity += new Vector2(velocity.x, velocity.y);
                    storedVelocity += new Vector2(velocity.x, velocity.y);
                    GetComponent<RocketPath>().CalculateParameters();
                    return;

                }
            }

            //Sun to Earth
            if (previous == sun && planet == earth)
            {
                Vector3 velocity = earth.GetComponent<BodyPath>().GetVelocityAtTime(Math.Round(TimeManager.time));
                if (!float.IsNaN(velocity.x) && !float.IsNaN(velocity.y))
                {

                    rb.velocity -= new Vector2(velocity.x, velocity.y);
                    storedVelocity -= new Vector2(velocity.x, velocity.y);
                    GetComponent<RocketPath>().CalculateParameters();
                    return;

                }
                else
                {
                    Debug.Log("Velocity is NaN");
                }
            }

            //EarthToSun
            if (planet == sun && previous == earth)
            {
                Vector3 velocity = earth.GetComponent<BodyPath>().GetVelocityAtTime(Math.Round(TimeManager.time));
                if (!float.IsNaN(velocity.x) && !float.IsNaN(velocity.y))
                {
                    rb.velocity += new Vector2(velocity.x, velocity.y);
                    storedVelocity += new Vector2(velocity.x, velocity.y);
                    GetComponent<RocketPath>().CalculateParameters();
                    return;
                }
                else
                {
                    Debug.Log("Velocity is NaN");
                }
            }
        }

        if (previous != planet && possessed == false && previous != null && rb.velocity.magnitude != float.NaN)
        {
            //Earth to Moon
            if (planet == moon && previous == earth)
            {
                Vector3 velocity = moon.GetComponent<BodyPath>().GetVelocityAtTime(TimeManager.time);
                if (!float.IsNaN(velocity.x) && !float.IsNaN(velocity.y))
                {

                    rb.velocity -= new Vector2(velocity.x, velocity.y);
                    storedVelocity -= new Vector2(velocity.x, velocity.y);

                }
            }

            //Moon to Earth
            if (previous == moon && planet == earth)
            {
                Vector3 velocity = moon.GetComponent<BodyPath>().GetVelocityAtTime(TimeManager.time);
                if (!float.IsNaN(velocity.x) && !float.IsNaN(velocity.y))
                {

                    rb.velocity += new Vector2(velocity.x, velocity.y);
                    storedVelocity += new Vector2(velocity.x, velocity.y);
                }
            }

            //Sun to Earth
            if (previous == sun && planet == earth)
            {
                Vector3 velocity = earth.GetComponent<BodyPath>().GetVelocityAtTime(TimeManager.time);
                if (!float.IsNaN(velocity.x) && !float.IsNaN(velocity.y))
                {

                    rb.velocity -= new Vector2(velocity.x, velocity.y);
                    storedVelocity -= new Vector2(velocity.x, velocity.y);

                }
            }

            //EarthToSun
            if (planet == sun && previous == earth)
            {
                Vector3 velocity = earth.GetComponent<BodyPath>().GetVelocityAtTime(TimeManager.time);
                if (!float.IsNaN(velocity.x) && !float.IsNaN(velocity.y))
                {

                    rb.velocity += new Vector2(velocity.x, velocity.y);
                    storedVelocity += new Vector2(velocity.x, velocity.y);

                }
            }
            GetComponent<RocketPath>().CalculateParameters();

        }

        if (previous != planet && possessed == true && rb.velocity.magnitude == float.NaN)
        {
            print("Velocity is NaN");
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
        // Define the buffer zone for SOI transitions
        const double earthBuffer = 100000.0;
        const double moonBuffer = 1000.0;

        MoonScript moonScript = FindObjectOfType<MoonScript>();
        EarthScript earthScript = FindObjectOfType<EarthScript>();
        SunScript sunScript = FindObjectOfType<SunScript>();

        // Retrieve the positions of the Moon, Earth, and Sun
        (double, double) moonPosition = (moonScript.GetComponent<DoubleTransform>().x_pos,moonScript.GetComponent<DoubleTransform>().y_pos);
        (double, double) earthPosition = (earthScript.GetComponent<DoubleTransform>().x_pos, FindObjectOfType<EarthScript>().GetComponent<DoubleTransform>().y_pos);
        (double, double) sunPosition = (sunScript.GetComponent<DoubleTransform>().x_pos, FindObjectOfType<SunScript>().GetComponent<DoubleTransform>().y_pos);

        // Calculate distances to the Moon, Earth, and Sun
        double distanceToMoon = Math.Sqrt(Math.Pow(rb.position.x - moonPosition.Item1, 2) + Math.Pow(rb.position.y - moonPosition.Item2, 2));
        double distanceToEarth = Math.Sqrt(Math.Pow(rb.position.x - earthPosition.Item1, 2) + Math.Pow(rb.position.y - earthPosition.Item2, 2));
        double distanceToSun = Math.Sqrt(Math.Pow(rb.position.x - sunPosition.Item1, 2) + Math.Pow(rb.position.y - sunPosition.Item2, 2));

        // Determine the new SOI based on distances and buffer zones
        GameObject newSOIPlanet = null;
        if (distanceToMoon < SolarSystemManager.moonSOI - moonBuffer)
        {
            newSOIPlanet = moonScript.gameObject;
        }
        else if (currentSOIPlanet == moonScript.gameObject && distanceToEarth < SolarSystemManager.earthSOI)
        {
            newSOIPlanet = earthScript.gameObject;
        }else if(currentSOIPlanet == sunScript && distanceToEarth < SolarSystemManager.earthSOI + earthBuffer)
        {
            newSOIPlanet = earthScript.gameObject;
        }
        else if (distanceToEarth > SolarSystemManager.earthSOI)
        {
            newSOIPlanet = sunScript.gameObject;
        }

        // If the SOI has changed, update the properties
        if (newSOIPlanet != currentSOIPlanet)
        {
            currentSOIPlanet = newSOIPlanet;

            if (currentSOIPlanet == moonScript.gameObject)
            {
                // Within Moon's SOI
                Mass = SolarSystemManager.moonMass;
                aeroCoefficient = 0.0f;
                planetRadius = SolarSystemManager.moonRadius;
                atmoAlt = SolarSystemManager.moonAlt;
                planet = moonScript.gameObject;
            }
            else if (currentSOIPlanet == earthScript.gameObject)
            {
                // Within Earth's SOI
                Mass = SolarSystemManager.earthMass;
                aeroCoefficient = SolarSystemManager.earthDensitySlvl;
                planetRadius = SolarSystemManager.earthRadius;
                atmoAlt = SolarSystemManager.earthAlt;
                planet = earthScript.gameObject;
            }
            else
            {
                // Default to Sun's SOI
                Mass = SolarSystemManager.sunMass;
                atmoAlt = 0.0f;
                aeroCoefficient = 0.0f;
                planetRadius = SolarSystemManager.sunRadius;
                planet = sunScript.gameObject;
            }
        }
    }
}


