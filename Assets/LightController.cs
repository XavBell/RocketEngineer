using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightController : MonoBehaviour
{
    public GameObject Sun;
    public GameObject CB;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        calculateAngle();
    }

    void calculateAngle()
    {
        Vector3 sunPos = Sun.transform.position;
        Vector3 cbPos = CB.transform.position;

        Vector3 sunToCB = cbPos - sunPos;

        float angle = Mathf.Rad2Deg * Mathf.Atan2(sunToCB.y, sunToCB.x);
        this.transform.eulerAngles = new Vector3(0, 0, angle + 90);
    }
}
