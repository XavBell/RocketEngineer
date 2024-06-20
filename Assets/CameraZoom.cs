using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraZoom : MonoBehaviour
{
    private float targetZoom;
    private float zoomFactor = 0.5f;
    private float userFactor = 1f;
    private float zoomLerp = 10f;
    private float moveFactor = 5f;

    public Camera cam;
    public GameObject MasterManager;

    // Start is called before the first frame update
    void Start()
    {
        MasterManager = GameObject.FindGameObjectWithTag("MasterManager");
        userFactor = 1;
        
        cam = GetComponent<Camera>();
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

        if (targetZoom - scrollData * zoomFactor * cam.orthographicSize > 1)
        {
            targetZoom -= scrollData * zoomFactor * cam.orthographicSize * userFactor;
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, Time.deltaTime * zoomLerp);
        }
        if (targetZoom - scrollData * zoomFactor * cam.orthographicSize < 1)
        {
            targetZoom = 1;
            cam.orthographicSize = Mathf.Lerp(cam.orthographicSize, targetZoom, Time.deltaTime * zoomLerp);
        }
    }
}
