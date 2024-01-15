using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class engineParticleController : MonoBehaviour
{
    public GameObject plume;
    public float baseRate = 20;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(this.gameObject.transform.parent != null)
        {
            float throttle = this.gameObject.transform.parent.GetComponent<Rocket>().throttle;
            if(throttle == 0)
            {
                var em = plume.GetComponent<ParticleSystem>().emission;
                em.rateOverTime = 0; 
            }else if(this.gameObject.transform.parent.GetComponent<Rocket>().currentThrust.magnitude > 0){
                var em = plume.GetComponent<ParticleSystem>().emission;
                em.rateOverTime = (throttle*baseRate)/100; 
            }
        }
        
    }
}
