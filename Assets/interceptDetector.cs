using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class interceptDetector : MonoBehaviour
{
    public Prediction prediction;
    private TimeManager timeManager;

    //Temporary planet variable
    public GameObject Earth;
    public GameObject Moon;
    public GameObject Sun;
    public BodyPath EarthPath;
    public BodyPath MoonPath;

    //Planet variable
    public GameObject activeBodyGO;
    public GameObject targetBodyGO;
    public BodyPath activeBodyBP;
    public BodyPath targetBodyBP;

    //Variables for algorithm
    public float endTimeStep = 2; //If the timestep is at or below that, stops iterating

    public float moonSOI = 600000f; //TO BE REMOVED

    public GameObject indicatorPrefab;
    public GameObject interceptIndicator = null;   

    // Start is called before the first frame update
    void Start()
    {
        prediction = GetComponent<Prediction>();

        timeManager = FindObjectOfType<TimeManager>();

        Earth = FindObjectOfType<EarthScript>().gameObject;
        Moon = FindObjectOfType<MoonScript>().gameObject;
        Sun = FindObjectOfType<SunScript>().gameObject;

        EarthPath = Earth.GetComponent<BodyPath>();
        MoonPath = Moon.GetComponent<BodyPath>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
       
    }

    //Idea is to get 4 times on target body orbit and then check which one is the closer, taking in account time
    //FirstVersion only work if rocket is in orbit of Earth toward Moon
    public void DetectIntercept()
    {
        double orbitalPeriod = MoonPath.orbitalPeriod;
        double startTime = timeManager.time;
        double endTime = orbitalPeriod + startTime;
        List<double> timeToCheck = new List<double>();

        //Initial timestep
        int numberOfPointsPerStep = 4;
        double initialTimeStep = (endTime - startTime)/numberOfPointsPerStep;
        int nIterations = 0;

        float foundDistance = 0;
        double foundTime = 0;

        //Add initial points to check to list
        for(int i = 0; i < numberOfPointsPerStep; i++)
        {
            timeToCheck.Add(i*initialTimeStep + startTime);
        }

        while(initialTimeStep >= endTimeStep)
        {
            List<float> distances = new List<float>();
            foreach(double time in timeToCheck)
            {
                //Since both pos are in reference to Earth no need to add Earth pos at time, might be needed when dealing with several orbits tho
                Vector2 moonPos = MoonPath.GetPositionAtTime(time);
                Vector2 rocketPos = prediction.GetPositionAtTime(time);
                nIterations += 2;

                float distance = Vector3.Distance(moonPos, rocketPos);
                if(initialTimeStep < 100)
                {
                    print(distance);
                }
                distances.Add(distance);
            }

            bool found = false;
            foreach(float distance in distances)
            {
                if(distance < moonSOI)
                {
                    foundDistance = distance;
                    found = true;
                    break;
                }
            }

            if(found == false)
            {
                foundDistance = distances.OrderBy(x => Math.Abs(moonSOI - x)).First();
            }
            
            int timeIndex = distances.IndexOf(foundDistance);
            foundTime = timeToCheck[timeIndex];
            startTime = foundTime - initialTimeStep;
            endTime = foundTime + initialTimeStep;

            initialTimeStep = (endTime - startTime)/numberOfPointsPerStep;

            timeToCheck.Clear();

            //Add initial points to check to list
            for(int i = 0; i < numberOfPointsPerStep; i++)
            {
                timeToCheck.Add(i*initialTimeStep + startTime);
            }
        }

        if(foundDistance <= moonSOI + 500)
        {
            if (interceptIndicator != null)
            {
                interceptIndicator.transform.position = prediction.GetPositionAtTime(foundTime) + prediction.planetGravity.getPlanet().transform.position;
            }
            else
            {
                interceptIndicator = Instantiate(indicatorPrefab, this.transform);
                interceptIndicator.transform.position = prediction.GetPositionAtTime(foundTime) + prediction.planetGravity.getPlanet().transform.position;
            }
        }else{
            Destroy(interceptIndicator);
        }

        
    }
}
