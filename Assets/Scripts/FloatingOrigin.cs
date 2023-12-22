using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FloatingOrigin : MonoBehaviour
{
    public float threshold = 0;
    public GameObject sun;
    public GameObject earth;
    public GameObject moon;
    public GameObject customCursor;
    public GameObject Prediction;
    public GameObject Camera;
    private RocketPart[] rps;
    public List<GameObject> planets = new List<GameObject>();
    public TimeManager MyTime;

    // Start is called before the first frame update
    void Start()
    {
        planets.Add(sun);
        planets.Add(earth);
        planets.Add(moon);
    }

    void FixedUpdate()
    {
        updateFloatReference();
        UpdateReferenceBody();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        //updateFloatReference();
    }



    public void updateFloatReference()
    {
        if(Camera.transform.position.magnitude > threshold){
            Vector3 difference = Vector3.zero - Camera.transform.position; 
            for(int z = 0; z < SceneManager.sceneCount; z++)
            {
                foreach (GameObject g in SceneManager.GetSceneAt(z).GetRootGameObjects())
                {
                   UpdatePosition(g, difference);
                }
            }
            
            Prediction.GetComponent<Prediction>().updated = false;
        }
        //Physics.SyncTransforms();

    }

    public void UpdatePosition(GameObject g, Vector3 difference)
    {
        DoubleTransform dt = g.GetComponent<DoubleTransform>();   
        if(dt != null)
        {
            dt.x_pos += difference.x;
            dt.y_pos += difference.y;
            dt.z_pos += difference.z;

            g.transform.position = new Vector3((float)dt.x_pos, (float)dt.y_pos, (float)dt.z_pos);
        }
        if(dt == null)
        {
            g.transform.position += difference;
        }   
    }

    public void PauseRockets()
    {
        Rigidbody2D[] rps = GameObject.FindObjectsOfType<Rigidbody2D>();
        foreach(Rigidbody2D part in rps)
        {
            part.simulated = false;
        }
    }

    public void ActivateRocket()
    {
        Rigidbody2D[] rps = GameObject.FindObjectsOfType<Rigidbody2D>();
        foreach(Rigidbody2D rp in rps)
        {
            rp.simulated = true;
        }
    }

    public void UpdateReferenceBody()
    {
        double bestDistance = Mathf.Infinity;
        GameObject closestPlanet = null;
        foreach(GameObject planet in planets)
        {
            float potentialDistance = Vector2.Distance(Camera.transform.position, planet.transform.position);
            if(potentialDistance < bestDistance)
            {
                bestDistance = potentialDistance;
                closestPlanet = planet;
            }
        }

        if(closestPlanet.GetComponent<TypeScript>().type == "earth")
        {
            Vector2 positionAtTime = closestPlanet.GetComponent<BodyPath>().GetPositionAtTime((float)MyTime.time);
            Vector2 actualPos = closestPlanet.transform.position;
            Vector2 toAdd = actualPos - positionAtTime;

            sun.transform.position = toAdd;
            moon.transform.position = new Vector2(moon.GetComponent<BodyPath>().GetPositionAtTime((float)MyTime.time).x, moon.GetComponent<BodyPath>().GetPositionAtTime((float)MyTime.time).y) + positionAtTime + toAdd;
        }

        if(closestPlanet.GetComponent<TypeScript>().type == "moon")
        {
            Vector2 positionAtTime = closestPlanet.GetComponent<BodyPath>().GetPositionAtTime((float)MyTime.time) + earth.GetComponent<BodyPath>().GetPositionAtTime((float)MyTime.time);
            Vector2 actualPos = closestPlanet.transform.position;
            Vector2 toAdd = actualPos - positionAtTime;

            sun.transform.position = toAdd;
            earth.transform.position = new Vector2(earth.GetComponent<BodyPath>().GetPositionAtTime((float)MyTime.time).x, earth.GetComponent<BodyPath>().GetPositionAtTime((float)MyTime.time).y) + toAdd;
        }

        if(closestPlanet.GetComponent<TypeScript>().type == "sun")
        {
            
        }
    }
}
