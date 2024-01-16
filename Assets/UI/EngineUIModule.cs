using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EngineUIModule : MonoBehaviour
{
    public Engine engine;
    public Color color1;
    public Color color2;
    public Color color3;
    // Start is called before the first frame update
    void Start()
    {
        color1 = engine.transform.GetChild(3).GetComponent<SpriteRenderer>().color;
        color2 = engine.transform.GetChild(4).GetComponent<SpriteRenderer>().color;
        color3 = engine.transform.GetChild(5).GetComponent<SpriteRenderer>().color;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void activate()
    {
        engine.active = true;
    }

    public void changeColorGreen()
    {
        engine.transform.GetChild(3).GetComponent<SpriteRenderer>().color = Color.green;
        engine.transform.GetChild(4).GetComponent<SpriteRenderer>().color = Color.green;
        engine.transform.GetChild(5).GetComponent<SpriteRenderer>().color = Color.green;
    }

    public void changeColorNormal()
    {
        engine.transform.GetChild(3).GetComponent<SpriteRenderer>().color = color1;
        engine.transform.GetChild(4).GetComponent<SpriteRenderer>().color = color2;
        engine.transform.GetChild(5).GetComponent<SpriteRenderer>().color = color3;
    }
}
