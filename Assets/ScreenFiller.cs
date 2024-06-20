using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenFiller : MonoBehaviour
{
    CameraControl cam;
    // Start is called before the first frame update
    void Start()
    {
        cam = transform.parent.GetComponent<CameraControl>();
    }

    // Update is called once per frame
    void Update()
    {
        float size = cam.smoothZoom * 6;
        transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(size, size, size), 0.5f);
    }
}
