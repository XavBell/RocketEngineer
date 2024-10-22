using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

public class flightUIManager : MonoBehaviour
{
    public StageViewer stageViewer;
    public MasterManager masterManager;
    public GameObject rocketUI;
    public GameObject progradeUI;
    public FloatingVelocity floatingVelocity;

    public TMP_Text velocity;
    public TMP_Text altitude;
    public RectTransform throttleBar;
    public TMP_Text apopasis;
    public TMP_Text periapsis;
    // Start is called before the first frame update
    void Start()
    {
        masterManager = FindObjectOfType<MasterManager>();
        floatingVelocity = FindObjectOfType<FloatingVelocity>();
    }

    // Update is called once per frame
    void Update()
    {
        if(masterManager.gameState == "Flight" && stageViewer.rocket != null)
        {
            updateUI();
        }
    }

    void updateUI()
    {
        rocketUI.transform.rotation = stageViewer.rocket.transform.rotation;
        Vector3 realVelocity = stageViewer.rocket.GetComponent<Rigidbody2D>().velocity - new Vector2((float)floatingVelocity.velocity.Item1, (float)floatingVelocity.velocity.Item2);
        velocity.text = Mathf.Round(realVelocity.magnitude).ToString() + " m/s";
        altitude.text = Mathf.Round((stageViewer.rocket.transform.position - stageViewer.rocket.GetComponent<PlanetGravity>().getPlanet().transform.position).magnitude - stageViewer.rocket.GetComponent<PlanetGravity>().getPlanetRadius()).ToString() + " m";

        if(Mathf.Round(stageViewer.rocket.GetComponent<Rigidbody2D>().velocity.magnitude) != 0)
        {
            
            float angle = Mathf.Rad2Deg*Mathf.Atan(realVelocity.y/realVelocity.x);
            if(realVelocity.x < 0)
            {
                angle += 180;
            }
            progradeUI.transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        throttleBar.sizeDelta = new Vector2(stageViewer.rocket.GetComponent<Rocket>().throttle*3, throttleBar.sizeDelta.y);

        if(stageViewer.rocket.GetComponent<RocketPath>().KeplerParams.eccentricity < 1)
        {
            RocketPath rp = stageViewer.rocket.GetComponent<RocketPath>();
            apopasis.text = (Math.Round(rp.KeplerParams.semiMajorAxis*(1 + rp.KeplerParams.eccentricity) - rp.GetComponent<PlanetGravity>().getPlanetRadius())).ToString();
            periapsis.text = (Math.Round(rp.KeplerParams.semiMajorAxis*(1 - rp.KeplerParams.eccentricity) - rp.GetComponent<PlanetGravity>().getPlanetRadius())).ToString();
        }else if(stageViewer.rocket.GetComponent<RocketPath>().e >= 1)
        {
            RocketPath rp = stageViewer.rocket.GetComponent<RocketPath>();
            apopasis.text = "Infinity";
            periapsis.text = (Math.Round(rp.a*(1 - rp.e) - rp.GetComponent<PlanetGravity>().getPlanetRadius())).ToString();
        }
        
    }
}
