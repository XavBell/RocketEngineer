using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    //TimeScale

    public float scaler = 1;
    public double time = 0;
    public double normalTime = 0;
    public float deltaTime = 0;
    public float normalDeltaTime;

    public StageViewer stage;

    // Start is called before the first frame update
    void Start()
    {
        calculateDeltaTime();
        calculateNormalDeltaTime();
        time = Time.time;
        normalTime = Time.time;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        calculateDeltaTime();
        calculateNormalDeltaTime();
        normalTime += normalDeltaTime;
        time += deltaTime;
    }

    void calculateDeltaTime()
    {
        deltaTime = Time.fixedDeltaTime * scaler;
    }

    void calculateNormalDeltaTime()
    {
        deltaTime = Time.deltaTime;
    }

    public void setScaler(float desiredScaler)
    {
        scaler = desiredScaler;
    }


}
