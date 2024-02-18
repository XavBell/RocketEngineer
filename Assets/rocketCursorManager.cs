using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rocketCursorManager : MonoBehaviour
{
    MasterManager masterManager = null;
    FloatingOrigin floatingOrigin = null;
    GameObject rocket = null;
    // Start is called before the first frame update
    void Start()
    {
        masterManager = FindObjectOfType<MasterManager>();
        floatingOrigin = FindObjectOfType<FloatingOrigin>();
        rocket = this.transform.parent.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void clicked()
    {
        if(masterManager.ActiveRocket != null)
        {
            masterManager.ActiveRocket.GetComponent<Rocket>().throttle = 0;
            masterManager.ActiveRocket.GetComponent<PlanetGravity>().possessed = false;
        }
        masterManager.ActiveRocket = rocket;
        floatingOrigin.closestPlanet = rocket.GetComponent<PlanetGravity>().getPlanet();
        rocket.GetComponent<PlanetGravity>().possessed = true;
    }
}
