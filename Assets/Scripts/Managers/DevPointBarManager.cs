using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

/*Manage Points in Sample Scene*/
public class DevPointBarManager : MonoBehaviour
{
    public pointManager pointManager;
    public TMP_Text nPoint;
    public BuildingManager buildingManager;

    // Start is called before the first frame update
    void Start()
    {
        pointManager = FindObjectOfType<pointManager>();
    }

    // Update is called once per frame
    void Update()
    {
        nPoint.text = pointManager.nPoints.ToString();
    }

    public void placeBuilding(GameObject building)
    {
        if(building.GetComponent<buildingType>().cost <= pointManager.nPoints)
        {
            buildingManager.ConstructPart(building);
        }
    }
}
