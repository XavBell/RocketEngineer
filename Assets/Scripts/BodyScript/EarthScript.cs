//using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEditor;


public class EarthScript : MonoBehaviour
{
    public GameObject sun;
    public GameObject TimeRef;
    public TimeManager TimeManager;

    public GameObject blockCollider;
    public GameObject earth;
    public List<Vector3> Points;


    public GameObject cam;

    private float G; //Gravitational constant
    public float gSlvl;
    public float earthMass = 0f;
    public float earthRadius;
    public bool updated = true;
    public PolygonCollider2D polyCollider;

    public SolarSystemManager SolarSystemManager;

    // Update is called once per frame
    void Start()
    {
        polyCollider = GetComponent<PolygonCollider2D>();

    }



    public void InitializeEarth()
    {
        SetEarthMass();
        this.GetComponent<DoubleTransform>().x_pos = this.transform.position.x;
        this.GetComponent<DoubleTransform>().y_pos = this.transform.position.y;
        this.GetComponent<PhysicsStats>().x_pos = this.transform.position.x;
        this.GetComponent<PhysicsStats>().y_pos = this.transform.position.y;
        //DrawCircle(100000, 63710);
        
        TimeManager = TimeRef.GetComponent<TimeManager>();
    }

    void DrawCircle(int steps, float radius)
    {
        radius += 0.5f;
        List<Vector2> edges = new List<Vector2>();
        Vector3 previousPos = new Vector3(0f, 0f, 0f);
        GameObject previous = null;

        for(int currentStep = 0; currentStep<steps; currentStep++)
        {
            float circumferenceProgress = (float)currentStep/steps;

            float currentRadian = circumferenceProgress * 2 * Mathf.PI;

            float xScaled = Mathf.Cos(currentRadian);
            float yScaled = Mathf.Sin(currentRadian);

            float x = xScaled * radius;
            float y = yScaled * radius;

            Vector3 currentPosition = new Vector3(x, y, 0) + earth.transform.position;
            edges.Add(currentPosition);
            
            GameObject current = Instantiate(blockCollider, currentPosition - new Vector3(0f, 0.5f, 0f), Quaternion.Euler(0f, 0f, 0f));

            if(currentStep == 0)
            {
                previous = current;
            }

            Vector2 v = earth.transform.position - current.transform.position;
            float lookAngle =  90 + Mathf.Atan2(v.y, v.x) * Mathf.Rad2Deg;
            current.transform.rotation = Quaternion.Euler(0f, 0f, lookAngle);

            if(currentStep != 0)
            {
                float newX = (current.transform.position - previousPos).magnitude;
                current.transform.localScale = new Vector3(newX, current.transform.localScale.y, current.transform.localScale.z);

                if(currentStep == 1)
                {
                    previous.transform.localScale = new Vector3(newX, previous.transform.localScale.y, previous.transform.localScale.z);
                }
            }

            previousPos = current.transform.position;

            current.transform.SetParent(earth.transform);
            Points.Add(current.transform.localPosition);
            
        }

        DrawMesh(Points, 63710, 100000);
        List<Vector3> Points2 = new List<Vector3>();
        foreach(Vector2 point in polyCollider.points)
        {
            Points2.Add(point);
        }

        test(Points2, 63710, Points2.Count);
        var mf = GetComponent<MeshFilter>();
        if (mf)
        {
            var savePath = "Assets/" + "EarthMeshHighRes" + ".asset";
            Debug.Log("Saved Mesh to:" + savePath);
            //AssetDatabase.CreateAsset(mf.mesh, savePath);
        }
    
    }

    void SetEarthMass()
    {
        GetValues();
        earthMass = gSlvl*(earthRadius*earthRadius)/G;
        SolarSystemManager.earthMass = earthMass;
    }

    public void DrawMesh(List<Vector3> verticiesList, float radius, int n)
    {
        Vector3[] verticies = verticiesList.ToArray();
        //polyCollider
        polyCollider.pathCount = 1;

        List<Vector2> pathList = new List<Vector2> { };
        for (int i = 0; i < n; i++)
        {
            pathList.Add(new Vector2(verticies[i].x, verticies[i].y));
        }
        Vector2[] path = pathList.ToArray();

        polyCollider.SetPath(0, path);
        //SaveMesh(mesh, "planet", true, false);
    }

    void test(List<Vector3> verticiesList, float radius, int n)
    {
        MeshFilter mf = GetComponent<MeshFilter>();
        Mesh mesh = new Mesh();
        mf.mesh = mesh;
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        Vector3[] verticies = verticiesList.ToArray();
        
        //triangles
        List<int> trianglesList = new List<int> {};
        for(int i = 0; i < (n-2); i++)
        {
            trianglesList.Add(0);
            trianglesList.Add(i+1);
            trianglesList.Add(i+2);
        }

        int[] triangles = trianglesList.ToArray();

        //normals
        List<Vector3> normalsList = new List<Vector3> { };
        for (int i = 0; i < verticies.Length; i++)
        {
            normalsList.Add(-Vector3.forward);
        }
        Vector3[] normals = normalsList.ToArray();

        

        //initialise
        mesh.vertices = verticies;
        mesh.triangles = triangles;
        mesh.normals = normals;


        int numVerticies = mesh.vertices.Length;
        Vector2[] uvs = new Vector2[numVerticies];
        for(int i = 0; i < numVerticies; i++)
        {
            float x = Mathf.Cos((i/(float)numVerticies)*Mathf.PI*2.0f)*0.5f+0.5f;
            float y = Mathf.Sin((i/(float)numVerticies)*Mathf.PI*2.0f)*0.5f+0.5f;
            uvs[i] = new Vector2(x, y);
        }
        mesh.uv = uvs;
        mesh.RecalculateTangents();
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }

    

    void GetValues()
    {
        G = SolarSystemManager.G;
        earthRadius = SolarSystemManager.earthRadius;
        gSlvl = SolarSystemManager.earthgSlvl;
    }

    
}
