using System.Collections.Generic;
using UnityEngine;

public class CircleColliderDivider : MonoBehaviour
{
    public int segments = 8; // Number of segments to divide the circle
    public float radius = 63710f; // Radius of the circle

    void Start()
    {
        CreatePolygonColliders();
    }

    void CreatePolygonColliders()
    {
        Vector2 center = transform.position; // Center of the circle
        float segmentAngle = 360f / segments; // Angle for each segment

        for (int i = 0; i < segments; i++)
        {
            // Calculate start and end angles for the current segment
            float startAngle = i * segmentAngle;
            float endAngle = (i + 1) * segmentAngle;

            // Generate points for the segment based on start and end angles
            List<Vector2> segmentPoints = GenerateSegmentPoints(center, radius, startAngle, endAngle);

            // Create a new GameObject to hold the collider and add PolygonCollider2D component
            GameObject segmentObject = new GameObject("SegmentCollider" + i);
            segmentObject.transform.position = center;
            PolygonCollider2D segmentCollider = segmentObject.AddComponent<PolygonCollider2D>();
            segmentObject.transform.SetParent(this.gameObject.transform);

            // Set the points for the PolygonCollider2D
            segmentCollider.points = segmentPoints.ToArray();
        }
    }

    List<Vector2> GenerateSegmentPoints(Vector2 center, float radius, float startAngle, float endAngle)
    {
        List<Vector2> points = new List<Vector2>();

        // Convert angles to radians
        startAngle *= Mathf.Deg2Rad;
        endAngle *= Mathf.Deg2Rad;

        // Add the center point as the first point of the segment
        points.Add(center);

        // Generate points along the arc between start and end angles
        float angleStep = (endAngle - startAngle) / 10f; // Increase subdivisions for smoother curve
        for (float angle = startAngle; angle <= endAngle; angle += angleStep)
        {
            float x = center.x + radius * Mathf.Cos(angle);
            float y = center.y + radius * Mathf.Sin(angle);
            points.Add(new Vector2(x, y));
        }

        // Add the center point again to close the shape
        points.Add(center);

        return points;
    }
}
