using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class barManager : MonoBehaviour
{
    public GameObject  barUI;
    public container container;
    public RectTransform bar;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(container.mass > 0)
        {
            barUI.gameObject.SetActive(true);
            bar.sizeDelta = new Vector2(container.volume/container.tankVolume*100, bar.sizeDelta.y);
        }else{
            barUI.gameObject.SetActive(false);
        }
    }

    
}
