using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesignerCursor : MonoBehaviour
{
    public GameObject selectedPart;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        transform.position = mousePosition;
        
    }
}
