using System.Transactions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketStateManager : MonoBehaviour
{
    public string state = "landed";
    public bool switched = false;
    public PlanetGravity planetGravity;
    public RocketPath prediction;
    public DoubleTransform doublePos;
    public float curr_X;
    public float curr_Y;
    public float previous_X;
    public float previous_Y;

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
        UpdatePosition();
    }

    void UpdatePosition()
    {
        if(state == "simulate")
        {
            planetGravity.simulate();
            curr_X = this.transform.position.x;
            curr_Y = this.transform.position.y;
            return;
        }

        if(state == "rail" && switched == false)
        {
            prediction.CalculateParameters();
            planetGravity.rb.simulated = false;
            switched = true;
            return;
        }

        if(state == "rail" && switched == true)
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
            planetGravity.rb.velocity = new Vector2((curr_X-previous_X)/Time.deltaTime, (curr_Y-previous_Y)/Time.deltaTime);
        }
    }
}
