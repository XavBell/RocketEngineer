using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoonScript : MonoBehaviour
{
    public GameObject Earth;
    public GameObject TimeRef;
    public TimeManager TimeManager;

    public GameObject blockCollider;
    public GameObject moon;

    private float G; //Gravitational constant
    public float gSlvl;
    public float moonMass = 0f;
    public float moonRadius;
    public SolarSystemManager SolarSystemManager;
    
    void Start()
    {
        SolarSystemManager = gameObject.AddComponent<SolarSystemManager>() as SolarSystemManager;SolarSystemManager = gameObject.AddComponent<SolarSystemManager>() as SolarSystemManager;
    }

    // Update is called once per frame
    void Update()
    {
        //Vector3 rotateAxis = Earth.transform.forward.normalized;
        //transform.RotateAround(Earth.transform.position, rotateAxis, 0.000000784f * Time.deltaTime);
    }

    public void InitializeMoon()
    {
        SetMoonMass();
        DrawCircle(1000, moonRadius);
        
    }

    void DrawCircle(int steps, float radius)
    {
        List<Vector2> edges = new List<Vector2>();
        Vector3 previousPos = new Vector3(0f, 0f, 0f);
        GameObject previous = null;

        for(int currentStep = 0; currentStep<steps; currentStep++)
        {
            float circumferenceProgress = (float)currentStep/steps;

            float currentRadian = circumferenceProgress * 2 * Mathf.PI;

            float xScaled = Mathf.Cos(currentRadian);
            float yScaled = Mathf.Sin(currentRadian);

            float x = xScaled * radius;
            float y = yScaled * radius;

            Vector3 currentPosition = new Vector3(x, y, 0) + moon.transform.position;
            edges.Add(currentPosition);
            
            GameObject current = Instantiate(blockCollider, currentPosition - new Vector3(0f, .5f, 0f), Quaternion.Euler(0f, 0f, 0f));

            if(currentStep == 0)
            {
                previous = current;
            }

            Vector2 v = moon.transform.position - current.transform.position;
            float lookAngle =  90 + Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
            current.transform.rotation = Quaternion.Euler(0f, 0f, lookAngle);

            if(currentStep != 0)
            {
                float newX = (current.transform.position - previousPos).magnitude;
                current.transform.localScale = new Vector3(newX, current.transform.localScale.y, current.transform.localScale.z);

                if(currentStep == 1)
                {
                    previous.transform.localScale = new Vector3(newX, previous.transform.localScale.y, previous.transform.localScale.z);
                }
            }

            previousPos = current.transform.position;

            current.transform.SetParent(moon.transform);
        }

    }

    void SetMoonMass()
    {
        GetValues();
        moonMass = gSlvl*(moonRadius*moonRadius)/G;
    }

    void GetValues()
    {
        G = SolarSystemManager.G;
        moonRadius = SolarSystemManager.moonRadius;
        gSlvl = SolarSystemManager.moongSlvl;
    }

}
