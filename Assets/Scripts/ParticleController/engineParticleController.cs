using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class engineParticleController : MonoBehaviour
{
    public GameObject plume;
    public AudioSource EngineSound;
    public float baseRate = 20;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
        updateParticle();
    }

    void updateParticle()
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
                    var em = plume.GetComponent<ParticleSystem>().emission;
                    em.rateOverTime = (throttle*baseRate)/100; 
                    if(!EngineSound.isPlaying)
                    {
                        EngineSound.Play();
                    }
                    if(EngineSound.isPlaying == false)
                    {
                        EngineSound.Play();
                    }
                    if(1 - FindObjectOfType<Camera>().orthographicSize/5000 < 0)
                    {
                        EngineSound.volume = 0;
                    }else if(1 - FindObjectOfType<Camera>().orthographicSize/5000 > 1){
                        EngineSound.volume = 1;
                    }else{
                        EngineSound.volume = 1 - FindObjectOfType<Camera>().orthographicSize/5000;
                    }
                    return;
               }
            }
            
            if(throttle == 0 || this.GetComponent<Engine>().active == false)
            {
                var em = plume.GetComponent<ParticleSystem>().emission;
                em.rateOverTime = 0; 
                if(EngineSound.isPlaying == true)
                {
                    EngineSound.Pause();
                }
            }else if(this.gameObject.transform.parent.GetComponent<Rocket>() != null){
                if(this.gameObject.transform.parent.GetComponent<Rocket>().throttle > 0 && this.GetComponent<Engine>().active == true)
                {
                    var em = plume.GetComponent<ParticleSystem>().emission;
                    em.rateOverTime = (throttle*baseRate)/100;
                    if(EngineSound.isPlaying == false)
                    {
                        EngineSound.Play();
                    }
                    if(1 - FindObjectOfType<Camera>().orthographicSize/5000 < 0)
                    {
                        EngineSound.volume = 0;
                    }else if(1 - FindObjectOfType<Camera>().orthographicSize/5000 > 1){
                        EngineSound.volume = 1;
                    }else{
                        EngineSound.volume = 1 - FindObjectOfType<Camera>().orthographicSize/5000;
                    }

                }
                
            }
        }

        if(SceneManager.GetActiveScene().name != "SampleScene")
        {
            var em = plume.GetComponent<ParticleSystem>().emission;
            em.rateOverTime = 0; 
            if(EngineSound.isPlaying == true)
            {
                EngineSound.Pause();
            }
        }
    }
}
