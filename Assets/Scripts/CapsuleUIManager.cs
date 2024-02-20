using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapsuleUIManager : MonoBehaviour
{
    public Satellite satellite;
    public Rocket rocket;
    public StageViewer stageViewer;
    public Color satelliteColor;
    // Start is called before the first frame update
    void Start()
    {
        stageViewer = FindObjectOfType<StageViewer>();
        satelliteColor = satellite.GetComponent<SpriteRenderer>().color;
        rocket = satellite.GetComponentInParent<Rocket>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void deployChute()
    {
        satellite.chuteDeployed = true;
        changeColorNormal();
        satellite.chute.SetActive(true);
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
