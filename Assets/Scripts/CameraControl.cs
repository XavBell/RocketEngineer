using System.Runtime.InteropServices.ComTypes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;

public class CameraControl : MonoBehaviour
{
    //Camera
    public Camera cam;
    public Rigidbody2D camBox;
    private float targetZoom;
    Vector3 dragOrigin;
    private float zoomFactor = 0.5f;
    private float zoomLerp = 10f;
    private float moveFactor = 5f;
    private float lineFactor = 0.01f;
    Vector3 position;
    public GameObject sun;
    public GameObject earth;
    public GameObject moon;
    public GameObject rocket;
    public Vector3 previousPos = new Vector3(0, 0, 0);

    private float sizeX, sizeY, ratio;

    public GameObject CamRef;

    public GameObject customCursor;

    public GameObject MasterManager;

    public GameObject MoonSprite;
    public GameObject EarthSprite;
    public GameObject Prediction;
    public launchsiteManager launchsiteManager;
    

    public float threshold = 5000;
    // Start is called before the first frame update
    void Start()
    {
        targetZoom = cam.orthographicSize;
        if(MasterManager == null)
        {  
            MasterManager = GameObject.FindGameObjectWithTag("MasterManager");
        }
    }

    // Update is called once per frame
    public void Update()
    {
        
        ZoomIn();

        if(MasterManager.GetComponent<MasterManager>().gameState == "Building")
        {  
            if(launchsiteManager.commandCenter != null)
            {
                WASD();
            }
            MapView();
        }

        if(MasterManager.GetComponent<MasterManager>().ActiveRocket != null)
        {
            UpdateToRocketPosition();
        }
        



    }


    public void ZoomIn()
    {
        float scrollData;
        scrollData = Input.GetAxis("Mouse ScrollWheel");

        if(targetZoom - scrollData * zoomFactor * cam.orthographicSize > 1)
        {
            targetZoom -= scrollData * zoomFactor * cam.orthographicSize;
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, Time.deltaTime*zoomLerp);
            //Prediction.GetComponent<LineRenderer>().widthMultiplier = cam.orthographicSize * lineFactor;
        }if(targetZoom - scrollData * zoomFactor * cam.orthographicSize < 1)
        {
            targetZoom = 1;
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, Time.deltaTime*zoomLerp);
            //Prediction.GetComponent<LineRenderer>().widthMultiplier = cam.orthographicSize*lineFactor;
        }
        
    }

    public void WASD()
    {
        float xAxisValue = 0;
        float yAxisValue = 0;
        if(Input.GetKey(KeyCode.D))
        {
            xAxisValue = 1 * cam.orthographicSize * moveFactor;
        }

        if(Input.GetKey(KeyCode.A))
        {
            xAxisValue = -1 * cam.orthographicSize * moveFactor;
        }

        if(Input.GetKey(KeyCode.S))
        {
            yAxisValue = -1 * cam.orthographicSize * moveFactor;
        }

        if(Input.GetKey(KeyCode.W))
        {
            yAxisValue = 1 * cam.orthographicSize * moveFactor;
        }

        if(cam != null)
        {
            if((cam.transform.position + new Vector3(xAxisValue*Time.deltaTime, yAxisValue*Time.deltaTime, 0) - launchsiteManager.commandCenter.transform.position).magnitude < 500)
            {
                cam.transform.position += new Vector3(xAxisValue*Time.deltaTime, yAxisValue*Time.deltaTime, 0);   
            }
            
        }
    }

    public void QE()
    {
        if (Input.GetKey(KeyCode.E))
        {
            cam.transform.RotateAround(earth.transform.position, earth.transform.forward*-1, 0.02f*Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.Q))
        {
            cam.transform.RotateAround(earth.transform.position, earth.transform.forward, 0.02f*Time.deltaTime);
        }
    }

    void MapView()
    {
        if(Input.GetKey(KeyCode.M))
        {
            cam.orthographicSize = 150000f;
            targetZoom = cam.orthographicSize;
        }
    }

    void UpdateToRocketPosition()
    {

        cam.transform.position = MasterManager.GetComponent<MasterManager>().ActiveRocket.transform.position;
            
    }

    public void updateFloatReference()
    {
        if(transform.position.magnitude > threshold){
            GameObject[] rockets = GameObject.FindGameObjectsWithTag("capsule");
            Vector3 difference = Vector3.zero - transform.position;

            foreach(GameObject go in rockets)
            {
                go.transform.position += difference; 
            }
            
            sun.transform.position += difference;
            earth.transform.position += difference;
            moon.transform.position += difference;
            customCursor.transform.position += difference;
            transform.position += difference;
            Prediction.GetComponent<Prediction>().updated = false;
        }

    }



}

