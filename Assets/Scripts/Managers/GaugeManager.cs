using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class GaugeManager : MonoBehaviour
{
    public container container;
    public TMP_Text current;
    public TMP_Text max;
    public GameObject Indicator;
    public GameObject[] Thresholds;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(container.mass != 0)
        {
            calculateIndicatorRotation();
        }
    }

    void calculateIndicatorRotation()
    {
        //Set text on lines
        float increment = float.Parse(max.text)/4;
        int i = 0;
        foreach(GameObject thre in Thresholds)
        {
            thre.GetComponent<TMP_Text>().text = (i*increment).ToString();
            i++;
        }

        //Set Rotation
        float angle = 180*float.Parse(current.text)/float.Parse(max.text);
        Indicator.transform.rotation = Quaternion.Euler(0,0, -angle);
    }
}
