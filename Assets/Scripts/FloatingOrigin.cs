using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
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
    void FixedUpdate()
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

        g.transform.position += difference;
        
    }
}
