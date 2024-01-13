using System.Collections;
using System.Collections.Generic;
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

    // Start is called before the first frame update
    void Start()
    {
        mapCam.GetComponent<Camera>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        EarthIcon.transform.position = mapCam.WorldToScreenPoint(Earth.transform.position);
        SunIcon.transform.position = mapCam.WorldToScreenPoint(Sun.transform.position);
        MoonIcon.transform.position = mapCam.WorldToScreenPoint(Moon.transform.position);
    }
}
