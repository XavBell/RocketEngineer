using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SetCamera : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if(SceneManager.GetActiveScene().name == "SampleScene")
        {
            Camera go = GameObject.FindObjectOfType<Camera>();
            this.GetComponent<Canvas>().worldCamera = go;

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
