using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class chuteController : MonoBehaviour
{
    PlanetGravity grav;
    
    float rocketCD = 0;
    public float chuteCD = 2;
    public GameObject anchor;
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

            if(this.GetComponentInParent<Satellite>().chuteDeployed == false)
            {
                grav.baseCoefficient = rocketCD;
            }

            if(Mathf.Round(grav.GetComponent<Rigidbody2D>().velocity.magnitude) != 0)
            {
            
                float angle = Mathf.Rad2Deg*Mathf.Atan(grav.GetComponent<Rigidbody2D>().velocity.y/grav.GetComponent<Rigidbody2D>().velocity.x);
                if(grav.GetComponent<Rigidbody2D>().velocity.x < 0)
                {
                    angle += 180;
                }
                anchor.transform.rotation = Quaternion.Euler(0, 0, (angle+270)+180);
            }

            if(this.GetComponentInParent<Satellite>().chuteDeployed == true && grav.GetComponent<Rigidbody2D>().velocity.magnitude < 1 && (this.transform.position - grav.getPlanet().transform.position).magnitude <= grav.getPlanetRadius() + grav.getAtmoAlt() && grav.GetComponent<RocketStateManager>().curr_X == grav.GetComponent<RocketStateManager>().previous_X && grav.GetComponent<RocketStateManager>().curr_Y == grav.GetComponent<RocketStateManager>().previous_Y)
            {
                this.GetComponentInParent<Satellite>().chuteDeployed = false;
                this.GetComponentInParent<Satellite>().chute.SetActive(false);
            }
        }
    }
}
