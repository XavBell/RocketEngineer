using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public GameObject EarthIcon;
    public GameObject Earth;

    public GameObject SunIcon;
    public GameObject Sun;

    public GameObject MoonIcon;
    public GameObject Moon;

    public Camera mapCam;
    public FloatingOrigin floatingOrigin;
    public List<GameObject> icons = new List<GameObject>();
    public List<PlanetGravity> rockets = new List<PlanetGravity>();
    public List<Prediction> prediction = new List<Prediction>();
    public List<GameObject> paths = new List<GameObject>();
    public bool MapOn = false;
    public GameObject cursor;

    public GameObject predictionPrefab;
    private float lineFactor = 0.01f;

    // Start is called before the first frame update
    void Start()
    {
        mapCam.GetComponent<Camera>();
        floatingOrigin = FindObjectOfType<FloatingOrigin>();
    }

    // Update is called once per frame
    void Update()
    {
        float previousSize = 0;
        if (MapOn == true)
        {   
            if(mapCam.orthographicSize != previousSize)
            {
                updateScale();
            }
            previousSize = mapCam.orthographicSize;
            
        }

    }

    public void updateScale()
    {
        EarthIcon.transform.localScale = (mapCam.orthographicSize * lineFactor * new Vector2(1, 1) * 5)/EarthIcon.transform.parent.localScale.x;
        MoonIcon.transform.localScale = (mapCam.orthographicSize * lineFactor * new Vector2(1, 1) * 5)/MoonIcon.transform.parent.localScale.x;
        SunIcon.transform.localScale = (mapCam.orthographicSize * lineFactor * new Vector2(1, 1) * 5)/SunIcon.transform.parent.localScale.x;

        List<GameObject> iconToRemove = new List<GameObject>();
        foreach (GameObject icon in icons)
        {
            if(icon != null)
            {
                icon.transform.localScale = new Vector2(mapCam.orthographicSize, mapCam.orthographicSize) / 100;
            }else{
                iconToRemove.Add(icon);
            }
        }

        foreach(GameObject icon in iconToRemove)
        {
            icons.Remove(icon);
        }

        foreach (Prediction pred in prediction)
        {
            if(pred != null)
            {
                pred.GetComponent<LineRenderer>().widthMultiplier = mapCam.orthographicSize * lineFactor;
                if (pred.GetComponent<interceptDetector>().interceptIndicator)
                {
                    pred.GetComponent<interceptDetector>().interceptIndicator.transform.localScale = mapCam.orthographicSize * lineFactor * new Vector2(1, 1) * 5;
                }
            }

        }

        foreach (GameObject path in paths)
        {
            path.GetComponent<LineRenderer>().widthMultiplier = mapCam.orthographicSize * lineFactor;
        }

    }

    public void mapOn()
    {
        if (MapOn == false)
        {
            EarthIcon.SetActive(true);
            SunIcon.SetActive(true);
            MoonIcon.SetActive(true);
            PlanetGravity[] planetGravity = FindObjectsOfType<PlanetGravity>();
            foreach (PlanetGravity planetGravity1 in planetGravity)
            {
                GameObject arrow = Instantiate(cursor, planetGravity1.transform);
                arrow.transform.position = planetGravity1.gameObject.transform.position;
                icons.Add(arrow);
                rockets.Add(planetGravity1);

                GameObject prediction1 = Instantiate(predictionPrefab);
                prediction1.GetComponent<Prediction>().planetGravity = planetGravity1;
                prediction.Add(prediction1.GetComponent<Prediction>());

            }

            BodyPath[] bodyPaths = FindObjectsOfType<BodyPath>();
            foreach(BodyPath bodyPath in bodyPaths)
            {
                bodyPath.GetComponent<LineRenderer>().enabled = true;
                paths.Add(bodyPath.GetComponent<LineRenderer>().gameObject);
            }

            soiLineRenderer[] soiLineRenderers = FindObjectsOfType<soiLineRenderer>();
            foreach(soiLineRenderer soi in soiLineRenderers)
            {
                soi.GetComponent<LineRenderer>().enabled = true;
                paths.Add(soi.GetComponent<LineRenderer>().gameObject);
            }
            MapOn = true;
            return;

        }

        if (MapOn == true)
        {
            EarthIcon.SetActive(false);
            SunIcon.SetActive(false);
            MoonIcon.SetActive(false);
            foreach(GameObject icon in icons)
            {
                Destroy(icon);
            }
            icons.Clear();

            foreach(Prediction pred in prediction)
            {
                Destroy(pred.gameObject);
            }

            foreach(GameObject path in paths)
            {
                path.GetComponent<LineRenderer>().enabled = false;
            }

            paths.Clear();
            prediction.Clear();
            MapOn = false;
            return;
        }

    }
}
