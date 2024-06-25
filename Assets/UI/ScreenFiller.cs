using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UI;

public class ScreenFiller : MonoBehaviour
{
    CanvasScaler scaler;

    private void Start()
    {
        scaler = GetComponent<CanvasScaler>();
    }
    // Update is called once per frame
    void Update()
    {
        float sizeX = Camera.main.orthographicSize * 2.1f;
        sizeX += sizeX - Camera.main.orthographicSize * 2.1f;
        float sizeY = sizeX * Screen.height/Screen.width;
        float size = Mathf.Max(sizeX, sizeY) * 2;
        transform.localScale = new Vector3(size, size, 1);
    }
}
