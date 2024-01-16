using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
    public GameObject panel;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void fade()
    {
        if(panel.active == false)
        {
            PanelFadeIn();
            return;
        }

        if(panel.active == true)
        {
            PanelFadeOut();
            return;
        }
    }

    public void PanelFadeIn()
    {
        panel.transform.localScale = new Vector3(0, 0, 0);
        panel.transform.DOScale(1, 0.5f);
    }

    public void PanelFadeOut()
    {
        panel.transform.DOScale(0, 0.5f);
        panel.transform.localScale = new Vector3(1, 1, 1);
    }

}
