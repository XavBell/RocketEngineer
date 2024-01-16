using System.Threading;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class TimeManager : MonoBehaviour
{
    //TimeScale

    public float scaler = 1;
    public double time = 0;
    public float deltaTime = 0;
    public float normalDeltaTime;
    public TMP_Text date;

    public StageViewer stage;
    public bool bypass = false;

    // Start is called before the first frame update
    void Start()
    {
        calculateDeltaTime();
        if(bypass == false)
        {
            time = Time.time;
        }
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        calculateDeltaTime();
        time += deltaTime;
        calculateDate();
    }

    void calculateDeltaTime()
    {
        deltaTime = Time.fixedDeltaTime * scaler;
    }

    public void setScaler(float desiredScaler)
    {
        scaler = desiredScaler;
    }

    public void calculateDate()
    {

        double year = time/(60*60*24*365);
        double remainingSec = time%(60*60*24*365);
        double month = remainingSec/(60*60*24*30);
        remainingSec = remainingSec%(60*60*24*30);
        double days = remainingSec/(60*60*24);
        remainingSec = remainingSec%(60*60*24);
        double hours = remainingSec/(60*60);
        remainingSec = remainingSec%(60*60);
        double minutes = remainingSec/60;
        remainingSec = remainingSec%60;

        date.text = Math.Truncate(year) + "Y:" + Math.Truncate(month) + "M:" + Math.Truncate(days)+ "D:" + Math.Truncate(hours)+ "H:" + Math.Truncate(minutes)+ "m:" + Math.Truncate(remainingSec) + "s";

    }


}
