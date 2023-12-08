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
    private RocketPart[] rps;

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
            for(int z = 0; z < SceneManager.sceneCount; z++)
            {
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
        DoubleTransform dt = g.GetComponent<DoubleTransform>();   
        if(false == true)
        {
            dt.x_pos += difference.x;
            dt.y_pos += difference.y;
            dt.z_pos += difference.z;

            g.transform.position = new Vector3((float)dt.x_pos, (float)dt.y_pos, (float)dt.z_pos);
        }
        if(true == true)
        {
            g.transform.position += difference;
        }   
    }

    public void PauseRockets()
    {
        Rigidbody2D[] rps = GameObject.FindObjectsOfType<Rigidbody2D>();
        foreach(Rigidbody2D part in rps)
        {
            part.simulated = false;
        }
    }

    public void ActivateRocket()
    {
        Rigidbody2D[] rps = GameObject.FindObjectsOfType<Rigidbody2D>();
        foreach(Rigidbody2D rp in rps)
        {
            rp.simulated = true;
        }
    }
}
