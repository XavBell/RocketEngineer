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
            print("Deploying chute");
            satellite.chuteDeployed = true;
            changeColorNormal();
            satellite.chute.SetActive(true);
            return;
        }
        
        if(satellite.chuteDeployed == false && Vector2.Distance(pg.getPlanet().transform.position, satellite.transform.position) > pg.getPlanetRadius() + pg.getAtmoAlt())
        {
            Debug.Log("You can't deploy the chute in space");
            //TODO add a message to the player
            return;
        }
        
        if(satellite.chuteDeployed == true)
        {
            print("Retracting chute");
            satellite.chuteDeployed = false;
            changeColorNormal();
            satellite.chute.SetActive(false);
            return;
        }
    }

    public void changeColorGreen()
    {
        //satellite.transform.GetChild(2).GetComponent<SpriteRenderer>().color = Color.green;
        satellite.GetComponent<SpriteRenderer>().color = Color.green;
    }

    public void changeColorNormal()
    {
        //satellite.transform.GetChild(2).GetComponent<SpriteRenderer>().color = Color.white;
        satellite.GetComponent<SpriteRenderer>().color = satelliteColor;

    }
}
