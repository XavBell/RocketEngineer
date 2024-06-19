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

    public void initializeRocketLines()
    {
        rocketLinesDropdown.ClearOptions();
        rocketLinesDropdown.AddOptions(rocketController.lineNames);
    }
}
