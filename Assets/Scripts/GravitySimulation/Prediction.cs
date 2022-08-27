using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Prediction : MonoBehaviour
{
    public GameObject[] capsule;
    public GameObject WorldSaveManager;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Prediction

        //Vector3 currentPos = rb.position;
        //Vector3 prevPos = currentPos;
        //Vector3 currentVelocity = rb.velocity;
        //Vector3 planetCords = planet.transform.position;
        //int stepCount = 5000;
        //line.positionCount = stepCount;
        //List<float> distances = new List<float>();
        //for (int i = 0; i < stepCount; i++)
        //{
        //    Vector3 distance = planetCords - currentPos;
        //    forceDir = (planet.transform.position - currentPos).normalized;
        //    ForceVector = forceDir * G * Mass * rocketMass / (distance.magnitude * distance.magnitude);
        //    currentVelocity += ForceVector * Time.fixedDeltaTime;
        //    currentPos += currentVelocity * Time.fixedDeltaTime;
        //    distances.Add((planet.transform.position - currentPos).magnitude);
        //    prevPos = currentPos;
        //    line.SetPosition(i, prevPos);
        //}

        //Debug.Log("Apogee:" + distances.Max().ToString());
        //Debug.Log("Perigee:" + distances.Min().ToString());
    }
}