using System.Transactions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketStateManager : MonoBehaviour
{
    public string state = "landed";
    public string previousState = "landed";
    public bool switched = false;
    public PlanetGravity planetGravity;
    public RocketPath prediction;
    public DoubleTransform doublePos;
    public BodySwitcher bodySwitcher;
    public float curr_X = 0f;
    public float curr_Y = 0f;
    public float previous_X = 0f;
    public float previous_Y = 0f;
    public TimeManager MyTime;

    bool ran = false;

    // Start is called before the first frame update
    void Start()
    {
        planetGravity = this.GetComponent<PlanetGravity>();
        prediction = this.GetComponent<RocketPath>();
        doublePos = this.GetComponent<DoubleTransform>();
        bodySwitcher = this.GetComponent<BodySwitcher>();
        MyTime = FindObjectOfType<TimeManager>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //StateUpdater();
        //UpdatePosition();
        ran = false;
    }

    void LateUpdate()
    {
        if(ran == false)
        {
            StateUpdater();
            UpdatePosition();
            ran = true;
        }
    }


    void StateUpdater()
    {
        if(curr_X == previous_X && curr_Y == previous_Y && planetGravity.possessed == false && planetGravity.rb.velocity.magnitude < 1 && previousState == "simulate" && (planetGravity.getCamera().transform.position - transform.position).magnitude > 100)
        {
            state = "landed";
            if(previousState != state)
            {
                planetGravity.rb.simulated = false;
            }
            previousState = state;
            return;
        }

        if((planetGravity.possessed == true || (planetGravity.getCamera().transform.position - transform.position).magnitude < 100) && MyTime.scaler == 1)
        {
            state = "simulate";
            if(previousState != state)
            {
                planetGravity.updateReferenceBody();
                planetGravity.rb.simulated = true;
                if(this.GetComponent<Rocket>().throttle > 0)
                {
                    prediction.CalculateParameters();
                }
                
            }
            previousState = state;
            return;
        }

        if(previousState == "simulate")
        {
            state = "rail";
            if(previousState != state)
            {
                planetGravity.updateReferenceBody();
                planetGravity.rb.simulated = false;
                prediction.startTime = (float)prediction.MyTime.time;
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
            curr_X = this.transform.position.x;
            curr_Y = this.transform.position.y;
            previous_X = curr_X;
            previous_Y = curr_Y;
            return;
        }

        if(state == "rail")
        {
            //Check for potential collision
            if((this.transform.position - planetGravity.getPlanet().transform.position).magnitude < planetGravity.getPlanetRadius())
            {
                Destroy(planetGravity.gameObject);
            }
            Vector2 transform = prediction.updatePosition();
            this.transform.position = transform;
            doublePos.x_pos = transform.x;
            doublePos.y_pos = transform.y;
            previous_X = curr_X;
            previous_Y = curr_Y;
            curr_X = transform.x;
            curr_Y = transform.y;
            Vector2 velocity = prediction.updateVelocity();
            planetGravity.rb.velocity = velocity;
            return;
        }
    }
}
