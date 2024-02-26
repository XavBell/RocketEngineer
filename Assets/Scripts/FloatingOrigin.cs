using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FloatingOrigin : MonoBehaviour
{
    public float threshold = 0;
    public float floatingFPS = 0.1f;
    public GameObject sun;
    public GameObject earth;
    public GameObject moon;
    public GameObject customCursor;
    public GameObject Prediction;
    public GameObject Camera;
    private RocketPart[] rps;
    public List<GameObject> planets = new List<GameObject>();
    public TimeManager MyTime;
    public MasterManager masterManager;
    public MapManager mapManager;
    public StageViewer StageViewer;

    public bool recalculateParameters;

    public GameObject closestPlanet = null;
    public bool bypass = false;
    public bool DO = true;
    public bool LOCK = false;
    public bool fixedRan = false;
    public bool waitingForShipTransfer = false;

    // Start is called before the first frame update
    void Start()
    {
        masterManager = FindObjectOfType<MasterManager>();
        mapManager = FindObjectOfType<MapManager>();
        planets.Add(sun);
        planets.Add(earth);
        planets.Add(moon);
    }

    void FixedUpdate()
    {
        fixedRan = true;
        UpdateReferenceBody();
        updateFloatReference();
    }

    void Update()
    {
        if (bypass == false)
        {
            if (fixedRan == true)
            {
                //UpdateReferenceBody();
            }

            if (DO == true && LOCK == false && fixedRan == true)
            {
                DO = true;
               //updateFloatReference();
            }
        }
        fixedRan = false;
        bypass = false;
    }



    public void updateFloatReference()
    {
        if (Camera.transform.position.magnitude > threshold)
        {

            Vector3 difference = Vector3.zero - Camera.transform.position;
            for (int z = 0; z < SceneManager.sceneCount; z++)
            {
                foreach (GameObject g in SceneManager.GetSceneAt(z).GetRootGameObjects())
                {
                    UpdatePosition(g, difference);
                }
            }

            recalculateParameters = true;

            LineRenderer[] lines = FindObjectsOfType<LineRenderer>();
            foreach (LineRenderer pred in lines)
            {
                if (!pred.GetComponent<soiLineRenderer>())
                {
                    for (int i = 0; i < pred.positionCount; i++)
                    {
                        pred.SetPosition(i, pred.GetPosition(i) + difference);
                    }
                }
            }

            Physics.SyncTransforms();
        }
    
    }

    public void UpdatePosition(GameObject g, Vector3 difference)
    {
        DoubleTransform dt = g.GetComponent<DoubleTransform>();
        if (dt != null)
        {
            dt.x_pos += difference.x;
            dt.y_pos += difference.y;
            dt.z_pos += difference.z;

            g.transform.position = new Vector3((float)dt.x_pos, (float)dt.y_pos, (float)dt.z_pos);
        }
        if (dt == null)
        {
            g.transform.position += difference;
        }

    }

    public void UpdateReferenceBody()
    {
        if (masterManager.ActiveRocket == null)
        {
            double bestDistance = Mathf.Infinity;
            GameObject closestPlanet = null;
            foreach (GameObject planet in planets)
            {
                float potentialDistance = Vector2.Distance(Camera.transform.position, planet.transform.position);
                if (potentialDistance < bestDistance)
                {
                    bestDistance = potentialDistance;
                    closestPlanet = planet;
                }
            }

            if (closestPlanet == earth)
            {
                Vector2 positionAtTime = closestPlanet.GetComponent<BodyPath>().GetPositionAtTime(MyTime.time);
                Vector2 actualPos = closestPlanet.transform.position;
                Vector2 toAdd = actualPos - positionAtTime;
                if (!float.IsNaN(toAdd.x) && !float.IsNaN(toAdd.x))
                {
                    sun.transform.position = toAdd;
                    sun.GetComponent<DoubleTransform>().x_pos = toAdd.x;
                    sun.GetComponent<DoubleTransform>().y_pos = toAdd.y;
                    moon.transform.position = new Vector2(moon.GetComponent<BodyPath>().GetPositionAtTime(MyTime.time).x, moon.GetComponent<BodyPath>().GetPositionAtTime(MyTime.time).y) + positionAtTime + toAdd;
                    moon.GetComponent<DoubleTransform>().x_pos = moon.transform.position.x;
                    moon.GetComponent<DoubleTransform>().y_pos = moon.transform.position.y;
                    earth.GetComponent<BodyPath>().ReDraw();
                    moon.GetComponent<BodyPath>().ReDraw();
                }
            }

            if (closestPlanet == moon)
            {
                Vector2 positionAtTime = closestPlanet.GetComponent<BodyPath>().GetPositionAtTime(MyTime.time) + earth.GetComponent<BodyPath>().GetPositionAtTime(MyTime.time);
                Vector2 actualPos = closestPlanet.transform.position;
                Vector2 toAdd = actualPos - positionAtTime;
                if (!float.IsNaN(toAdd.x) && !float.IsNaN(toAdd.x))
                {
                    sun.transform.position = new Vector2(toAdd.x, toAdd.y);
                    sun.GetComponent<DoubleTransform>().x_pos = toAdd.x;
                    sun.GetComponent<DoubleTransform>().y_pos = toAdd.y;
                    earth.transform.position = new Vector2(earth.GetComponent<BodyPath>().GetPositionAtTime(MyTime.time).x, earth.GetComponent<BodyPath>().GetPositionAtTime(MyTime.time).y) + toAdd;
                    earth.GetComponent<DoubleTransform>().x_pos = earth.transform.position.x;
                    earth.GetComponent<DoubleTransform>().y_pos = earth.transform.position.y;
                    earth.GetComponent<BodyPath>().ReDraw();
                    moon.GetComponent<BodyPath>().ReDraw();
                }
            }

            if (closestPlanet == sun)
            {
                Vector2 positionAtTime = new Vector2(0, 0);
                Vector2 actualPos = closestPlanet.transform.position;
                Vector2 toAdd = actualPos - positionAtTime;
                if (!float.IsNaN(toAdd.x) && !float.IsNaN(toAdd.x))
                {
                    earth.transform.position = new Vector2(earth.GetComponent<BodyPath>().GetPositionAtTime(MyTime.time).x, earth.GetComponent<BodyPath>().GetPositionAtTime(MyTime.time).y) + toAdd;
                    earth.GetComponent<DoubleTransform>().x_pos = earth.transform.position.x;
                    earth.GetComponent<DoubleTransform>().y_pos = earth.transform.position.y;
                    moon.transform.position = new Vector2(moon.GetComponent<BodyPath>().GetPositionAtTime(MyTime.time).x, moon.GetComponent<BodyPath>().GetPositionAtTime(MyTime.time).y) + new Vector2(earth.transform.position.x, earth.transform.position.y);
                    moon.GetComponent<DoubleTransform>().x_pos = moon.transform.position.x;
                    moon.GetComponent<DoubleTransform>().y_pos = moon.transform.position.y;
                    earth.GetComponent<BodyPath>().ReDraw();
                    moon.GetComponent<BodyPath>().ReDraw();
                }


            }
        }

        if (masterManager.ActiveRocket != null)
        {
            bool toRecalculate = false;
            double bestDistance = Mathf.Infinity;
            if (closestPlanet == null)
            {
                closestPlanet = masterManager.ActiveRocket.GetComponent<PlanetGravity>().getPlanet();
            }


            if (closestPlanet != masterManager.ActiveRocket.GetComponent<PlanetGravity>().getPlanet())
            {
                MyTime.setScaler(1);

                bypass = true;
                //Earth to Moon
                if (masterManager.ActiveRocket.GetComponent<PlanetGravity>().getPlanet() == moon && closestPlanet == earth)
                {
                    Vector3 velocity = masterManager.ActiveRocket.GetComponent<PlanetGravity>().getPlanet().GetComponent<BodyPath>().GetVelocityAtTime(MyTime.time);
                    masterManager.ActiveRocket.GetComponent<PlanetGravity>().rb.velocity -= new Vector2(velocity.x, velocity.y);
                }

                //Moon to Earth
                if (closestPlanet == moon && masterManager.ActiveRocket.GetComponent<PlanetGravity>().getPlanet() == earth)
                {
                    Vector3 velocity = closestPlanet.GetComponent<BodyPath>().GetVelocityAtTime(MyTime.time);
                    masterManager.ActiveRocket.GetComponent<PlanetGravity>().rb.velocity += new Vector2(velocity.x, velocity.y);
                }

                //Sun to Earth
                if (closestPlanet == sun && masterManager.ActiveRocket.GetComponent<PlanetGravity>().getPlanet() == earth)
                {
                    Vector3 velocity = earth.GetComponent<BodyPath>().GetVelocityAtTime(MyTime.time);
                    masterManager.ActiveRocket.GetComponent<PlanetGravity>().rb.velocity -= new Vector2(velocity.x, velocity.y);
                }

                //EarthToSun
                if (masterManager.ActiveRocket.GetComponent<PlanetGravity>().getPlanet() == sun && closestPlanet == earth)
                {
                    Vector3 velocity = earth.GetComponent<BodyPath>().GetVelocityAtTime(MyTime.time);
                    masterManager.ActiveRocket.GetComponent<PlanetGravity>().rb.velocity += new Vector2(velocity.x, velocity.y);
                }

                closestPlanet = masterManager.ActiveRocket.GetComponent<PlanetGravity>().getPlanet();
                Prediction[] predictions = FindObjectsOfType<Prediction>();
                foreach (Prediction prediction in predictions)
                {
                    prediction.updated = false;
                }
                toRecalculate = true;
            }

            if (closestPlanet == earth)
            {
                Vector2 positionAtTime = closestPlanet.GetComponent<BodyPath>().GetPositionAtTime(MyTime.time);
                Vector2 actualPos = closestPlanet.transform.position;
                Vector2 toAdd = actualPos - positionAtTime;
                if (!float.IsNaN(toAdd.x) && !float.IsNaN(toAdd.y))
                {
                    sun.transform.position = toAdd;
                    sun.GetComponent<DoubleTransform>().x_pos = toAdd.x;
                    sun.GetComponent<DoubleTransform>().y_pos = toAdd.y;
                    moon.transform.position = new Vector2(moon.GetComponent<BodyPath>().GetPositionAtTime(MyTime.time).x, moon.GetComponent<BodyPath>().GetPositionAtTime(MyTime.time).y) + positionAtTime + toAdd;
                    moon.GetComponent<DoubleTransform>().x_pos = moon.transform.position.x;
                    moon.GetComponent<DoubleTransform>().y_pos = moon.transform.position.y;
                    earth.GetComponent<BodyPath>().ReDraw();
                    moon.GetComponent<BodyPath>().ReDraw();
                }
            }

            if (closestPlanet == moon)
            {
                Vector2 positionAtTime = closestPlanet.GetComponent<BodyPath>().GetPositionAtTime(MyTime.time) + earth.GetComponent<BodyPath>().GetPositionAtTime(MyTime.time);
                Vector2 actualPos = closestPlanet.transform.position;
                Vector2 toAdd = actualPos - positionAtTime;
                if (!float.IsNaN(toAdd.x) && !float.IsNaN(toAdd.y))
                {
                    sun.transform.position = new Vector2(toAdd.x, toAdd.y);
                    sun.GetComponent<DoubleTransform>().x_pos = toAdd.x;
                    sun.GetComponent<DoubleTransform>().y_pos = toAdd.y;
                    earth.transform.position = new Vector2(earth.GetComponent<BodyPath>().GetPositionAtTime(MyTime.time).x, earth.GetComponent<BodyPath>().GetPositionAtTime(MyTime.time).y) + toAdd;
                    earth.GetComponent<DoubleTransform>().x_pos = earth.transform.position.x;
                    earth.GetComponent<DoubleTransform>().y_pos = earth.transform.position.y;
                    earth.GetComponent<BodyPath>().ReDraw();
                    moon.GetComponent<BodyPath>().ReDraw();
                }
            }

            if (closestPlanet == sun)
            {
                Vector2 positionAtTime = new Vector2(0, 0);
                Vector2 actualPos = closestPlanet.transform.position;
                Vector2 toAdd = actualPos - positionAtTime;

                if (!float.IsNaN(toAdd.x) && !float.IsNaN(toAdd.y))
                {
                    earth.transform.position = new Vector2(earth.GetComponent<BodyPath>().GetPositionAtTime(MyTime.time).x, earth.GetComponent<BodyPath>().GetPositionAtTime(MyTime.time).y) + toAdd;
                    earth.GetComponent<DoubleTransform>().x_pos = earth.transform.position.x;
                    earth.GetComponent<DoubleTransform>().y_pos = earth.transform.position.y;
                    moon.transform.position = new Vector2(moon.GetComponent<BodyPath>().GetPositionAtTime(MyTime.time).x, moon.GetComponent<BodyPath>().GetPositionAtTime(MyTime.time).y) + new Vector2(earth.transform.position.x, earth.transform.position.y);
                    moon.GetComponent<DoubleTransform>().x_pos = moon.transform.position.x;
                    moon.GetComponent<DoubleTransform>().y_pos = moon.transform.position.y;
                    earth.GetComponent<BodyPath>().ReDraw();
                    moon.GetComponent<BodyPath>().ReDraw();
                }
            }

        }

        foreach (Prediction prediction1 in mapManager.prediction)
        {
            prediction1.updated = false;
        }

    }
}
