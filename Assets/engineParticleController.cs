using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

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
        if(this.gameObject.transform.parent != null && SceneManager.GetActiveScene().name == "SampleScene")
        {
            float throttle = 0;
            if(this.gameObject.transform.parent.GetComponent<Rocket>() != null)
            {
                throttle = this.gameObject.transform.parent.GetComponent<Rocket>().throttle;
            }

            if(this.gameObject.transform.parent.GetComponent<staticFireStandManager>() != null)
            {
               if(this.gameObject.transform.parent.GetComponent<staticFireStandManager>().started == true)
               {
                    throttle = 100;
               }
            }
            
            if(throttle == 0)
            {
                var em = plume.GetComponent<ParticleSystem>().emission;
                em.rateOverTime = 0; 
            }else if(this.gameObject.transform.parent.GetComponent<Rocket>() != null){
                if(this.gameObject.transform.parent.GetComponent<Rocket>().throttle > 0)
                {
                    var em = plume.GetComponent<ParticleSystem>().emission;
                    em.rateOverTime = (throttle*baseRate)/100; 
                }
                
            }
        }else{
            var em = plume.GetComponent<ParticleSystem>().emission;
            em.rateOverTime = 0;
        }
        
    }
}
