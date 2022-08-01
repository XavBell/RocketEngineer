using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.SceneManagement;

public class PlanetGravity : MonoBehaviour
{

    public GameObject[] planets;
    public bool posUpdated;
    public GameObject capsule;
    //Gravity variables for Earth
    public GameObject planet;
    public float Mass = 90000000000.0f; //Planet mass in kg
    private float G = 0.0000000000667f; //Gravitational constant
    public float atmoAlt = 70.0f;
    public float aeroCoefficient = 5f;
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

    GameObject activeEngine;
    public float[] maxThrusts;
    public GameObject[] engines;

    bool stageUpdated = false;
    public float capsuleInitialSizeX;

    public GameObject sun;
    public GameObject WorldSaveManager;

    public bool possessed = false;

    // Start is called before the first frame update
    void Start()
    {
        WorldSaveManager = GameObject.FindGameObjectWithTag("WorldSaveManager");
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(WorldSaveManager == null)
        {
            WorldSaveManager = GameObject.FindGameObjectWithTag("WorldSaveManager");
        }

        //return;
        if (SceneManager.GetActiveScene().name == "SampleScene" && posUpdated == false)
        {
            //Set initial position and scale of rocket when it enters the world
            transform.position = new Vector3(1 , 51, 1);
            transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            rb = GetComponent<Rigidbody2D>();
            posUpdated = true;
        }

        if (SceneManager.GetActiveScene().name == "SampleScene" && WorldSaveManager.GetComponent<WorldSaveManager>().loaded == true)
        {
            

            updateReferenceBody();
            //Gravity
            float Dist = Vector3.Distance(transform.position, planet.transform.position);
            Vector3 forceDir = (planet.transform.position - transform.position).normalized;
            Vector3 ForceVector = forceDir * G * Mass * rocketMass / (Dist * Dist);

            Vector3 Thrust = transform.up * thrust;
            if (Dist < atmoAlt)
            {
                AeroForces = rb.velocity.normalized * rb.velocity.magnitude * aeroCoefficient * -1;
            }
            else
            {
                AeroForces = Vector3.zero;
            }

            Vector3 ResultVector = ForceVector + Thrust + AeroForces;
            rb.AddForce(ResultVector);

            if(possessed == true)
            {
                updateReferenceStage();
                _orientation();
                _thrust();
                updateParticle(thrust, maxThrust);
                updateScene();

                //Prediction
                Vector3 currentPos = rb.position;
                Vector3 prevPos = currentPos;
                Vector3 currentVelocity = rb.velocity;
                Vector3 planetCords = planet.transform.position;
                int stepCount = 15000;
                line.positionCount = stepCount;
                for (int i = 0; i < stepCount; i++)
                {
                    Vector3 distance = planetCords - currentPos;
                    forceDir = (planet.transform.position - currentPos).normalized;
                    ForceVector = forceDir * G * Mass * rocketMass / (distance.magnitude * distance.magnitude);
                    currentVelocity += ForceVector * Time.deltaTime;
                    currentPos += currentVelocity * Time.deltaTime;
                    prevPos = currentPos;
                    line.SetPosition(i, prevPos);
                }
            }

            
            
        }
    }



    void _thrust()
    {
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

        if(currentFuel <= 0.0f)
        {
            thrust = 0;
        }

        currentFuel -= thrust/maxThrust * rate;
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

        if(bestDistance > 500)
        {
            planet = GameObject.FindGameObjectWithTag("Sun");
            Mass = 4000000000000.0f;
        }

        if (bestPlanet.GetComponent<TypeScript>().type == "moon" && bestDistance < 500)
        {
            Mass = 900000000.0f;
            atmoAlt = 10.0f;
            aeroCoefficient = 5f;
            planet = bestPlanet;
        }

        if (bestPlanet.GetComponent<TypeScript>().type == "earth" && bestDistance < 500)
        {
            Mass = 500000000000.0f;
            atmoAlt = 70.0f;
            aeroCoefficient = 0.05f;
            planet = bestPlanet;
        }

    }

    void updateReferenceStage()
    {
        int x = 0;
        AttachPointScript currentAttach = capsule.GetComponent<Part>().attachBottom;
        if (Input.GetKey(KeyCode.Space))
        {
            Part[] decouplers;
            decouplers = GameObject.FindObjectsOfType<Part>();
            float bestDist = 0;
            Part bestDecoupler = null;
            foreach(Part go1 in decouplers)
            {
                if((go1.transform.position - transform.position).magnitude > bestDist && go1.GetComponent<Part>().type.ToString() == "decoupler")
                {
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
                        
                        if(referenceGo.GetComponent<Part>().type.ToString() == "tank")
                        {
                            rocketMass -= (referenceGo.GetComponent<Part>().mass - maxFuel);
                            capsule.GetComponent<BoxCollider2D>().size += new Vector2(capsuleInitialSizeX - referenceGo.GetComponent<Part>().boxSize.x, referenceGo.GetComponent<Part>().boxSize.y) * -1;
                            capsule.GetComponent<BoxCollider2D>().offset += new Vector2(0, (referenceGo.GetComponent<Part>().offsets.y));

                        }

                        if (referenceGo.GetComponent<Part>().type.ToString() == "engine")
                        {
                            rocketMass -= referenceGo.GetComponent<Part>().mass;
                            capsule.GetComponent<BoxCollider2D>().size -= new Vector2(capsuleInitialSizeX - referenceGo.GetComponent<Part>().boxSize.x, referenceGo.GetComponent<Part>().boxSize.y);
                            capsule.GetComponent<BoxCollider2D>().offset -= new Vector2(0, (referenceGo.GetComponent<Part>().offsets.y));

                        }
                        UnityEngine.Object.Destroy(referenceGo);
                        
                    }
                }
                bestDecoupler.GetComponent<Part>().attachTop.GetComponent<AttachPointScript>().attachedBody.GetComponent<Part>().attachBottom.GetComponent<AttachPointScript>().attachedBody = null;
                UnityEngine.Object.Destroy(decouplerToUse);
                stageUpdated = false;
            }
            

        }

        
        
      while (x == 0)
      {
            if (currentAttach.attachedBody != null)
            {
                
                currentAttach = currentAttach.attachedBody.GetComponent<Part>().attachBottom;
                if(currentAttach.attachedBody != null)
                {
                    if (currentAttach.attachedBody.GetComponent<Part>().type.ToString() == "engine" && currentAttach.attachedBody.GetComponent<Part>().attachBottom.GetComponent<AttachPointScript>().attachedBody == null)
                    {

                        GameObject CurrentEngine = currentAttach.attachedBody;
                        maxThrust = CurrentEngine.GetComponent<Part>().maxThrust;
                        rate = CurrentEngine.GetComponent<Part>().rate;

                        if (currentFuel <= 0)
                        {
                            maxFuel = CurrentEngine.GetComponent<Part>().fuel;
                            if(stageUpdated == false)
                            {
                                activeEngine = CurrentEngine;
                                currentFuel = maxFuel;
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
