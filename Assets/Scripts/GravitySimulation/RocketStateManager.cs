using System.Transactions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Callbacks;

public class RocketStateManager : MonoBehaviour
{
    public string state = "landed";
    public string previousState = "landed";
    public bool switched = false;
    public PlanetGravity planetGravity;
    public FloatingOrigin floatingOrigin;
    public FloatingVelocity floatingVelocity;
    public RocketPath prediction;
    public DoubleTransform doublePos;
    public BodySwitcher bodySwitcher;
    public float curr_X = 0f;
    public float curr_Y = 0f;
    public float previous_X = 0f;
    public float previous_Y = 0f;
    public TimeManager MyTime;

    public GameObject savedPlanet;
    public Vector2 savedOffset;

    public bool ran = false;

    public bool initialized = false;

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (GetComponent<PlanetGravity>().getCamera() != null)
        {
            Initialize();
        }
        if (initialized == true)
        {
            StateUpdater();
            UpdatePosition();
            Physics.SyncTransforms();
        }

        ran = false;
    }

    public void Initialize()
    {
        planetGravity = this.GetComponent<PlanetGravity>();
        prediction = this.GetComponent<RocketPath>();
        doublePos = this.GetComponent<DoubleTransform>();
        bodySwitcher = this.GetComponent<BodySwitcher>();
        MyTime = FindObjectOfType<TimeManager>();
        floatingOrigin = FindObjectOfType<FloatingOrigin>();
        floatingVelocity = FindObjectOfType<FloatingVelocity>();
        initialized = true;

    }

    void Update()
    {
        if(ran == false && floatingOrigin.waitingForShipTransfer == false)
        {
            //StateUpdater();
            //UpdatePosition();
            ran = true;
        }
    }


    public void StateUpdater()
    {
        if(curr_X == previous_X && curr_Y == previous_Y && planetGravity.possessed == false && this.planetGravity.rb.velocity.magnitude < 1 && (planetGravity.getCamera().transform.position - transform.position).magnitude >= 200)
        {
            state = "landed";
            if(previousState != state)
            {
                planetGravity.rb.simulated = false;
                savedPlanet = planetGravity.getPlanet();
                this.transform.parent = savedPlanet.transform;
                doublePos.x_pos = this.transform.localPosition.x;
                doublePos.y_pos = this.transform.localPosition.y;
            }
            previousState = state;

            return;
        }

        if((planetGravity.possessed == true || (planetGravity.getCamera().transform.position - transform.position).magnitude < 100) && MyTime.scaler == 1)
        {
            state = "simulate";
            if(previousState != state)
            {   
                planetGravity.rb.simulated = true;
                if(previousState == "rail" && planetGravity.possessed == true)
                {
                    if(planetGravity.rb.velocity.magnitude > planetGravity.velocityThreshold)
                    {
                        planetGravity.storedVelocity = planetGravity.rb.velocity;
                        planetGravity.rb.velocity = planetGravity.rb.velocity.normalized * planetGravity.velocityThreshold;
                        floatingVelocity.velocity = (-planetGravity.storedVelocity.x - planetGravity.rb.velocity.x, -planetGravity.storedVelocity.y - planetGravity.rb.velocity.y);
                        planetGravity.velocityStored = true;
                    }
                }
                if(savedPlanet != null)
                {
                    this.transform.parent = null;
                    doublePos.x_pos = this.transform.position.x;
                    doublePos.y_pos = this.transform.position.y;
                    savedPlanet = null;
                }
                planetGravity.updateReferenceBody();                
            }
            previousState = state;

            return;
        }

        if(((planetGravity.possessed == false || MyTime.scaler != 1) && previousState != "landed" & planetGravity.rb.velocity.magnitude > 1))
        {
            state = "rail";
            if(previousState != state)
            {
                if(savedPlanet != null)
                {
                    this.transform.parent = null;
                    savedPlanet = null;
                }
                planetGravity.updateReferenceBody();
                planetGravity.rb.simulated = false;
                if(previousState == "simulate" && planetGravity.possessed == true)
                {
                    planetGravity.rb.velocity -= new Vector2((float)floatingVelocity.velocity.Item1, (float)floatingVelocity.velocity.Item2);
                    planetGravity.storedVelocity = planetGravity.rb.velocity;
                    floatingVelocity.velocity = (0, 0);
                    planetGravity.velocityStored = false;
                }
                prediction.startTime = prediction.MyTime.time;
                prediction.CalculateParameters();
            }
            previousState = state;
            return;
        }
    }

    public void UpdatePosition()
    {
        if(state == "simulate")
        {
            planetGravity.simulate();
            if(this.GetComponent<Rocket>().throttle > 0)
            {
                prediction.CalculateParameters();
            }
            previous_X = curr_X;
            previous_Y = curr_Y;
            curr_X = this.transform.position.x;
            curr_Y = this.transform.position.y;
            return;
        }

        if(state == "rail")
        {
            Vector2 transform = prediction.updatePosition();
            this.transform.position = transform;
            previous_X = curr_X;
            previous_Y = curr_Y;
            curr_X = transform.x;
            curr_Y = transform.y;
            Vector2 velocity = prediction.updateVelocity();
            if(velocity.x != float.NaN && velocity.y != float.NaN)
            {
                planetGravity.rb.velocity = velocity;
            }
            return;
        }

        if(state == "landed")
        {
            this.transform.localPosition = new Vector3((float)doublePos.x_pos, (float)doublePos.y_pos, 0);
        }
    }
}
