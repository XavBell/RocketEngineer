using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FloatingOrigin : MonoBehaviour
{
    public float threshold = 0;
    public GameObject sun;
    public GameObject earth;
    public GameObject moon;
    public GameObject customCursor;
    public GameObject Prediction;
    public GameObject Camera;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        updateFloatReference();
    }



    public void updateFloatReference()
    {
        if(Camera.transform.position.magnitude > threshold){
            Vector3 difference = Vector3.zero - Camera.transform.position; 
            Debug.Log(SceneManager.sceneCount);
            for(int z = 0; z < SceneManager.sceneCount; z++)
            {
                Debug.Log(SceneManager.GetSceneAt(z).GetRootGameObjects().Count());
                foreach (GameObject g in SceneManager.GetSceneAt(z).GetRootGameObjects())
                {
                   UpdatePosition(g, difference);
                }
            }

            
            Prediction.GetComponent<Prediction>().updated = false;
        }

    }

    public void UpdatePosition(GameObject g, Vector3 difference)
    {
        if(g.GetComponent<DoubleTransform>() != null)
        {
            DoubleTransform doubleTransform = g.GetComponent<DoubleTransform>();
            doubleTransform.x_pos += difference.x;
            doubleTransform.y_pos += difference.y;
            doubleTransform.z_pos += difference.z;

            g.transform.position = new Vector3((float)doubleTransform.x_pos, (float)doubleTransform.y_pos, (float)doubleTransform.z_pos);
        }
        if(g.GetComponent<DoubleTransform>() == null)
        {
            g.transform.position += difference;
        }
        
        
    }
}
