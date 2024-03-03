using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class externalTemperatureController : MonoBehaviour
{
    public container container;
    TypeScript[] Planets;
    SolarSystemManager solarSystemManager;
    // Start is called before the first frame update
    void Start()
    {
        Planets = FindObjectsOfType<TypeScript>();
        solarSystemManager = FindObjectOfType<SolarSystemManager>();
        updateExternalTemp();
        if(container.internalTemperature == 0)
        {
            container.internalTemperature = container.externalTemperature;
        }
    }

    // Update is called once per frame
    void Update()
    {
        updateExternalTemp();
    }

    void updateExternalTemp()
    {
        float distance = Mathf.Infinity;
        TypeScript closestPlanet = null;
        foreach(TypeScript planet in Planets)
        {
            if((planet.transform.position - this.transform.position).magnitude < distance)
            {
                distance = (planet.transform.position - this.transform.position).magnitude;
                closestPlanet = planet;
            }
        }

        if(closestPlanet != null)
        {
            if(closestPlanet.type == "earth")
            {
                if(distance-solarSystemManager.earthRadius < solarSystemManager.earthAlt)
                {
                    container.externalTemperature = 298;
                }else{
                    container.externalTemperature = container.internalTemperature;
                }
                return;
            }

            if(closestPlanet.type == "sun")
            {
                if(distance-solarSystemManager.earthRadius < solarSystemManager.earthAlt)
                {
                    container.externalTemperature = 20000;
                }else{
                    container.externalTemperature = container.internalTemperature;
                }
                return;
            }

            if(closestPlanet.type == "moon")
            {
                if(distance-solarSystemManager.earthRadius < solarSystemManager.earthAlt)
                {
                    container.externalTemperature = 270;
                }else{
                    container.externalTemperature = container.internalTemperature;
                }
                return;
            }
        }
    }
}
