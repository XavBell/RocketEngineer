using System.Buffers.Text;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class tankParticleController : MonoBehaviour
{
    public outputInputManager outputInputManager;
    public GameObject particle;
    public float baseRateTime = 5;
    public float baseRateDistance = 50;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(((outputInputManager.state == "liquid" && outputInputManager.internalTemperature < 280) || outputInputManager.state == "gas") && outputInputManager.substance != "none" && outputInputManager.mass != 0)
        {
            var em = particle.GetComponent<ParticleSystem>().emission;
            em.rateOverTime = baseRateTime;
            em.rateOverDistance = baseRateDistance;

        }else{
            var em = particle.GetComponent<ParticleSystem>().emission;
            em.rateOverTime = 0;
            em.rateOverDistance = 0;

        }
    }
}
