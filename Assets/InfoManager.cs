using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfoManager : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject infoPanel;
    public List<GameObject> infoPanels;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void activateDeactivate(GameObject panelToExclude)
    {
        if (panelToExclude.active == false)
        {
            infoPanel.SetActive(true);
            foreach (GameObject panel in infoPanels)
            {
                panel.SetActive(false);
            }
            panelToExclude.SetActive(true);
            return;
        }else{
            foreach (GameObject panel in infoPanels)
            {
                panel.SetActive(false);
            }
            infoPanel.SetActive(false);
        }

    }
}
