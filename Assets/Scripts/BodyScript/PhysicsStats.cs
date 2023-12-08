using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsStats : MonoBehaviour
{
    public SolarSystemManager solarSystemManager;
    public string body;
    public double x_pos;
    public double y_pos;
    public double x_vel;
    public double y_vel;
    public double mass;
    // Start is called before the first frame update
    void Start()
    {
        if(body == "moon")
        {
            mass = solarSystemManager.moonMass;
        }

        if(body == "earth")
        {
            mass = solarSystemManager.earthMass;
        }

        x_pos = this.transform.position.x;
        y_pos = this.transform.position.y;
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
