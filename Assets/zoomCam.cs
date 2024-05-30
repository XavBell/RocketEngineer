using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class zoomCam : MonoBehaviour
{
     //Camera
    public Camera cam;
    private float targetZoom;
    private float zoomFactor = 0.1f;
    private float zoomLerp = 100f;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        ZoomIn();
    }

    public void ZoomIn()
    {

        float scrollData;
        scrollData = Input.GetAxis("Mouse ScrollWheel");

        if (targetZoom - scrollData * zoomFactor * cam.orthographicSize > 0.01)
        {
            targetZoom -= scrollData * zoomFactor * cam.orthographicSize;
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, Time.deltaTime * zoomLerp);
            //Prediction.GetComponent<LineRenderer>().widthMultiplier = cam.orthographicSize * lineFactor;
        }
        if (targetZoom - scrollData * zoomFactor * cam.orthographicSize < 0.01)
        {
            targetZoom = 0.01f;
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, Time.deltaTime * zoomLerp);
            //Prediction.GetComponent<LineRenderer>().widthMultiplier = cam.orthographicSize*lineFactor;
        }
    }
}
