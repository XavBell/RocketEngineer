using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class rocketCursorManager : MonoBehaviour
{
    MasterManager masterManager = null;
    GameObject rocket = null;
    // Start is called before the first frame update
    void Start()
    {
        masterManager = FindObjectOfType<MasterManager>();
        rocket = this.transform.parent.gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void clicked()
    {
        Debug.Log("clicked");
        if(masterManager.ActiveRocket != null)
        {
            Debug.Log("found");
            masterManager.ActiveRocket.GetComponent<Rocket>().throttle = 0;
            masterManager.ActiveRocket.GetComponent<PlanetGravity>().possessed = false;
        }

        Debug.Log("here");
        masterManager.ActiveRocket = rocket;
        rocket.GetComponent<PlanetGravity>().possessed = true;
    }
}
