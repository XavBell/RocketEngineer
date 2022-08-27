using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class autoParentScaling : MonoBehaviour
{
    public SpriteRenderer parentSP;
    public SpriteRenderer childSP;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float minValue = Mathf.Min(parentSP.size.x, parentSP.size.y);
        childSP.size = new Vector2 (minValue, minValue);
    }
}
