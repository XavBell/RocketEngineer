using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DistanceChecker : MonoBehaviour
{
    public GameObject Planet;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float distance  = Vector2.Distance(transform.position, Planet.transform.position);
        Debug.Log(distance);
    }
}
