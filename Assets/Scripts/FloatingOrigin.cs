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
    public GameObject previousPlanet;
    public bool corrected = true;
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
        updateFloatReference();
        UpdateReferenceBody();
    }

    void Update()
    {
        if (bypass == false)
        {

            if (DO == true && LOCK == false && fixedRan == true)
            {
                DO = true;
            }
        }
        fixedRan = false;
        bypass = false;
    }



    /// <summary>
    /// Updates the floating origin reference point based on the camera position.
    /// </summary>
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

    /// <summary>
    /// Updates the position of a GameObject by applying the specified difference vector.
    /// If the GameObject has a DoubleTransform component, the position will be updated using its x, y, and z positions.
    /// If the GameObject does not have a DoubleTransform component, the position will be updated directly.
    /// </summary>
    /// <param name="g">The GameObject to update the position of.</param>
    /// <param name="difference">The difference vector to apply to the position.</param>
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
            if(!g.GetComponent<Canvas>())
            {
                g.transform.position += difference;
            }
        }
    }

    /// <summary>
    /// Updates the reference body based on the active rocket's position.
    /// If there is no active rocket, it finds the closest planet and adjusts the positions of the sun, earth, and moon accordingly.
    /// If there is an active rocket, it adjusts the positions of the sun, earth, and moon based on the active rocket's position.
    /// </summary>
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

        if (masterManager.ActiveRocket != null)
        {
            closestPlanet = masterManager.ActiveRocket.GetComponent<PlanetGravity>().getPlanet();
            DoubleTransform dtClosestPlanet = closestPlanet.GetComponent<DoubleTransform>();

            (double, double) separation = (0, 0);
            if(corrected == false && closestPlanet != previousPlanet)
            {
                separation = (masterManager.ActiveRocket.GetComponent<DoubleTransform>().x_pos - previousPlanet.GetComponent<DoubleTransform>().x_pos, masterManager.ActiveRocket.GetComponent<DoubleTransform>().y_pos - previousPlanet.GetComponent<DoubleTransform>().y_pos);
            }else if(closestPlanet == previousPlanet)
            {
                corrected = true;
                previousPlanet = null;
            }

            if (closestPlanet == earth)
            {
                double positionAtTimeX = closestPlanet.GetComponent<BodyPath>().GetPositionAtTimeDouble(MyTime.time).x;
                double positionAtTimeY = closestPlanet.GetComponent<BodyPath>().GetPositionAtTimeDouble(MyTime.time).y;

                double actualPosX = dtClosestPlanet.x_pos;
                double actualPosY = dtClosestPlanet.y_pos;

                double toAddX = actualPosX - positionAtTimeX;
                double toAddY = actualPosY - positionAtTimeY;

                Vector2 toAdd = new Vector2((float)toAddX, (float)toAddY);

                if (!double.IsNaN(toAddX) && !double.IsNaN(toAddY))
                {
                    sun.transform.position = toAdd;
                    sun.GetComponent<DoubleTransform>().x_pos = toAddX;
                    sun.GetComponent<DoubleTransform>().y_pos = toAddY;
                    double moonPosX = moon.GetComponent<BodyPath>().GetPositionAtTimeDouble(MyTime.time).x + positionAtTimeX + toAddX;
                    double moonPosY = moon.GetComponent<BodyPath>().GetPositionAtTimeDouble(MyTime.time).y + positionAtTimeY + toAddY;
                    moon.transform.position = new Vector2((float)moonPosX, (float)moonPosY);
                    moon.GetComponent<DoubleTransform>().x_pos = moonPosX;
                    moon.GetComponent<DoubleTransform>().y_pos = moonPosY;
                    earth.GetComponent<BodyPath>().ReDraw();
                    moon.GetComponent<BodyPath>().ReDraw();

                    if (corrected == false)
                    {
                        masterManager.ActiveRocket.GetComponent<DoubleTransform>().x_pos = previousPlanet.GetComponent<DoubleTransform>().x_pos + separation.Item1;
                        masterManager.ActiveRocket.GetComponent<DoubleTransform>().y_pos = previousPlanet.GetComponent<DoubleTransform>().y_pos + separation.Item2;
                        masterManager.ActiveRocket.transform.position = new Vector2((float)masterManager.ActiveRocket.GetComponent<DoubleTransform>().x_pos, (float)masterManager.ActiveRocket.GetComponent<DoubleTransform>().y_pos);
                        corrected = true;
                        previousPlanet = null;
                    }
                }
            }

            if (closestPlanet == moon)
            {
                double positionAtTimeX = closestPlanet.GetComponent<BodyPath>().GetPositionAtTimeDouble(MyTime.time).x + earth.GetComponent<BodyPath>().GetPositionAtTimeDouble(MyTime.time).x;
                double positionAtTimeY = closestPlanet.GetComponent<BodyPath>().GetPositionAtTimeDouble(MyTime.time).y + earth.GetComponent<BodyPath>().GetPositionAtTimeDouble(MyTime.time).y;

                double actualPosX = dtClosestPlanet.x_pos;
                double actualPosY = dtClosestPlanet.y_pos;

                double toAddX = actualPosX - positionAtTimeX;
                double toAddY = actualPosY - positionAtTimeY;

                Vector2 toAdd = new Vector2((float)toAddX, (float)toAddY);

                if (!double.IsNaN(toAddX) && !double.IsNaN(toAddY))
                {
                    sun.transform.position = toAdd;
                    sun.GetComponent<DoubleTransform>().x_pos = toAddX;
                    sun.GetComponent<DoubleTransform>().y_pos = toAddY;
                    double earthPosX = earth.GetComponent<BodyPath>().GetPositionAtTimeDouble(MyTime.time).x + toAddX;
                    double earthPosY = earth.GetComponent<BodyPath>().GetPositionAtTimeDouble(MyTime.time).y + toAddY;
                    earth.transform.position = new Vector2((float)earthPosX, (float)earthPosY);
                    earth.GetComponent<DoubleTransform>().x_pos = earthPosX;
                    earth.GetComponent<DoubleTransform>().y_pos = earthPosY;
                    earth.GetComponent<BodyPath>().ReDraw();
                    moon.GetComponent<BodyPath>().ReDraw();

                    if (corrected == false)
                    {
                        masterManager.ActiveRocket.GetComponent<DoubleTransform>().x_pos = previousPlanet.GetComponent<DoubleTransform>().x_pos + separation.Item1;
                        masterManager.ActiveRocket.GetComponent<DoubleTransform>().y_pos = previousPlanet.GetComponent<DoubleTransform>().y_pos + separation.Item2;
                        masterManager.ActiveRocket.transform.position = new Vector2((float)masterManager.ActiveRocket.GetComponent<DoubleTransform>().x_pos, (float)masterManager.ActiveRocket.GetComponent<DoubleTransform>().y_pos);
                        corrected = true;
                        previousPlanet = null;
                    }

                }
            }

            if (closestPlanet == sun)
            {
                double positionAtTimeX = 0;
                double positionAtTimeY = 0;

                double actualPosX = dtClosestPlanet.x_pos;
                double actualPosY = dtClosestPlanet.y_pos;
                
                double toAddX = actualPosX - positionAtTimeX;
                double toAddY = actualPosY - positionAtTimeY;

                Vector2 toAdd = new Vector2((float)toAddX, (float)toAddY);

                if (!double.IsNaN(toAddX) && !double.IsNaN(toAddY))
                {
                    double earthPosX = earth.GetComponent<BodyPath>().GetPositionAtTimeDouble(MyTime.time).x + toAddX;
                    double earthPosY = earth.GetComponent<BodyPath>().GetPositionAtTimeDouble(MyTime.time).y + toAddY;
                    earth.transform.position = new Vector2((float)earthPosX, (float)earthPosY);
                    earth.GetComponent<DoubleTransform>().x_pos = earthPosX;
                    earth.GetComponent<DoubleTransform>().y_pos = earthPosY;

                    double moonPosX = moon.GetComponent<BodyPath>().GetPositionAtTimeDouble(MyTime.time).x + earthPosX;
                    double moonPosY = moon.GetComponent<BodyPath>().GetPositionAtTimeDouble(MyTime.time).y + earthPosY;
                    moon.transform.position = new Vector2((float)moonPosX, (float)moonPosY);
                    moon.GetComponent<DoubleTransform>().x_pos = moonPosX;
                    moon.GetComponent<DoubleTransform>().y_pos = moonPosY;
                    earth.GetComponent<BodyPath>().ReDraw();
                    moon.GetComponent<BodyPath>().ReDraw();

                    if (corrected == false)
                    {
                        masterManager.ActiveRocket.GetComponent<DoubleTransform>().x_pos = previousPlanet.GetComponent<DoubleTransform>().x_pos + separation.Item1;
                        masterManager.ActiveRocket.GetComponent<DoubleTransform>().y_pos = previousPlanet.GetComponent<DoubleTransform>().y_pos + separation.Item2;
                        masterManager.ActiveRocket.transform.position = new Vector2((float)masterManager.ActiveRocket.GetComponent<DoubleTransform>().x_pos, (float)masterManager.ActiveRocket.GetComponent<DoubleTransform>().y_pos);
                        corrected = true;
                        previousPlanet = null;
                    }
                }
            }

        }

        foreach (Prediction prediction1 in mapManager.prediction)
        {
            prediction1.updated = false;
        }

    }
}
