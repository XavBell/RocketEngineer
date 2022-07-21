using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControl : MonoBehaviour
{
    //Camera
    public Camera cam;
    private float targetZoom;
    Vector3 dragOrigin;
    private float zoomFactor = 300f;
    private float zoomLerp = 10f;
    Vector3 position;
    public GameObject sun;
    public GameObject rocket;
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
        Rocket();


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
            float X = rocket.transform.position.x;
            float Y = rocket.transform.position.y;
            position.Set(X, Y, 0f);

            cam.transform.position = position;
        }

    }



}

