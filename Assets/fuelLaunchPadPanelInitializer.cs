using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class fuelLaunchPadPanelInitializer : MonoBehaviour
{
    public launchPadManager launchPadManager;
    public TMP_Dropdown padLinesDropdown;
    // Start is called before the first frame update
    void Start()
    {
        initializePadLines();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void initializePadLines()
    {
        padLinesDropdown.ClearOptions();
        padLinesDropdown.AddOptions(launchPadManager.padLines);
    }

    public void Connect()
    {
        FuelConnectorManager fuelConnectorManager = FindObjectOfType<FuelConnectorManager>();
        launchPadManager.connectedContainersPerLine[padLinesDropdown.GetComponent<TMP_Dropdown>().value] = fuelConnectorManager.output.guid;
        fuelConnectorManager.output = null;
        fuelConnectorManager.input = null;
        fuelConnectorManager.ShowConnection();

        fuelConnectorManager.connectPopUp.SetActive(true);
        fuelConnectorManager.PanelFadeIn(fuelConnectorManager.connectPopUp);
        fuelConnectorManager.StartCoroutine(fuelConnectorManager.ActiveDeactive(1,fuelConnectorManager.connectPopUp, false ));
    }
}
