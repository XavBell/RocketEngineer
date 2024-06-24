using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapsuleUIManager : MonoBehaviour
{
    public Satellite satellite;
    public Rocket rocket;
    public PlanetGravity pg;
    public StageViewer stageViewer;
    public Color satelliteColor;
    // Start is called before the first frame update
    void Start()
    {
        stageViewer = FindObjectOfType<StageViewer>();
        satelliteColor = satellite.GetComponent<SpriteRenderer>().color;
        rocket = satellite.GetComponentInParent<Rocket>();
        pg = satellite.GetComponent<PlanetGravity>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void deployChute()
    {
        if(Vector2.Distance(pg.getPlanet().gameObject.transform.position, satellite.transform.position) <= pg.getPlanetRadius() + pg.getAtmoAlt() && satellite.chuteDeployed == false)
        {
            satellite.chuteDeployed = true;
            changeColorNormal();
            satellite.chute.SetActive(true);
            return;
        }
        
        if(satellite.chuteDeployed == false && Vector2.Distance(pg.getPlanet().transform.position, satellite.transform.position) > pg.getPlanetRadius() + pg.getAtmoAlt())
        {
            //TODO add a message to the player
            return;
        }
        
        if(satellite.chuteDeployed == true)
        {
            satellite.chuteDeployed = false;
            changeColorNormal();
            satellite.chute.SetActive(false);
            return;
        }
    }

    public void changeColorGreen()
    {
        satellite.GetComponent<SpriteRenderer>().color = Color.green;
    }

    public void changeColorNormal()
    {
        satellite.GetComponent<SpriteRenderer>().color = satelliteColor;

    }
}
