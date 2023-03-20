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
    public float Mass = 5927000000000000000000.0f; //Planet mass in kg
    private float G = 0.0000000000667f; //Gravitational constant
    public float atmoAlt = 70.0f;
    public float aeroCoefficient = 5f;
    public float planetRadius = 6371;
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
    public GameObject EngineColliderDetector;

    public bool stageUpdated = false;
    public float capsuleInitialSizeX;

    public GameObject sun;
    public GameObject WorldSaveManager;

    public bool possessed = false;

    public GameObject TimeRef;
    public TimeManager TimeManager;
    public MasterManager MasterManager;

    public float threshold = 10f;

    public float previousApogee;

    public float time = 0;

    public bool FixedUpdatePassed = false;

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


        if (SceneManager.GetActiveScene().name == "SampleScene" )
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

            if(possessed == true)
            {
                time = time + Time.deltaTime;
                Debug.Log(time);
                if(time > 1)
                { 
                    updateReferenceStage();
                }
                _orientation();
                _thrust();
                updateParticle(thrust, maxThrust);
                updateScene();
                this.GetComponent<outputInputManager>().log = false;
            }

            if(possessed == false)
            {
                this.GetComponent<outputInputManager>().log = true;
            }

            simulateGravity();
        }

        if(FixedUpdatePassed == false)
        {
            FixedUpdatePassed = true;
        }
    }

    void simulateGravity()
    {
        updateReferenceBody();
        //Gravity
        float Dist = Vector2.Distance(transform.position, planet.transform.position);
        float EngineDist = Vector2.Distance(transform.position, planet.transform.position);
        Vector3 forceDir = (planet.transform.position - transform.position).normalized;
        Vector3 ForceVector = forceDir * G * Mass * rocketMass / (Dist * Dist);
        Vector3 Thrust = transform.up * thrust;
        
        //Fake collision
        if(EngineDist <= planetRadius)
        {
            ForceVector = new Vector3(0, 0, 0);
            rb.velocity = new Vector2(0,0);

       

        }

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
        previousRocketPos = transform.position;

    }
   
    void _thrust()
    {
        if(activeEngine != null){
        if(Input.GetKey(KeyCode.LeftShift) && thrust<maxThrust)
        {
            thrust += Time.deltaTime * 1;
        }

        else if (Input.GetKey(KeyCode.LeftControl)&&thrust>0)
        {
            thrust += Time.deltaTime * -1;
        }

        if (Input.GetKey(KeyCode.Z))
        {
            thrust = maxThrust;
        }

        if (Input.GetKey(KeyCode.X))
        {
            thrust = 0;
        }

        if(activeEngine.GetComponent<Part>().fuel <= 0.0f)
        {
            thrust = 0;
        }

        activeEngine.GetComponent<Part>().fuel -= thrust/maxThrust * rate;
        float ratio = currentFuel / maxFuel;
        
        if (ratio < 1.0f && ratio > 0.0f && thrust != 0 && activeEngine != null && rocketMass > 0)
        {
            rocketMass -= (thrust / maxThrust * (rate))/2;
        }

        if(rocketMass < 0.0f)
        {
            rocketMass = 0.01f;
        }
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
        foreach(GameObject go in planets)
        { 
            Vector3 distance = go.transform.position - transform.position;
            float distMag = distance.magnitude;
            if(distMag < bestDistance)
            {
                bestDistance = distMag;
                bestPlanet = go;
            }
        }

        if(bestDistance < 1274200)
        {
            planet = GameObject.FindGameObjectWithTag("Sun");
            Mass = 14639429504700000000000000000.0f;
        }

        if (bestPlanet.GetComponent<TypeScript>().type == "moon" && bestDistance < 1274200)
        {
            Mass = 5456514633570000000.0f;
            atmoAlt = 10.0f;
            aeroCoefficient = 0.0f;
            planetRadius = 127421f;
            planet = bestPlanet;
        }

        if (bestPlanet.GetComponent<TypeScript>().type == "earth" && bestDistance < 10274200)
        {
            Mass = 5927000000000000000000.0f;
            atmoAlt = 157400.0f;
            aeroCoefficient = 0f;
            planetRadius = 6371.0f;
            planet = bestPlanet;
        }

    }

    void updateReferenceStage()
    {
        int x = 0;
        AttachPointScript currentAttach = capsule.GetComponent<Part>().attachBottom;
        if (Input.GetKey(KeyCode.Space) && stageUpdated == true)
        {
            Part[] decouplers;
            decouplers = GameObject.FindObjectsOfType<Part>();
            float bestDist = 0;
            Part bestDecoupler = null;
            
            foreach(Part go1 in decouplers)
            {
                if((go1.transform.position - transform.position).magnitude > bestDist && go1.GetComponent<Part>().type.ToString() == "decoupler")
                {
                    bestDist = (go1.transform.position - transform.position).magnitude;
                    bestDecoupler = go1;
                }
            }
            if (bestDecoupler != null)
            {
                GameObject decouplerToUse = bestDecoupler.attachBottom.GetComponent<AttachPointScript>().referenceBody;
                foreach (Part go2 in decouplers)
                {
                    GameObject referenceGo = go2.attachBottom.GetComponent<AttachPointScript>().referenceBody;
                    if (decouplerToUse == referenceGo.GetComponent<Part>().referenceDecoupler)
                    {
                        Debug.Log("Hello");
                        if(referenceGo.GetComponent<Part>().type.ToString() == "tank")
                        {
                            rocketMass -= (referenceGo.GetComponent<Part>().mass - maxFuel);
                        }

                        if (referenceGo.GetComponent<Part>().type.ToString() == "engine")
                        {
                            rocketMass -= referenceGo.GetComponent<Part>().mass;
                        }

                        if(referenceGo != null){
                            Destroy(referenceGo);
                        }
                        
                    }
                }
                //decouplerToUse.GetComponent<Part>().attachTop.GetComponent<AttachPointScript>().attachedBody.GetComponent<Part>().attachBottom.GetComponent<AttachPointScript>().attachedBody = null;
                Destroy(decouplerToUse);
            }
            
            if(capsule.GetComponent<Part>().attachBottom.GetComponent<AttachPointScript>().attachedBody != null){
                stageUpdated = false;
            }
            time = 0;
        }

        
        
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
                                EngineColliderDetector = CurrentEngine;
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

    void updateScene()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            if (SceneManager.GetActiveScene().name == "SampleScene")
            {
                UnityEngine.Object.Destroy(capsule);
                posUpdated = true;
                SceneManager.LoadScene("Building");
                line.SetVertexCount(0);
            }

            if (SceneManager.GetActiveScene().name == "Building")
            {
                Application.Quit();
            }
        }  
    }
}
