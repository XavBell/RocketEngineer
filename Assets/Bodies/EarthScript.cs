using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthScript : MonoBehaviour
{
    public GameObject sun;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 rotateAxis = sun.transform.forward.normalized;
        transform.RotateAround(sun.transform.position, rotateAxis, 0.00001f * Time.deltaTime);
    }
}
