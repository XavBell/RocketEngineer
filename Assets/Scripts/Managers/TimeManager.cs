using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    //TimeScale

    public float scaler = 1;
    public double time = 0;
    public float deltaTime = 0;

    public StageViewer stage;

    // Start is called before the first frame update
    void Start()
    {
        calculateDeltaTime();
        time = Time.time;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        calculateDeltaTime();
        time += deltaTime;
    }

    void calculateDeltaTime()
    {
        deltaTime = Time.fixedDeltaTime * scaler;
    }

    public void setScaler(float desiredScaler)
    {
        scaler = desiredScaler;
    }


}
