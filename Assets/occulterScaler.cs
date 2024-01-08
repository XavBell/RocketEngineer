using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class occulterScaler : MonoBehaviour
{
    public SpriteRenderer TankRef;
    public string relative;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(relative == "side")
        {
            this.transform.localScale = new Vector2(TankRef.size.y*10, this.transform.localScale.y);
        }

        if(relative == "top")
        {
            this.transform.localScale = new Vector2(TankRef.size.x*10, this.transform.localScale.y);
        }
    }
}
