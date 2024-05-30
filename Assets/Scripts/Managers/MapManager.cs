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
    public Camera mainCam;
    public FloatingOrigin floatingOrigin;
    public List<GameObject> icons = new List<GameObject>();
    public List<PlanetGravity> rockets = new List<PlanetGravity>();
    public List<Prediction> prediction = new List<Prediction>();
    public List<GameObject> paths = new List<GameObject>();
    public MasterManager masterManager;
    public bool MapOn = false;
    public GameObject cursor;

    public GameObject predictionPrefab;
    private float lineFactor = 0.01f;
    public const float scaledSpace = 50_000;

    // Start is called before the first frame update
    void Start()
    {
        mapCam.GetComponent<Camera>().enabled = false;
        floatingOrigin = FindObjectOfType<FloatingOrigin>();
        masterManager = FindObjectOfType<MasterManager>();
    }

    // Update is called once per frame
    void Update()
    {
        float previousSize = 0;
        if (MapOn == true)
        {   
            MapView();
            if(mapCam.orthographicSize != previousSize)
            {
                updateScale();
            }
            previousSize = mapCam.orthographicSize;
            
        }

    }

    void FixedUpdate()
    {
        if (MapOn == true)
        {
            updatePosition();
        }
    }

    public void updatePosition()
    {
        EarthIcon.transform.position = Earth.transform.position/scaledSpace;
        MoonIcon.transform.position = Moon.transform.position/scaledSpace;
        SunIcon.transform.position = Sun.transform.position/scaledSpace;

        if(masterManager.ActiveRocket != null)
        {
            mapCam.transform.position = masterManager.ActiveRocket.transform.position/scaledSpace;
        }else{
            mapCam.transform.position = EarthIcon.transform.position;
        }

        List<GameObject> iconToRemove = new List<GameObject>();
        foreach (GameObject icon in icons)
        {
            if(icon != null)
            {
                icon.transform.position = icon.GetComponent<rocketCursorManager>().rocket.transform.position/scaledSpace;
                icon.transform.rotation = icon.GetComponent<rocketCursorManager>().rocket.transform.rotation;
            }else{
                iconToRemove.Add(icon);
            }
        }

        foreach(GameObject icon in iconToRemove)
        {
            icons.Remove(icon);
        }
    }

    public void updateScale()
    {
        if(mapCam.orthographicSize > 10)
        {
            //Fade in planet icon 2
        }

        EarthIcon.transform.position = Earth.transform.position/scaledSpace;
        MoonIcon.transform.position = Moon.transform.position/scaledSpace;
        SunIcon.transform.position = Sun.transform.position/scaledSpace;

        mapCam.transform.position = EarthIcon.transform.position;

        List<GameObject> iconToRemove = new List<GameObject>();
        foreach (GameObject icon in icons)
        {
            if(icon != null)
            {
                Vector3 rot = icon.transform.rotation.eulerAngles;
                //icon.transform.rotation = Quaternion.Euler(0, 0, 0);
                icon.transform.localScale = new Vector2(mapCam.orthographicSize, mapCam.orthographicSize) / 200;
                //icon.transform.rotation = Quaternion.Euler(rot);
                icon.transform.position = icon.GetComponent<rocketCursorManager>().rocket.transform.position/scaledSpace;
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
            path.GetComponent<LineRenderer>().widthMultiplier = mapCam.orthographicSize * lineFactor * 1;
        }

    }

    void MapView()
    {
        if(Input.GetKey(KeyCode.M))
        {
            mapOn();
        }
    }

    public void mapOn()
    {
        if (MapOn == false)
        {
            //Activate Map
            mapCam.GetComponent<Camera>().enabled = true;
            mapCam.GetComponent<zoomCam>().enabled = true;
            mainCam.GetComponent<Camera>().enabled = false;
            mainCam.GetComponent<CameraControl>().enabled = false;
            EarthIcon.SetActive(true);
            SunIcon.SetActive(true);
            MoonIcon.SetActive(true);
            PlanetGravity[] planetGravity = FindObjectsOfType<PlanetGravity>();
            foreach (PlanetGravity planetGravity1 in planetGravity)
            {
                GameObject arrow = Instantiate(cursor);
                //arrow.transform.SetParent(planetGravity1.gameObject.transform, false);
                arrow.transform.localPosition = new Vector3(0, 0, 0);
                icons.Add(arrow);
                rockets.Add(planetGravity1);
                arrow.GetComponent<rocketCursorManager>().rocket = planetGravity1.gameObject;
                GameObject prediction1 = Instantiate(predictionPrefab);
                prediction1.GetComponent<Prediction>().planetGravity = planetGravity1;
                prediction.Add(prediction1.GetComponent<Prediction>());

            }

            BodyPath[] bodyPaths = FindObjectsOfType<BodyPath>();
            foreach(BodyPath bodyPath in bodyPaths)
            {
                if(bodyPath.line != null)
                {
                    bodyPath.line.enabled = true;
                    paths.Add(bodyPath.line.gameObject);
                }
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
            //Turn off Map
            mapCam.GetComponent<Camera>().enabled = false;
            mapCam.GetComponent<zoomCam>().enabled = false;
            mainCam.GetComponent<Camera>().enabled = true;
            mainCam.GetComponent<CameraControl>().enabled = true;
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
