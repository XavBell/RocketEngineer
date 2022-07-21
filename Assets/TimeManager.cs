using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    //TimeScale
    float initialTimeScale;
    float initialTimeFixedScale;

    // Start is called before the first frame update
    void Start()
    {
        initialTimeScale = Time.timeScale;
        initialTimeFixedScale = Time.fixedDeltaTime;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        timeScale(initialTimeScale);
    }

    void timeScale(float initial)
    {
        if (Input.GetKey(KeyCode.P))
        {
            Time.timeScale = initialTimeScale * 100;
            Time.fixedDeltaTime = initialTimeFixedScale * 100;
        }
        if (Input.GetKey(KeyCode.O))
        {
            Time.timeScale = initialTimeScale * 2;
            Time.fixedDeltaTime = initialTimeFixedScale * 2;
        }
        if (Input.GetKey(KeyCode.I))
        {
            Time.timeScale = initialTimeScale;
            Time.fixedDeltaTime = initialTimeFixedScale;
        }
    }
}
