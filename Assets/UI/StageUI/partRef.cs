using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class partRef : MonoBehaviour
{
    public GameObject refObj;

    public List<Color> baseColors = new List<Color>();
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void initializeEngineColors()
    {
        baseColors.Add(refObj.transform.GetChild(2).GetComponent<SpriteRenderer>().color);
        baseColors.Add(refObj.transform.GetChild(3).GetComponent<SpriteRenderer>().color);
        baseColors.Add(refObj.transform.GetChild(4).GetComponent<SpriteRenderer>().color);
    }

    public void initializeDecouplerColor()
    {
        baseColors.Add(refObj.GetComponent<SpriteRenderer>().color);
    }

    public void changeColorGreen()
    {
        if(baseColors.Count == 1)
        {
            refObj.GetComponent<SpriteRenderer>().color = Color.green;
            return;
        }

        if(baseColors.Count == 3)
        {
            refObj.transform.GetChild(2).GetComponent<SpriteRenderer>().color = Color.green;
            refObj.transform.GetChild(3).GetComponent<SpriteRenderer>().color = Color.green;
            refObj.transform.GetChild(4).GetComponent<SpriteRenderer>().color = Color.green;
            return;
        }
    }

    public void changeColorNormal()
    {
        if(baseColors.Count == 1)
        {
            refObj.GetComponent<SpriteRenderer>().color = baseColors[0];
            return;
        }

        if(baseColors.Count == 3)
        {
            refObj.transform.GetChild(2).GetComponent<SpriteRenderer>().color = baseColors[0];
            refObj.transform.GetChild(3).GetComponent<SpriteRenderer>().color = baseColors[1];
            refObj.transform.GetChild(4).GetComponent<SpriteRenderer>().color = baseColors[2];
            return;
        }
        refObj.GetComponent<SpriteRenderer>().color = baseColors[0];
    }
}
