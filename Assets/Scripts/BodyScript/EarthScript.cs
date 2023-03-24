using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EarthScript : MonoBehaviour
{
    
    
    //[RequireComponent(typeof(EdgeCollider2D))]

    public EdgeCollider2D EdgeCollider2D;
    public PolygonCollider2D Poly;

    public GameObject sun;
    public GameObject TimeRef;
    public TimeManager TimeManager;

    public LineRenderer circleRenderer;

    public GameObject blockCollider;
    public GameObject earth;
    // Start is called before the first frame update
        void Start()
    {
        DrawCircle(5000, 6371);
        
        TimeManager = TimeRef.GetComponent<TimeManager>();
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        //Vector3 rotateAxis = sun.transform.forward.normalized;
        //transform.RotateAround(sun.transform.position, rotateAxis, 0.0000000001f * Time.deltaTime);
    }

    void DrawCircle(int steps, float radius)
    {
        circleRenderer.positionCount = steps;
        List<Vector2> edges = new List<Vector2>();

        for(int currentStep = 0; currentStep<steps; currentStep++)
        {
            float circumferenceProgress = (float)currentStep/steps;

            float currentRadian = circumferenceProgress * 2 * Mathf.PI;

            float xScaled = Mathf.Cos(currentRadian);
            float yScaled = Mathf.Sin(currentRadian);

            float x = xScaled * radius;
            float y = yScaled * radius;

            Vector3 currentPosition = new Vector3(x, y, 0);
            edges.Add(currentPosition);
            circleRenderer.SetPosition(currentStep, currentPosition);
            
            
            GameObject current = Instantiate(blockCollider, currentPosition, Quaternion.Euler(0f, 0f, 0f));

            Vector2 v = earth.transform.position - current.transform.position;
            float lookAngle =  90 + Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
            current.transform.rotation = Quaternion.Euler(0f, 0f, lookAngle);
        }
        //Poly.SetPath(0, edges);
        EdgeCollider2D.SetPoints(edges);

    }

    
}
