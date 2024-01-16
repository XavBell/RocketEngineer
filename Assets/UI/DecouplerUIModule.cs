using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecouplerUIModule : MonoBehaviour
{
    public Decoupler decoupler;
    public StageViewer stageViewer;
    public Color decouplerColor;
    // Start is called before the first frame update
    void Start()
    {
        stageViewer = FindObjectOfType<StageViewer>();
        decouplerColor = decoupler.GetComponent<SpriteRenderer>().color;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void activate()
    {
        decoupler.activated = true;
        changeColorNormal();
        if(stageViewer != null)
        {
            //stageViewer.resetDropdown();
        }
    }

    public void changeColorGreen()
    {
        decoupler.transform.GetChild(2).GetComponent<SpriteRenderer>().color = Color.green;
        decoupler.GetComponent<SpriteRenderer>().color = Color.green;
    }

    public void changeColorNormal()
    {
        decoupler.transform.GetChild(2).GetComponent<SpriteRenderer>().color = Color.white;
        decoupler.GetComponent<SpriteRenderer>().color = decouplerColor;

    }
}
