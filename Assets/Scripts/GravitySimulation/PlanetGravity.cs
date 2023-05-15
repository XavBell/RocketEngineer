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
    public bool posUpdated = false;
    public GameObject capsule;
    //Gravity variables for Earth
    public GameObject planet;
    public float Mass = 0f; //Planet mass in kg
    public float G = 0.0000000000667f; //Gravitational constant
    public float atmoAlt = 70.0f;
    public float aeroCoefficient = 5f;
    public float planetRadius = 0f; //Planet radius in m
    float maxAlt;

    
    public LineRenderer line;


    //Rocket variables
    public float rocketMass = 0.0f;
    public float thrust = 0.0f;
    public float maxThrust = 0.0f;
    public float rate = 0.0f;
    public Rigidbody2D rb;
    Vector3 AeroForces;
    public ParticleSystem particle;

    public float currentFuel = 0.0f;
    public float maxFuel = 0.0f;

    public GameObject activeEngine;
    public float[] maxThrusts;
    public GameObject[] engines;

    public bool stageUpdated = false;

    public GameObject sun;
    public GameObject WorldSaveManager;

    public bool possessed = false;

    public GameObject TimeRef;
    public TimeManager TimeManager;
    public MasterManager MasterManager;

    public float threshold = 10f;

    public float previousApogee;

    public float time = 0;

    public Vector3 previousRocketPos;

    // Start is called before the first frame update
    void Start()
    {
        WorldSaveManager = GameObject.FindGameObjectWithTag("WorldSaveManager");
        rb = GetComponent<Rigidbody2D>();
        rb.mass = rocketMass;
    }


    void FixedUpdate()
    {
        checkManager();

        if (SceneManager.GetActiveScene().name == "SampleScene" )
        {

            initializeRocket();

            if(possessed == true)
            {
                stageControl();
                _orientation();
                _thrust();
                updateParticle(thrust, maxThrust);
                updateScene();
                this.GetComponent<outputInputManager>().log = false;
                MasterManager.ActiveRocket = capsule;
            }

            simulateGravity();
        }


    }

    void simulateGravity()
    {
        updateReferenceBody();
        //Gravity
        float Dist = Vector2.Distance(transform.position, planet.transform.position);
        float EngineDist = Vector2.Distance(transform.position, planet.transform.position);
        Vector3 forceDir = (planet.transform.position - transform.position).normalized;
        Vector3 ForceVector = forceDir * ((G * Mass * rocketMass) / (Dist * Dist));
        Vector3 Thrust = transform.up * thrust;
        

        if (Dist < atmoAlt)
        {
            //AeroForces = rb.velocity.normalized  *  1/Dist * aeroCoefficient * -1;
            AeroForces = Vector3.zero;
        }
        else
        {
            AeroForces = Vector3.zero;
        }

        Vector3 ResultVector = (ForceVector + Thrust + AeroForces) * Time.fixedDeltaTime;
        rb.mass = rocketMass;
        rb.AddForce(ResultVector);
    }
   
    void _thrust()
    {
        if(activeEngine != null){
            if(thrust == maxThrust)
            {
                float dF = rate * Time.deltaTime;
                Debug.Log(dF);
                if(activeEngine.GetComponent<Part>().fuel - dF > 0f)
                {
                    activeEngine.GetComponent<Part>().fuel -= dF;
                    rocketMass -= dF;
                    
                }else{
                    rocketMass -= activeEngine.GetComponent<Part>().fuel;
                    activeEngine.GetComponent<Part>().fuel = 0;
                    thrust = 0;
                }
            }


            if (Input.GetKey(KeyCode.Z) && activeEngine.GetComponent<Part>().fuel >= 0.0f)
            {
                thrust = maxThrust;
            }

            if (Input.GetKey(KeyCode.X))
            {
                thrust = 0;
            }       
        }
    }

    void stageControl()
    {
        time = time + Time.deltaTime;
        if(time > 1)
        { 
            updateReferenceStage();
        }   
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
        if (posUpdated == false)
        {
            //Set initial position and scale of rocket when it enters the world
            GameObject[] planetsToMove = GameObject.FindGameObjectsWithTag("Planet");
            foreach(GameObject planet in planetsToMove) {
                if(planet.GetComponent<TypeScript>().type == "earth")
                {
                    transform.position = new Vector3(planet.transform.position.x, planet.transform.position.y + planetRadius, 0);
                }
            }
                
            transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            rb = GetComponent<Rigidbody2D>();
            posUpdated = true;
        }
    }

    void _orientation()
    {
        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(0, 0 , Time.deltaTime*50);
        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(0, 0, Time.deltaTime * -50);
        }
    }

    void updateParticle(float thrust, float maxThrust)
    {
        if(thrust == 0)
        {
            var emission = particle.emission;
            emission.enabled = false;
        }
        else if(thrust > 0)
        {
            float rate = thrust / maxThrust;
            var emission = particle.emission;
            emission.enabled = true;
            emission.rateOverTime = rate*10;
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
            Vector3 distance = go.transform.position - transform.position;
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
            planetRadius = bestPlanet.GetComponent<EarthScript>().earthMass;
            planet = bestPlanet;
        }

        if(bestPlanet.GetComponent<TypeScript>().type == "moon" && bestDistance < 6700)
        {
            Mass = bestPlanet.GetComponent<MoonScript>().moonMass;
            atmoAlt = 0.0f;
            aeroCoefficient = 0.0f;
            planetRadius = bestPlanet.GetComponent<MoonScript>().moonRadius;
            planet = bestPlanet;
            Debug.Log("Moon");
        }else if(Earth != null) {
            bestPlanet = Earth;
            Mass = bestPlanet.GetComponent<EarthScript>().earthMass;
            atmoAlt = 0.0f;
            aeroCoefficient = 0.0f;
            planetRadius = bestPlanet.GetComponent<EarthScript>().earthMass;
            planet = bestPlanet;
        }

    }

    //TODO STRONGLY BAD CODE REDO THAT ASAP
    void updateReferenceStage()
    {
        //Decoupling Logic
        int x = 0;
        AttachPointScript currentAttach = capsule.GetComponent<Part>().attachBottom;
        if (Input.GetKey(KeyCode.Space) && stageUpdated == true)
        {
            Part[] decouplers;
            decouplers = GameObject.FindObjectsOfType<Part>();
            float farthestDecouplerDistance = 0;
            Part bestDecouplerPartRef = null;
            
            //Find Closest Decoupler
            foreach(Part rocketPart in decouplers)
            {
                if((rocketPart.transform.position - transform.position).magnitude > farthestDecouplerDistance && rocketPart.GetComponent<Part>().type.ToString() == "decoupler")
                {
                    farthestDecouplerDistance = (rocketPart.transform.position - transform.position).magnitude;
                    bestDecouplerPartRef = rocketPart;
                }
            }

            //Decouple if decoupler found
            if (bestDecouplerPartRef != null)
            {
                GameObject decouplerToUse = bestDecouplerPartRef.attachBottom.GetComponent<AttachPointScript>().referenceBody;
                if(decouplerToUse.GetComponent<Part>().attachBottom.GetComponent<AttachPointScript>().attachedBody != null)
                {
                    GameObject currentPartChecker = decouplerToUse.GetComponent<Part>().attachBottom.GetComponent<AttachPointScript>().attachedBody;
                    while(currentPartChecker.GetComponent<Part>().attachBottom.GetComponent<AttachPointScript>().attachedBody != null)
                    {
                        GameObject PartToBeDestroyed = currentPartChecker;

                        updateMass(rocketMass, currentPartChecker);
                        currentPartChecker = currentPartChecker.GetComponent<Part>().attachBottom.GetComponent<AttachPointScript>().attachedBody;
                        Destroy(PartToBeDestroyed);

                        if(currentPartChecker.GetComponent<Part>().attachBottom.GetComponent<AttachPointScript>().attachedBody == null)
                        {
                            updateMass(rocketMass, currentPartChecker);
                            Destroy(currentPartChecker);
                        }
                    }
                }
                Destroy(decouplerToUse);
            }
            
            if(capsule.GetComponent<Part>().attachBottom.GetComponent<AttachPointScript>().attachedBody != null){
                stageUpdated = false;
            }
            time = 0;
        }
        
        //TODO move to another function
        //Update Current Engine
        while (x == 0 && capsule.GetComponent<Part>().attachBottom.attachedBody != null)
        { 
            if (currentAttach.GetComponent<AttachPointScript>().attachedBody != null)
            {
                
                currentAttach = currentAttach.attachedBody.GetComponent<Part>().attachBottom;
                if(currentAttach.attachedBody != null)
                {
                    if (currentAttach.attachedBody.GetComponent<Part>().type.ToString() == "engine" && currentAttach.attachedBody.GetComponent<Part>().attachBottom.GetComponent<AttachPointScript>().attachedBody == null)
                    {
                        GameObject CurrentEngine = currentAttach.attachedBody;
                        maxThrust = CurrentEngine.GetComponent<Part>().maxThrust;
                        rate = CurrentEngine.GetComponent<Part>().rate;
                        particle.transform.position = CurrentEngine.transform.position;

                        if (currentFuel <= 0)
                        {
                            maxFuel = CurrentEngine.GetComponent<Part>().maxFuel;
                            if(stageUpdated == false)
                            {
                                activeEngine = CurrentEngine;
                                //currentFuel = maxFuel;
                                stageUpdated = true;
                            }
 
                        }

                        x = 1;
                    }
                }

                if(currentAttach.attachedBody == null)
                {
                    x = 1;
                }
            }


            if (currentAttach == null)
            {
                break;
            }
      }
        
    }

    //TODO Is the mass really updated correctly even with fuel
    void updateMass(float rocketMass, GameObject part)
    {
        if(part.GetComponent<Part>().type.ToString() == "tank")
        {
            rocketMass -= part.GetComponent<Part>().mass;
        }
                
        if (part.GetComponent<Part>().type.ToString() == "engine")
        {
            rocketMass -= part.GetComponent<Part>().mass;
        }
    }

    void updateScene()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            if (SceneManager.GetActiveScene().name == "SampleScene")
            {
                UnityEngine.Object.Destroy(capsule);
                posUpdated = true;
                SceneManager.LoadScene("Building");
                line.positionCount = 0;
            }

            if (SceneManager.GetActiveScene().name == "Building")
            {
                Application.Quit();
            }
        }  
    }
}
