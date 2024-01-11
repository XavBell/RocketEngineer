using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AttachPointScript : MonoBehaviour
{
    public string relativeLocation = "";
    public GameObject referenceBody;
    public GameObject attachedBody;

    void Start()
    {
        if(SceneManager.GetActiveScene().name == "SampleScene")
        {
            this.GetComponent<SpriteRenderer>().enabled = false;
        }
    }
}
