using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    //Camera
    public Camera cam;
    private float targetZoom;
    Vector3 dragOrigin;
    private float zoomFactor = 10f;
    private float zoomLerp = 10f;
    Vector3 position;
    public GameObject sun;
    public GameObject rocket;
    public Vector3 previousPos = new Vector3(0, 0, 0);

    public GameObject CamRef;
    // Start is called before the first frame update
    void Start()
    {
        targetZoom = cam.orthographicSize;
    }

    // Update is called once per frame
    void Update()
    {
        PanCamera();
        ZoomIn();
        if(rocket != null){
            Rocket();
        }
        rocket = GameObject.FindGameObjectWithTag("capsule");

    }

    private void PanCamera()
    {
        if (Input.GetMouseButtonDown(0))
        {
            dragOrigin = cam.ScreenToWorldPoint(Input.mousePosition);
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 difference = dragOrigin - cam.ScreenToWorldPoint(Input.mousePosition);

            cam.transform.position += difference;
        }


    }

    public void ZoomIn()
    {
        float scrollData;
        scrollData = Input.GetAxis("Mouse ScrollWheel");

        targetZoom -= scrollData * zoomFactor;
        cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, Time.deltaTime*zoomLerp);
        

    }

    public void Rocket()
    {


        if (Input.GetKey(KeyCode.E))
        {
            CamRef.transform.position = new Vector3(rocket.transform.position.x, rocket.transform.position.y, -10);
        }


    }



}

