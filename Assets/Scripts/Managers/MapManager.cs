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
        mapCam.GetComponent<Camera>().enabled = false;
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

        EarthIcon.transform.position = Earth.transform.position/1_000_00;
        MoonIcon.transform.position = Moon.transform.position/1_000_00;
        SunIcon.transform.position = Sun.transform.position/1_000_00;

        mapCam.transform.position = EarthIcon.transform.position;

        List<GameObject> iconToRemove = new List<GameObject>();
        foreach (GameObject icon in icons)
        {
            if(icon != null)
            {
                Vector3 rot = icon.transform.rotation.eulerAngles;
                icon.transform.rotation = Quaternion.Euler(0, 0, 0);
                icon.transform.localScale = new Vector2(mapCam.orthographicSize/icon.transform.parent.transform.localScale.x, mapCam.orthographicSize/icon.transform.parent.transform.localScale.y) / 100;
                icon.transform.rotation = Quaternion.Euler(rot);
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
            mapCam.GetComponent<Camera>().enabled = true;
            EarthIcon.SetActive(true);
            SunIcon.SetActive(true);
            MoonIcon.SetActive(true);
            PlanetGravity[] planetGravity = FindObjectsOfType<PlanetGravity>();
            foreach (PlanetGravity planetGravity1 in planetGravity)
            {
                GameObject arrow = Instantiate(cursor);
                arrow.transform.SetParent(planetGravity1.gameObject.transform, false);
                arrow.transform.localPosition = new Vector3(0, 0, 0);
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
            mapCam.GetComponent<Camera>().enabled = false;
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
                if(pred != null)
                {
                    Destroy(pred.gameObject);
                }
                
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
