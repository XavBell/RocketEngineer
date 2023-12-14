using System.Collections.Generic;
using UnityEngine;

public class CircleMeshDivider : MonoBehaviour
{
    public int segments = 8; // Number of segments to divide the circle
    public float radius = 5f; // Radius of the circle
    public Material segmentMaterial; // Material for the segments

    void Start()
    {
        CreateMeshSegments();
    }

    void CreateMeshSegments()
    {
        Vector3 center = transform.position; // Center of the circle
        float segmentAngle = 360f / segments; // Angle for each segment

        for (int i = 0; i < segments; i++)
        {
            // Calculate start and end angles for the current segment
            float startAngle = i * segmentAngle;
            float endAngle = (i + 1) * segmentAngle;

            // Generate points for the segment based on start and end angles
            List<Vector3> segmentPoints = GenerateSegmentPoints(center, radius, startAngle, endAngle);

            // Create a new GameObject to hold the mesh and add MeshFilter and MeshRenderer components
            GameObject segmentObject = new GameObject("SegmentMesh" + i);
            MeshFilter segmentMeshFilter = segmentObject.AddComponent<MeshFilter>();
            MeshRenderer segmentMeshRenderer = segmentObject.AddComponent<MeshRenderer>();

            // Create a mesh for the segment
            Mesh segmentMesh = new Mesh();
            segmentMesh.vertices = segmentPoints.ToArray();

            // Triangulate the vertices to create triangles for the mesh
            // (You can modify this part based on your specific triangulation method)
            List<int> triangles = new List<int>();
            for (int j = 1; j < segmentPoints.Count - 1; j++)
            {
                triangles.Add(0);
                triangles.Add(j);
                triangles.Add(j + 1);
            }
            segmentMesh.triangles = triangles.ToArray();

            //normals
            List<Vector3> normalsList = new List<Vector3> { };
            for (int x = 0; x < segmentPoints.Count; x++)
            {
                normalsList.Add(-Vector3.forward);
            }
            Vector3[] normals = normalsList.ToArray();
            segmentMeshFilter.mesh.normals = normals;

            int numVerticies = segmentMeshFilter.mesh.vertices.Length;
            Vector2[] uvs = new Vector2[numVerticies];
            for(int g = 0; g < numVerticies; g++)
            {
                float x = Mathf.Cos((i/(float)numVerticies)*Mathf.PI*2.0f)*0.5f+0.5f;
                float y = Mathf.Sin((i/(float)numVerticies)*Mathf.PI*2.0f)*0.5f+0.5f;
                uvs[i] = new Vector2(x, y);
            }
            segmentMeshFilter.mesh.uv = uvs;
            segmentMeshFilter.mesh.RecalculateTangents();
            segmentMeshFilter.mesh.RecalculateNormals();
            segmentMeshFilter.mesh.RecalculateBounds();

            segmentMeshFilter.mesh = segmentMesh;
            //segmentMeshRenderer.material = segmentMaterial;

            // Optional: Adjusting the position and rotation of each segment object
            segmentObject.transform.position = center;
            segmentObject.transform.rotation = Quaternion.Euler(0f, 0f, startAngle);
        }
    }

    List<Vector3> GenerateSegmentPoints(Vector3 center, float radius, float startAngle, float endAngle)
    {
        List<Vector3> points = new List<Vector3>();

        // Convert angles to radians
        startAngle *= Mathf.Deg2Rad;
        endAngle *= Mathf.Deg2Rad;

        // Generate points along the arc between start and end angles
        float angleStep = (endAngle - startAngle) / 10f; // Increase subdivisions for smoother curve
        for (float angle = startAngle; angle <= endAngle; angle += angleStep)
        {
            float x = center.x + radius * Mathf.Cos(angle);
            float y = center.y + radius * Mathf.Sin(angle);
            points.Add(new Vector3(x, y, 0f));
        }

        return points;
    }
}
