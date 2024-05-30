using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class soiLineRenderer : MonoBehaviour
{
    public float soiRange;
    public LineRenderer line;

    // Start is called before the first frame update
    void Start()
    {
        generateCircle(soiRange/(this.transform.parent.localScale.x * MapManager.scaledSpace), 2000);
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    void generateCircle(float radius, int steps)
    {
        List<Vector3> points = new List<Vector3>();
        for(int currentStep = 0; currentStep<steps; currentStep++)
        {
            float circumferenceProgress = (float)currentStep/steps;

            float currentRadian = circumferenceProgress * 2 * Mathf.PI;

            float xScaled = Mathf.Cos(currentRadian);
            float yScaled = Mathf.Sin(currentRadian);

            float x = xScaled * radius;
            float y = yScaled * radius;

            Vector3 currentPosition = new Vector3(x, y, 0);
            points.Add(currentPosition);
        }
        
        line.positionCount = points.Count;
        line.SetPositions(points.ToArray());
    }
}
