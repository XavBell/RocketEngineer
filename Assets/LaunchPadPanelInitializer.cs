using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LaunchPadPanelInitializer : MonoBehaviour
{
    public TMP_Dropdown padLinesDropdown;
    public TMP_Dropdown rocketLinesDropdown;
    public launchPadManager launchPadManager;
    public RocketController rocketController;

    // Start is called before the first frame update
    void Start()
    {
        initializePadLines();
        initializeRocketLines(launchPadManager.rocketData);
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

    public void initializeRocketLines(RocketData rocketData)
    {
        rocketLinesDropdown.ClearOptions();
        rocketLinesDropdown.AddOptions(rocketData.lineNames);
    }

    public void connectLines()
    {
        launchPadManager.connectedRocketLines[padLinesDropdown.value] = launchPadManager.rocketData.lineGuids[rocketLinesDropdown.value];
        Debug.Log("Connected rocket line " + launchPadManager.rocketData.lineGuids[rocketLinesDropdown.value] + " to launch pad line " + launchPadManager.connectedContainersPerLine[padLinesDropdown.value]);
    }
}
