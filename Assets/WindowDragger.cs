using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WindowDragger : MonoBehaviour
{
    public Camera cam;
    // Start is called before the first frame update
    void Start()
    {
        cam = GameObject.Find("Main Camera").GetComponent<Camera>();   
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnBeginDrag()
    {
        Debug.Log("begin drag");

    }


    public void OnDrag()
    {
        Debug.Log("dragging");
        this.transform.position =Input.mousePosition;
    }

    public void OnEndDrag()
    {
        Debug.Log("end drag");
    }
}
