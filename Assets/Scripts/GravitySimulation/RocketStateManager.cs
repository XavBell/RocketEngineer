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
    public float curr_X = 0f;
    public float curr_Y = 0f;
    public float previous_X = 0f;
    public float previous_Y = 0f;

    // Start is called before the first frame update
    void Start()
    {
        planetGravity = this.GetComponent<PlanetGravity>();
        prediction = this.GetComponent<RocketPath>();
        doublePos = this.GetComponent<DoubleTransform>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        StateUpdater();
        UpdatePosition();
    }

    void StateUpdater()
    {
        if(curr_X == previous_X && curr_Y == previous_Y && planetGravity.possessed == false && planetGravity.rb.velocity.magnitude < 1 && previousState == "simulate")
        {
            state = "landed";
            if(previousState != state)
            {
                planetGravity.rb.simulated = true;
            }
            previousState = state;
            return;
        }

        if(planetGravity.possessed == true)
        {
            state = "simulate";
            if(previousState != state)
            {
                planetGravity.rb.simulated = true;
            }
            previousState = state;
            return;
        }

        if(planetGravity.possessed == false && previousState == "simulate")
        {
            state = "rail";
            if(previousState != state)
            {
                planetGravity.rb.simulated = false;
                prediction.CalculateParameters();
            }
            previousState = state;
            return;
        }
    }

    void UpdatePosition()
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
            //prediction.CalculateParameters();
            Vector2 transform = prediction.updatePosition();
            this.transform.position = transform;
            doublePos.x_pos = transform.x;
            doublePos.y_pos = transform.y;
            previous_X = curr_X;
            previous_Y = curr_Y;
            curr_X = transform.x;
            curr_Y = transform.y;
            Vector2 velocity = new Vector2((curr_X-previous_X)/Time.deltaTime, (curr_Y-previous_Y)/Time.deltaTime);
            planetGravity.rb.velocity = velocity;
        }
    }
}
