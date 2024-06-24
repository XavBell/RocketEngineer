using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AttachPoint : MonoBehaviour
{
    public string relativeOrientation;
    public bool isConnected;

    public  void Start()
    {
        if(SceneManager.GetActiveScene().name == "SampleScene")
        {
            GetComponent<SpriteRenderer>().enabled = false;
        }

    }
}
