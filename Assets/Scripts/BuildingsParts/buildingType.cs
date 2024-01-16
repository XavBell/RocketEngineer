using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class buildingType : MonoBehaviour
{
    public string type;
    public int buildingID;
    public GameObject inputUI;
    public GameObject outputUI;
    public GameObject selectUI;
    public GameObject UI;
    public GameObject anchor;
    public float cost;
    
    public BuildingManager buildingManager;
    void Start()
    {
        buildingManager = FindObjectOfType<BuildingManager>();
    }

    public void changeColorRed()
    {
        if(buildingManager.CanDestroy == true)
        {
            this.GetComponent<SpriteRenderer>().color = Color.red;
        }
    }

    public void changeColorWhite()
    {
        if(buildingManager.CanDestroy == true)
        {
            this.GetComponent<SpriteRenderer>().color = Color.white;
        }
    }
}
