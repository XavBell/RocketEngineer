using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngineUIModule : MonoBehaviour
{
    public Engine engine;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void activate()
    {
        engine.active = true;
    }
}
