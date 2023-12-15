using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecouplerUIModule : MonoBehaviour
{
    public Decoupler decoupler;
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
        decoupler.activated = true;
    }
}
