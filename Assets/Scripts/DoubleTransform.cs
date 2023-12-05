using System.Runtime.InteropServices.ComTypes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoubleTransform : MonoBehaviour
{
    public double x_pos = 0;
    public double y_pos = 0;
    public double z_pos = 0;

    public Vector3 previousPos = Vector3.zero;
    public Vector3 currentPos;

    // Start is called before the first frame update
    void Start()
    {
        x_pos = this.transform.position.x;
        y_pos = this.transform.position.y;
        z_pos = this.transform.position.z;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
