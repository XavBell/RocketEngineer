using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class chuteController : MonoBehaviour
{
    PlanetGravity grav;
    
    float rocketCD = 0;
    public float chuteCD = 2;
    // Start is called before the first frame update
    void Start()
    {
        grav = this.GetComponentInParent<PlanetGravity>();
        if(grav != null)
        {
            rocketCD = grav.baseCoefficient;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(grav != null)
        {
            if(this.GetComponentInParent<Satellite>().chuteDeployed == true)
            {
                grav.baseCoefficient = chuteCD + rocketCD;
            }
        }
    }
}
