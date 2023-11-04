using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    //Camera
    public Camera cam;
    public Rigidbody2D camBox;
    private float targetZoom;
    Vector3 dragOrigin;
    private float zoomFactor = 0.5f;
    private float zoomLerp = 10f;
    private float moveFactor = 0.01f;
    Vector3 position;
    public GameObject sun;
    public GameObject earth;
    public GameObject rocket;
    public Vector3 previousPos = new Vector3(0, 0, 0);

    private float sizeX, sizeY, ratio;

    public GameObject CamRef;

    public GameObject customCursor;

    public GameObject MasterManager;

    public GameObject MoonSprite;
    public GameObject EarthSprite;
    public GameObject Prediction;
    

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
    void Update()
    {
        
        ZoomIn();

        if(MasterManager.GetComponent<MasterManager>().gameState == "Building")
        {  
            QE();
            WASD();
            MapView();
        }

        if(MasterManager.GetComponent<MasterManager>().gameState == "Flight")
        {  
            UpdateToRocketPosition();
        }
    }

    void FixedUpdate()
    {
        updateFloatReference();
    }



    public void ZoomIn()
    {
        float scrollData;
        scrollData = Input.GetAxis("Mouse ScrollWheel");

        if(targetZoom - scrollData * zoomFactor * cam.orthographicSize > 1)
        {
            targetZoom -= scrollData * zoomFactor * cam.orthographicSize;
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, Time.deltaTime*zoomLerp);
        }if(targetZoom - scrollData * zoomFactor * cam.orthographicSize < 1)
        {
            targetZoom = 1;
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, Time.deltaTime*zoomLerp);
        }
        
    }

    public void WASD()
    {
        Vector2 dist = this.transform.position;
        float xAxisValue = Input.GetAxis("Horizontal") * cam.orthographicSize * moveFactor;
        float yAxisValue = Input.GetAxis("Vertical") * cam.orthographicSize * moveFactor;
        if(cam != null)
        {

            cam.transform.Translate(new Vector2(xAxisValue, yAxisValue));
            
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

    void updateFloatReference()
    {
        if(transform.position.magnitude > threshold){
            GameObject[] planetsToMove = GameObject.FindGameObjectsWithTag("Planet");
            GameObject sun = GameObject.FindGameObjectWithTag("Sun");
            GameObject[] rockets = GameObject.FindGameObjectsWithTag("capsule");
            Vector3 difference = new Vector3(0, 0, transform.position.z) - transform.position;
            foreach(GameObject go in planetsToMove)
            {
                go.transform.position = go.transform.position + difference;
            }

            foreach(GameObject go in rockets)
            {
                go.transform.position = go.transform.position + difference; 
            }
            
            sun.transform.position = sun.transform.position + difference;
            customCursor.transform.position += difference;
            transform.position += difference;
            Prediction.GetComponent<Prediction>().updated = false;
        }
    }

}

