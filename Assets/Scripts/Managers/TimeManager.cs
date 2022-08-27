using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    //TimeScale
    float initialTimeScale;
    float initialTimeFixedScale;

    public float scaler = 1;

    // Start is called before the first frame update
    void Start()
    {
        initialTimeScale = Time.timeScale;
        initialTimeFixedScale = Time.fixedDeltaTime;
    }

    // Update is called once per frame
    void Update()
    {
        timeScale(initialTimeScale);
    }

    void timeScale(float initial)
    {
        if (Input.GetKey(KeyCode.P))
        {
            //scaler = 100;
        }
        if (Input.GetKey(KeyCode.O))
        {
            Time.fixedDeltaTime = 0.02f*50;
        }
        if (Input.GetKey(KeyCode.I))
        {
            Time.fixedDeltaTime = 0.02f;
        }
    }
}
