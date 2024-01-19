using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InformationCalculator : MonoBehaviour
{
    public TextMeshProUGUI altitudeText;
    public PlanetGravity planetGravity;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    //WAS USED FOR DEBUG
    // Update is called once per frame
    void Update()
    {
        if(planetGravity.planet != null)
        {
            //float dist = (planetGravity.planet.transform.position - this.transform.position).magnitude - planetGravity.planetRadius;
            //altitudeText.text = dist.ToString();
        }
        
    }
}
