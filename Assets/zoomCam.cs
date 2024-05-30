using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class zoomCam : MonoBehaviour
{
     //Camera
    public Camera cam;
    public MasterManager masterManager;
    private float userFactor = 1f;
    private float targetZoom = 3;
    public float zoomFactor = 1f;
    public float zoomLerp = 10f;
    // Start is called before the first frame update
    void Start()
    {
        masterManager = FindObjectOfType<MasterManager>();
        userFactor = masterManager.scrollMultiplierValue;
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

        if (targetZoom - scrollData * zoomFactor * cam.orthographicSize  > 0.001)
        {
            targetZoom -= scrollData * zoomFactor * cam.orthographicSize * userFactor;
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, Time.deltaTime * zoomLerp);
            //Prediction.GetComponent<LineRenderer>().widthMultiplier = cam.orthographicSize * lineFactor;
        }
        if (targetZoom - scrollData * zoomFactor * cam.orthographicSize < 0.001)
        {
            targetZoom = 0.001f;
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, Time.deltaTime * zoomLerp);
            //Prediction.GetComponent<LineRenderer>().widthMultiplier = cam.orthographicSize*lineFactor;
        }
    }
}
