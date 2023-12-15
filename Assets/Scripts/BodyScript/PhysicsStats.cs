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
    public double mass = 0;
    // Start is called before the first frame update
    void Start()
    {
        x_pos = this.transform.position.x;
        y_pos = this.transform.position.y;
    }
    void Update()
    {
        if(mass == 0)
        {
            if(body == "moon")
            {
                mass = solarSystemManager.moonMass;
            }

            if(body == "earth")
            {
                mass = solarSystemManager.earthMass;
            }

            if(GetComponent<BodyPath>() != null)
            {
                GetComponent<BodyPath>().calculate = true;
            }
        }
        
        
    }

}
