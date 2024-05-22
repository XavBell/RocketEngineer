using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingVelocity : MonoBehaviour
{
    public List<GameObject> bodies = new List<GameObject>();
    public (double, double) velocity = (0, 0);
    public TimeManager timeManager;
    public double x = 0;
    public double y = 0;

    // Start is called before the first frame update
    void Start()
    {
        timeManager = FindObjectOfType<TimeManager>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        x = velocity.Item1;
        y = velocity.Item2;
        
    }
}
