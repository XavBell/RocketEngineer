using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowRescaler : MonoBehaviour
{
    public BoxCollider2D boxCollider;
    public GameObject Shadow;
    public SpriteRenderer sp;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePositionScale();
    }

    void UpdatePositionScale()
    {
        //Update pos of shadow
        float posX = -2f*(boxCollider.size.x*0.2f);
        Shadow.transform.localPosition = new Vector2(posX, Shadow.transform.localPosition.y);

        //Update scale y
        float scaleY = 2f*(boxCollider.size.y*0.7f);
        sp.size = new Vector2(sp.size.x, scaleY);
    }
}
