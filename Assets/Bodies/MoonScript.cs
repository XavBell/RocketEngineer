using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoonScript : MonoBehaviour
{
    public GameObject Earth;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 rotateAxis = Earth.transform.forward.normalized;
        transform.RotateAround(Earth.transform.position, rotateAxis, 0.002f * Time.deltaTime);
    }
}
